using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;

	public float distance = 10.0f;
	public float height = 5.0f;
	public float heightDamping = 2.0f;

//	[HideInInspector] public Ray ray;

	[HideInInspector] public Camera camera;

	[AddComponentMenu("Camera-Control/Smooth Follow")]

	private bool _aiming;

	private void Start()
	{
		camera = Camera.main;
	}
	private void Update()
	{
		//if (!_aiming) return;
		
		/*ray = camera.ScreenPointToRay(transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 50f))
		{
			Debug.DrawRay(transform.position, transform.forward * 50);
		}*/
	}

	private void LateUpdate () 
	{
		/*if (!target) return;

		if (_aiming) return;*/
		transform.LookAt(target);
		var targetPosition = target.position;
		float wantedHeight = targetPosition.y + height;

		var position = transform.position;
		float currentHeight = position.y;

		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		position = new Vector3(position.x, targetPosition.y, targetPosition.z); // target.position;
		position -= Vector3.forward * distance;

		// Set the height of the camera
		position = new Vector3(position.x,currentHeight,position.z);
		transform.position = position;
	}
}
