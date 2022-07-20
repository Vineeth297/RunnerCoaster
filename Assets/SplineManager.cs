using System.Collections.Generic;
using System.Linq;
using Dreamteck;
using Dreamteck.Splines;
using UnityEngine;

public class SplineManager : MonoBehaviour
{
	[SerializeField] private SplineComputer combinedSpline;
	[SerializeField] private List<SplineComputer> splines;
	
	private SplinePoint[] _splinePoints;
	private int _totalSplinePoints;

	private readonly List<TriggerGroup> _individualTriggerGroups = new List<TriggerGroup>();
	private readonly List<int> _babySplineEdges = new List<int>();

	public void Awake() => GameObject.FindGameObjectWithTag("Player").GetComponent<SplineFollower>().spline = combinedSpline;

	[ContextMenu("Join Splines")]
	public void JoinSplines()
	{
		_totalSplinePoints = 0;
		_babySplineEdges.Clear();
		_individualTriggerGroups.Clear();
		
		foreach (var spline in splines) _totalSplinePoints += spline.pointCount;

		_splinePoints = new SplinePoint[_totalSplinePoints - 1];

		GetAllTheSplinePoints();
	}
	
	private void GetAllTheSplinePoints()
	{
		//initialise empty array with spline1.length + splineN.len-1 ... n
		
		splines[0].GetPoints().CopyTo(_splinePoints,0);
		
		_individualTriggerGroups.Add(splines[0].triggerGroups[0]);
		_babySplineEdges.Add(0);
		
		var copyToIndex = splines[0].pointCount - 1;
		_babySplineEdges.Add(copyToIndex);
		
		for (var i = 1; i < splines.Count; i++)
		{
			var tempArray = new SplinePoint[splines[i].pointCount];

			for (var j = 0; j < tempArray.Length; j++) tempArray[j] = splines[i].GetPoint(j);

			tempArray.CopyTo(_splinePoints, copyToIndex);
			
			copyToIndex += tempArray.Length;
			
			_individualTriggerGroups.Add(splines[i].triggerGroups[0]);
			_babySplineEdges.Add(copyToIndex);
		}
		
		combinedSpline.SetPoints(_splinePoints);
		
		ReArrangeTriggers();
	}

	private void ReArrangeTriggers()
	{
		var x = 0;
		
		var totalTriggers = splines.Sum(spline => spline.triggerGroups[0].triggers.Length);

		var array = new SplineTrigger[totalTriggers];
		for (var splineIndex = 0; splineIndex < splines.Count; splineIndex++)
		{
			var startPoint = combinedSpline.GetPointPercent(_babySplineEdges[splineIndex]);
			var endPoint = combinedSpline.GetPointPercent(_babySplineEdges[splineIndex + 1]);

			var triggersCount = _individualTriggerGroups[splineIndex].triggers.Length;
			
			for (var triggerIndex = 0; triggerIndex < triggersCount; triggerIndex++)
			{
				var currentTrigger = _individualTriggerGroups[splineIndex].triggers[triggerIndex];
				var currentTriggerPosition = currentTrigger.position;
				var currentTriggerOnCrossEvent = currentTrigger.onCross;
				
				array[x++] = new SplineTrigger(SplineTrigger.Type.Double)
				{
					position = MyHelpers.LerpClampedDouble(startPoint, endPoint, currentTriggerPosition),
					onCross = currentTriggerOnCrossEvent
				};
			}
		}

		combinedSpline.triggerGroups = new TriggerGroup[1]
		{
			new TriggerGroup()
		};
		combinedSpline.triggerGroups[0].triggers = array;
	}
}
