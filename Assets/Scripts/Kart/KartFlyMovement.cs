﻿using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class KartFlyMovement : MonoBehaviour
	{
		[SerializeField] private Limits forwardSpeedLimits, downwardSpeedLimits;

		private Transform _transform;
		private Vector3 _currentMovementVector;
		private float _currentForwardSpeed, _currentDownSpeed, _lowestAllowedY;
		private bool _shouldMove = true;

		private Tween _forwardSpeedTween, _downSpeedTween;
		
		private void OnEnable()
		{
			GameEvents.StopOnBonusRamp += OnStopTheRollerCoaster;
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
		}

		private void OnDisable()
		{
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
			GameEvents.StopOnBonusRamp -= OnStopTheRollerCoaster;
		}

		private void Start()
		{
			_transform = transform;
			_lowestAllowedY = GameObject.FindGameObjectWithTag("BonusRamp").GetComponent<BonusRamp>().LowestPointY;
		}
		
		public void SetForwardOrientedValues()
		{
			if (!_shouldMove) return;
			
			_currentForwardSpeed = forwardSpeedLimits.max;
			_currentDownSpeed = downwardSpeedLimits.min;
		}

		public void SetDownwardOrientedValues()
		{
			if (!_shouldMove) return;
			_currentForwardSpeed = forwardSpeedLimits.min;
			_currentDownSpeed = downwardSpeedLimits.max;
		}
		
		public void CalculateForwardMovement() => 
			_currentMovementVector += transform.forward * (_currentForwardSpeed * Time.deltaTime);

		public void CalculateDownwardMovement() => 
			_currentMovementVector += -transform.up * (_currentDownSpeed * Time.deltaTime);

		public void ApplyMovement()
		{
			if(!_shouldMove) return;
			if (_transform.position.y < _lowestAllowedY)
			{
				_shouldMove = false;
				const float duration = 2f;
				_transform.DOMove(_transform.position + _transform.forward * (_currentForwardSpeed * Time.deltaTime * duration), duration);
			}

			_transform.position += _currentMovementVector;
			_currentMovementVector = Vector3.zero;
		}
		
		private void OnReachEndOfTrack()
		{
			transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f);
		}
		
		private void OnStopTheRollerCoaster()
		{
			_shouldMove = false;
			forwardSpeedLimits.min = forwardSpeedLimits.max = _currentForwardSpeed = 0f;
			downwardSpeedLimits.min = downwardSpeedLimits.max = _currentDownSpeed = 0f;
		}
	}
}