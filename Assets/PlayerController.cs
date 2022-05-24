using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private SplineFollower _spline;

	private void Start()
	{
		_spline = GetComponent<SplineFollower>();
		_spline.follow = false;
	}
	private void Update()
	{
		_spline.follow = Input.GetMouseButton(0);
	}
}
