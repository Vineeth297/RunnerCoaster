using DG.Tweening;
using UnityEngine;

public class CameraFxController : MonoBehaviour
{
	public static CameraFxController only;

	[SerializeField] private GameObject speedParticleSystem;
	private Camera _cam;
	private Tween _fovTween;

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
	
	public void DoNormalFov()
	{
		if (_fovTween.IsActive()) _fovTween.Kill();
		_fovTween = _cam.DOFieldOfView(60, 0.5f);
	}

	public void DoWideFov()
	{
		if (_fovTween.IsActive()) _fovTween.Kill();
		_fovTween = _cam.DOFieldOfView(70, 0.5f);
	}

	public void DoCustomFov(float fov)
	{
		if (_fovTween.IsActive()) _fovTween.Kill();
		_fovTween = _cam.DOFieldOfView(fov, 0.5f);
	}

	public void SetSpeedLinesStatus(bool status) => speedParticleSystem.SetActive(status);
	

	private void OnObstacleCollision(Vector3 collisionPoint)
	{
		SetSpeedLinesStatus(false);
	}
}