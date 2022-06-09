using System;
using DG.Tweening;
using UnityEngine;

public class CameraFxController : MonoBehaviour
{
	public static CameraFxController only;
	
	[SerializeField] private GameObject finalPosition;
	
	[SerializeField] private GameObject speedParticleSystem;
	private Camera _cam;

	private void Awake()
	{
		if (!only) only = this;
		else Destroy(only);
	}

	private void Start()
	{
		_cam = Camera.main;
	}

	private void OnEnable()
	{
		GameEvents.ExplosionCameraPushBack += OnExplosion;
		GameEvents.BonusCameraPushBack += OnBonus;
	}

	private void OnDisable()
	{
		GameEvents.ExplosionCameraPushBack -= OnExplosion;
		GameEvents.BonusCameraPushBack -= OnBonus;
	}

	public void SetSpeedLinesStatus(bool status) => speedParticleSystem.SetActive(status);

	private void MoveToFinalPosition() => _cam.transform.DOMove(finalPosition.transform.position, 0.5f);

	private void OnExplosion()
	{
		MoveToFinalPosition();
		SetSpeedLinesStatus(false);
	}

	private void OnBonus() => MoveToFinalPosition();
}