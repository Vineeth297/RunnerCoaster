using DG.Tweening;
using UnityEngine;

public class CameraFxController : MonoBehaviour
{
	public static CameraFxController only;

	[SerializeField] private GameObject speedParticleSystem;
	[SerializeField] private Transform finalPosition;
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
		GameEvents.ObstacleCollision += OnObstacleCollision;
	}

	private void OnDisable()
	{
		GameEvents.ObstacleCollision -= OnObstacleCollision;
	}
	
	public void DoNormalFov() => _cam.DOFieldOfView(60, 0.5f);

	public void DoWideFov() => _cam.DOFieldOfView(70, 0.5f);

	public void SetSpeedLinesStatus(bool status) => speedParticleSystem.SetActive(status);
	

	private void OnObstacleCollision(Vector3 collisionPoint)
	{
		SetSpeedLinesStatus(false);
	}
}