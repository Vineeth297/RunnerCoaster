using System;
using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class SplineManager : MonoBehaviour
{
	public List<SplineComputer> splines;

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
		foreach (var spline in splines)
		{
			_totalSplinePoints += spline.pointCount;
		}
		
		_splinePoints = new SplinePoint[_totalSplinePoints];
		GetAllTheSplinePoints();

		//splines.RemoveRange(1, splines.Count - 1);
		print("Joined splines successfully.");
	}
	
	private void GetAllTheSplinePoints()
	{
		//initialise empty array with spline1.length + splineN.len-1 ... n
		
		splines[0].GetPoints().CopyTo(_splinePoints,0);
		var copyToIndex = splines[0].pointCount;
		for (var i = 1; i < splines.Count; i++)
		{
			splines[i].GetPoints().CopyTo(_splinePoints, copyToIndex);
			
			copyToIndex += splines[i].pointCount;
		}

		splines[0].SetPoints(_splinePoints);
		splines[0].RebuildImmediate(true, true);
	}
}
