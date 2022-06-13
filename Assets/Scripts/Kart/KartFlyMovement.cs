using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class KartFlyMovement : MonoBehaviour
	{
		[SerializeField] private Limits forwardSpeedLimits, downwardSpeedLimits;
		[SerializeField] private Ease speedTweenEase;
		[SerializeField] private float speedTweenDuration;

		private Transform _transform;
		private BonusRamp _bonusRamp;
		private Vector3 _currentMovementVector;
		private float _currentForwardSpeed, _currentDownSpeed, _lowestAllowedY;
		private bool _shouldMove = true;

		private Tween _speedTween;

		private void OnEnable()
		{
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
			GameEvents.RunOutOfPassengers += OnStopTheRollerCoaster;
		}

		private void OnDisable()
		{
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
			GameEvents.RunOutOfPassengers -= OnStopTheRollerCoaster;
		}

		private void Start()
		{
			_transform = transform;
			_bonusRamp = GameObject.FindGameObjectWithTag("BonusRamp").GetComponent<BonusRamp>();
			_lowestAllowedY = _bonusRamp.LowestPointY - 1.8f;
		}
		
		public void SetForwardOrientedValues()
		{
			if (!_shouldMove) return;

			if (_speedTween.IsActive()) _speedTween.Kill();

			var tempVal = 0f;
			_speedTween = DOTween.To(() => tempVal, value => tempVal = value, 1f, speedTweenDuration)
				.SetEase(speedTweenEase)
				.OnUpdate(() =>
				{
					_currentForwardSpeed = Mathf.Lerp(_currentForwardSpeed, forwardSpeedLimits.max, tempVal);
					_currentDownSpeed = Mathf.Lerp(_currentDownSpeed, downwardSpeedLimits.min, tempVal);
				});
		}

		public void SetDownwardOrientedValues()
		{
			if (!_shouldMove) return;
			
			if (_speedTween.IsActive()) _speedTween.Kill();

			var tempVal = 0f;
			_speedTween = DOTween.To(() => tempVal, value => tempVal = value, 1f, speedTweenDuration)
				.SetEase(speedTweenEase)
				.OnUpdate(() =>
				{
					_currentForwardSpeed = Mathf.Lerp(_currentForwardSpeed, forwardSpeedLimits.min, tempVal);
					_currentDownSpeed = Mathf.Lerp(_currentDownSpeed, downwardSpeedLimits.max, tempVal);
				});
		}
		
		public void CalculateForwardMovement() => 
			_currentMovementVector += transform.forward * (_currentForwardSpeed * Time.deltaTime);

		public void CalculateDownwardMovement() => 
			_currentMovementVector += -transform.up * (_currentDownSpeed * Time.deltaTime);

		public void ApplyMovement()
		{
			if(!_shouldMove) return;
			if (_transform.position.y < _lowestAllowedY) BringToAStop();

			_transform.position += _currentMovementVector;
			_currentMovementVector = Vector3.zero;
		}

		private void BringToAStop()
		{
			_shouldMove = false;
			const float duration = 1.75f;
			DOTween.To(() => _currentForwardSpeed, value => _currentForwardSpeed = value, 0f, duration)
				.SetEase(Ease.OutQuint)
				.OnUpdate(() => _transform.position += transform.forward * (_currentForwardSpeed * Time.deltaTime));

			_transform.DOMoveY(_lowestAllowedY, duration).SetEase(Ease.OutQuint);
		}

		private void OnReachEndOfTrack()
		{
			var dir = _bonusRamp.transform.forward;
			dir.y = 0;
			transform.DORotateQuaternion( Quaternion.LookRotation(dir), 0.5f);
		}

		private void OnStopTheRollerCoaster()
		{
			BringToAStop();
			//forwardSpeedLimits.min = forwardSpeedLimits.max = _currentForwardSpeed = 0f;
			downwardSpeedLimits.min = downwardSpeedLimits.max = _currentDownSpeed = 0f;
		}
	}
}