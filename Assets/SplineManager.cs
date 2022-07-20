using System.Collections.Generic;
using System.Linq;
using Dreamteck;
using Dreamteck.Splines;
using UnityEngine;

public class SplineManager : MonoBehaviour
{
	[SerializeField] private SplineComputer combinedSpline;
	public List<SplineComputer> splines;
	public List<TriggerGroup> individualTriggerGroups;

	private List<int> _babySplineEdges = new List<int>();
	private SplinePoint[] _splinePoints;
	private int _totalSplinePoints;
	
	

	public void Awake()
	{
		//splines[0].RebuildImmediate();
		//splines[0].rebuildOnAwake = true;
		GameObject.FindGameObjectWithTag("Player").GetComponent<SplineFollower>().spline = splines[0];
		
	}

	[ContextMenu("triggerGroups")]
	public void Trigg()
	{
		print(splines[0].triggerGroups[0].triggers[0].position);
		print(splines[0].GetPointPercent(3));
		
	}
	
	[ContextMenu("Join Splines")]
	public void JoinSplines()
	{
		_totalSplinePoints = 0;
		_babySplineEdges.Clear();
		individualTriggerGroups.Clear();
		
		foreach (var spline in splines)
		{
			_totalSplinePoints += spline.pointCount;
		}
		
		_splinePoints = new SplinePoint[_totalSplinePoints - 1];

		GetAllTheSplinePoints();

		//splines.RemoveRange(1, splines.Count - 1);
		print("Joined splines successfully.");
	}
	
	private void GetAllTheSplinePoints()
	{
		//initialise empty array with spline1.length + splineN.len-1 ... n
		
		splines[0].GetPoints().CopyTo(_splinePoints,0);
		print("Spline[0] point Count = " + splines[0].pointCount);
		individualTriggerGroups.Add(splines[0].triggerGroups[0]);
		_babySplineEdges.Add(0);
		var copyToIndex = splines[0].pointCount - 1;
		_babySplineEdges.Add(copyToIndex);
		
		for (var i = 1; i < splines.Count; i++)
		{
			SplinePoint[] tempArray = new SplinePoint[splines[i].pointCount];

			print("Temp Array Length = " + tempArray.Length);
			for (int j = 0; j < tempArray.Length; j++)
			{
				print("j = " + j);
				tempArray[j] = splines[i].GetPoint(j);
			}
			
			tempArray.CopyTo(_splinePoints, copyToIndex);
			
			copyToIndex += tempArray.Length;
			
			individualTriggerGroups.Add(splines[i].triggerGroups[0]);
			_babySplineEdges.Add(copyToIndex);
			print("Spline[0] point Count = " + splines[0].pointCount);
		}
		
		combinedSpline.SetPoints(_splinePoints);
		
		ReArrangeTriggers();
		//splines[0].RebuildImmediate(true, true);
		//individualTriggerGroups.Clear();
	}

	private void ReArrangeTriggers()
	{
		// var triggerPos1 = individualTriggerGroups[0].triggers[0].position;
		// print("Trigger Pos - " + triggerPos1);
		// var startPoint1 = splines[0].GetPointPercent(_babySplineEdges[0]);
		// var endPoint1 = splines[0].GetPointPercent(_babySplineEdges[1]);
		// print($"1 {startPoint1} 2 {endPoint1} originalPos {triggerPos1}");
		//
		// var newTriggerPos1 = MyHelpers.LerpClampedDouble(startPoint1, endPoint1, triggerPos1);
		// splines[0].triggerGroups[0].triggers[0].position = newTriggerPos1;
		// print("new Trigger Pos - " + newTriggerPos1);
		//
		//
		// var triggerPos2 = individualTriggerGroups[0].triggers[1].position;
		// print("Trigger Pos - " + triggerPos2);
		// var startPoint2 = splines[0].GetPointPercent(_babySplineEdges[0]);
		// var endPoint2 = splines[0].GetPointPercent(_babySplineEdges[1]);
		// print($"1 {startPoint2} 2 {endPoint2} originalPos {triggerPos2}");
		//
		// var newTriggerPos2 = MyHelpers.LerpClampedDouble(startPoint2, endPoint2, triggerPos2);
		// splines[0].triggerGroups[0].triggers[1].position = newTriggerPos2;
		// print("new Trigger Pos - " + newTriggerPos2);

		/*
		var triggersCount = individualTriggerGroups[0].triggers.Length;
		for (var i = 0; i < triggersCount; i++)
		{
			var triggerPos = individualTriggerGroups[0].triggers[i].position;
			print("Trigger Pos - " + triggerPos);
			var startPoint = splines[0].GetPointPercent(_babySplineEdges[0]);
			var endPoint = splines[0].GetPointPercent(_babySplineEdges[1]);
			print($"StartIndex - {startPoint} EndIndex - {endPoint} originalPos - {triggerPos}");

			var newTriggerPos = MyHelpers.LerpClampedDouble(startPoint, endPoint, triggerPos);
			splines[0].triggerGroups[0].triggers[i].position = newTriggerPos;
			print("new Trigger Pos - " + newTriggerPos);
		}
		*/

		var x = 0;
		
		var totalTriggers = splines.Sum(spline => spline.triggerGroups[0].triggers.Length);

		var array = new SplineTrigger[totalTriggers];
		for (var splineIndex = 0; splineIndex < splines.Count; splineIndex++)
		{
			var startPoint = combinedSpline.GetPointPercent(_babySplineEdges[splineIndex]);
			var endPoint = combinedSpline.GetPointPercent(_babySplineEdges[splineIndex + 1]);

			print($"{startPoint}, {endPoint}");
			
			var triggersCount = individualTriggerGroups[splineIndex].triggers.Length;
			
			for (var triggerIndex = 0; triggerIndex < triggersCount; triggerIndex++)
			{
				var triggerPos = individualTriggerGroups[splineIndex].triggers[triggerIndex].position;

				array[x++] = new SplineTrigger(SplineTrigger.Type.Double)
				{
					position = MyHelpers.LerpClampedDouble(startPoint, endPoint, triggerPos)
				};
				
				print($"StartIndex - {startPoint} EndIndex - {endPoint} originalPos - {triggerPos} {x}");
			}
		}

		combinedSpline.triggerGroups = new TriggerGroup[1]
		{
			new TriggerGroup()
		};
		combinedSpline.triggerGroups[0].triggers = array;
		/*print(splines.Count);
		for (var i = 0; i < splines.Count; i++)
		{
			var currentTriggerGroupsLength = individualTriggerGroups[i].triggers.Length;

			for (var j = 0; j < currentTriggerGroupsLength; j++)
			{
				var triggerPos = individualTriggerGroups[i].triggers[j].position;
				var point1 = splines[i].GetPointPercent(_babySplineEdges[i]);
				var point2 = splines[i].GetPointPercent(_babySplineEdges[i + 1]);
				print($"{_babySplineEdges[i]}, {_babySplineEdges[i + 1]}, {i}");

				var newTriggerPos = MyHelpers.LerpClampedDouble(point1, point2, triggerPos);
				//print("i = " + i + "," + "j = " + j + "," + "New Trigger Pos = " + newTriggerPos);
				//print("babySplineEdge1 = " + _babySplineEdges[i] + "," + "babySplineEdge2 = " + _babySplineEdges[i + 1]);
				splines[0].triggerGroups[0].triggers[j].position = newTriggerPos;
			}
		}*/
	}
}
