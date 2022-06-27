using DG.Tweening;
using Kart;
using UnityEngine;

public class CameraFxController : MonoBehaviour
{
	public static CameraFxController only;

	[SerializeField] private GameObject speedParticleSystem;
	[SerializeField] private float shakeDuration = 5f,shakeStrength = 5f;
	
	private Camera _cam;
	private Tween _fovTween, _screenShakeEnd;

	private Tweener _screenShakeTween;

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
		GameEvents.PlayerDeath += OnPlayerDeath;
	}

	private void OnDisable()
	{
		GameEvents.PlayerDeath -= OnPlayerDeath;
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

	public void ScreenShake(float intensity)
	{
		var target = DampCamera.only.MediateTarget();

		if (_screenShakeTween.IsActive()) _screenShakeTween.Kill(true);
		_screenShakeTween = target.DOShakePosition(shakeDuration, shakeStrength * intensity, 10, 45f)
			.OnComplete(RequestEndScreenShaker);
	}

	private void RequestEndScreenShaker()
	{
		if (_screenShakeEnd.IsActive()) _screenShakeEnd.Kill();
		
		_screenShakeEnd = DOVirtual.DelayedCall(0.5f, () => DampCamera.only.StopMediatingTarget());
	}

	public void SetSpeedLinesStatus(bool status) => speedParticleSystem.SetActive(status);

	private void OnPlayerDeath() => SetSpeedLinesStatus(false);
}