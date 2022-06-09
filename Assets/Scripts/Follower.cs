using Dreamteck.Splines;
using UnityEngine;

public class Follower : MonoBehaviour
{
	private SplinePositioner _spline;

	private void Start()
	{
		_spline = GetComponent<SplinePositioner>();
	}

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetMouseButton(0))
			_spline.position += Time.deltaTime;
	}
}