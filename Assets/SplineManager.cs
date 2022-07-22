using System;
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

	private bool _foundEmptySpline;
	
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
		var copyToIndex = splines[0].pointCount;
		print("Copy To Index = " + copyToIndex);

		if (splines[0].triggerGroups.Length != 0)
		{
			individualTriggerGroups.Add(splines[0].triggerGroups[0]);
			// _babySplineEdges.Add(0);
			// _babySplineEdges.Add(copyToIndex);
		}
		_babySplineEdges.Add(0);
		_babySplineEdges.Add(copyToIndex - 1);

		for (var i = 1; i < splines.Count; i++)
		{
			var tempArray = new SplinePoint[splines[i].pointCount - 1];

			Array.Copy(splines[i].GetPoints(), 1,
				_splinePoints, copyToIndex,
				splines[i].pointCount - 1);

			if (splines[i].triggerGroups.Length != 0)
			{
				individualTriggerGroups.Add(splines[i].triggerGroups[0]);
				//print("babySplineEdge 0 = " + copyToIndex);
				// if(!_babySplineEdges.Contains(copyToIndex - 1)) _babySplineEdges.Add(copyToIndex - 1);
				//print("babySplineEdge 1 = " + (copyToIndex - 1));
				// copyToIndex += tempArray.Length;
				// _babySplineEdges.Add(copyToIndex - 1);
				//print("babySplineEdge 2 = " + (copyToIndex - 1));
			}
			// else
			// {
			// 	copyToIndex += tempArray.Length;
			// }
			//print("babySplineEdge 0 = " + copyToIndex);
			if(!_babySplineEdges.Contains(copyToIndex - 1)) _babySplineEdges.Add(copyToIndex - 1);
			//print("babySplineEdge 1 = " + (copyToIndex - 1));
			copyToIndex += tempArray.Length;
			_babySplineEdges.Add(copyToIndex - 1);
			//print("babySplineEdge 2 = " + (copyToIndex - 1));
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
		// keeps a record of whether every spline has triggers or no
		var splineTriggerRecord = new bool[splines.Count];
		var addedTriggerIndex = 0;
		
		// var totalTriggers = splines.Sum(spline => spline.triggerGroups[0].triggers.Length);
		var totalTriggers = 0;

		for (var i = 0; i < splines.Count; i++)
		{
			var spline = splines[i];
			print("Spline["+i+"] " + "TriggerGroupLength = " + spline.triggerGroups.Length);
			if (spline.triggerGroups.Length == 0) continue;
			
			totalTriggers += spline.triggerGroups[0].triggers.Length;
			splineTriggerRecord[i] = true;
		}

		var x = 0;
		var array = new SplineTrigger[totalTriggers];
		for (var splineIndex = 0; splineIndex < splines.Count; splineIndex++)
		{
			if (!splineTriggerRecord[splineIndex])
			{
				print("Spline["+splineIndex+"] has no triggers");
				continue;
			}
			print("Spline["+splineIndex+"] has triggers");
			print("Spline Index = " + splineIndex);
			print("_babySplineEdges[splineIndex - 1] = " + _babySplineEdges[splineIndex]);
			print("_babySplineEdges[splineIndex] = " + _babySplineEdges[splineIndex + 1]);
			var startPoint = combinedSpline.GetPointPercent(_babySplineEdges[splineIndex]);
			var endPoint = combinedSpline.GetPointPercent(_babySplineEdges[splineIndex + 1]);
			
			var triggersCount = individualTriggerGroups[x].triggers.Length;
			//print("Trigger Count = " + triggersCount);
			
			for (var triggerIndex = 0; triggerIndex < triggersCount; triggerIndex++)
			{
				var currentTrigger = individualTriggerGroups[x].triggers[triggerIndex];
				var currentTriggerPosition = currentTrigger.position;
				var currentTriggerOnCrossEvent = currentTrigger.onCross;
				
				array[addedTriggerIndex++] = new SplineTrigger(SplineTrigger.Type.Double)
				{
					position = MyHelpers.LerpClampedDouble(startPoint, endPoint, currentTriggerPosition),
					onCross = currentTriggerOnCrossEvent
				};
			}

			print(x);
			x++;
		}

		combinedSpline.triggerGroups = new TriggerGroup[1]
		{
			new TriggerGroup()
		};
		combinedSpline.triggerGroups[0].triggers = array;
	}
}
