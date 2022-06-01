using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public int totalAdditionalKarts;
	public int numberOfActiveKarts;

	private Camera _camera;
	public float cameraIncrementInY = 0.25f;
	public float cameraIncrementInZ = 1.16f;
	
	private void Awake()
	{
		if (Instance)
			Destroy(gameObject);
		else
			Instance = this;
	}

	private void Start()
	{
		_camera = Camera.main;
	}
	

	public void MoveCameraFront()
	{
		_camera.transform.position -= new Vector3(0f,cameraIncrementInY,cameraIncrementInZ);
	}

	public void MoveCameraBack()
	{
		print(_camera.transform.position.y);
		_camera.transform.localPosition += new Vector3(0f,cameraIncrementInY,-cameraIncrementInZ);
		print(_camera.transform.position);
	}

	public void SpeedPullEffect()
	{
		_camera.DOFieldOfView(60, 0.5f);
	}
	
	public void SpeedPushEffect()
	{
		_camera.DOFieldOfView(70, 0.5f);
	}
}
