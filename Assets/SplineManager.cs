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

	public List<TriggerGroup> individualTriggerGroups = new List<TriggerGroup>();
	private readonly List<int> _babySplineEdges = new List<int>();

	public void Awake() => GameObject.FindGameObjectWithTag("Player").GetComponent<SplineFollower>().spline = combinedSpline;

	[ContextMenu("Join Splines")]
	public void JoinSplines()
	{
		_totalSplinePoints = 0;
		_babySplineEdges.Clear();
		individualTriggerGroups.Clear();
		
		//foreach (var spline in splines) _totalSplinePoints += spline.pointCount;

		_totalSplinePoints += splines[0].pointCount;
		for (var i = 1; i < splines.Count; i++)
		{
			_totalSplinePoints += splines[i].pointCount - 1;
		}

		print("Total Spline Points = " + _totalSplinePoints);
		// _splinePoints = new SplinePoint[_totalSplinePoints - 1];
		_splinePoints = new SplinePoint[_totalSplinePoints];
		
		print("_SplinePoints = " + _splinePoints.Length);

		GetAllTheSplinePoints();
	}

	private void GetAllTheSplinePoints()
	{
		//initialise empty array with spline1.length + splineN.len-1 ... n
		
		splines[0].GetPoints().CopyTo(_splinePoints,0);
		// var copyToIndex = splines[0].pointCount - 1;
		var copyToIndex = splines[0].pointCount;
		print("Copy To Index = " + copyToIndex);

		if (splines[0].triggerGroups.Length != 0)
		{
			individualTriggerGroups.Add(splines[0].triggerGroups[0]);
			_babySplineEdges.Add(0);
			_babySplineEdges.Add(copyToIndex);
		}

		for (var i = 1; i < splines.Count; i++)
		{
			var tempArray = new SplinePoint[splines[i].pointCount - 1];

			for (var j = 0; j < tempArray.Length; j++) tempArray[j] = splines[i].GetPoint(j + 1);

			tempArray.CopyTo(_splinePoints, copyToIndex);
			
			//_babySplineEdges.Add(copyToIndex);

			if (splines[i].triggerGroups.Length != 0)
			{
				individualTriggerGroups.Add(splines[i].triggerGroups[0]);
				if(!_babySplineEdges.Contains(copyToIndex)) _babySplineEdges.Add(copyToIndex);
				//_babySplineEdges.Add(copyToIndex);
				copyToIndex += tempArray.Length;
				_babySplineEdges.Add(copyToIndex);
			}
			else
			{
				copyToIndex += tempArray.Length;
			}

		}
		
		combinedSpline.SetPoints(_splinePoints);

		foreach (var babySplineEdge in _babySplineEdges)
		{
			print("Edge = " + babySplineEdge);
			print("Spline Point = " + combinedSpline.GetPoint(babySplineEdge).position);
		}
		ReArrangeTriggers();
	}
	private void ReArrangeTriggers()
	{
		var x = 0;
		var totalSplinesWithTriggers = 0;
		
		// var totalTriggers = splines.Sum(spline => spline.triggerGroups[0].triggers.Length);
		var totalTriggers = 0;

		foreach (var spline in splines)
		{
			if (spline.triggerGroups.Length != 0)
			{
				totalTriggers += spline.triggerGroups[0].triggers.Length;
				totalSplinesWithTriggers++;
			}
		}
		
		print("Total Splines With Triggers = " + totalSplinesWithTriggers);
		var array = new SplineTrigger[totalTriggers];
		for (var splineIndex = 0; splineIndex < totalSplinesWithTriggers; splineIndex++)
		{
			var startPoint = combinedSpline.GetPointPercent(_babySplineEdges[splineIndex] - 1);
			var endPoint = combinedSpline.GetPointPercent(_babySplineEdges[splineIndex + 1] - 1);
			
			var triggersCount = individualTriggerGroups[splineIndex].triggers.Length;
			//print("Trigger Count = " + triggersCount);
			
			for (var triggerIndex = 0; triggerIndex < triggersCount; triggerIndex++)
			{
				var currentTrigger = individualTriggerGroups[splineIndex].triggers[triggerIndex];
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
