using DG.Tweening;
using UnityEngine;

namespace Player
{
	public class PlayerFlyMovement : MonoBehaviour
	{
		[SerializeField] private float minimumMoveSpeed = 5f;
		[SerializeField] private float maximumMoveSpeed = 5f;
		[SerializeField] private float minimumDownSpeed = 5f;
		[SerializeField] private float maximumDownSpeed = 5f;

		private Transform _transform;
		private Vector3 _currentMovementVector;
		private float _currentForwardSpeed, _currentDownSpeed;
		private bool _hasStopped;

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
		}

		public void SetForwardOrientedValues()
		{
			if (_hasStopped) return;
			_currentForwardSpeed = maximumMoveSpeed;
			_currentDownSpeed = minimumDownSpeed;
		}

		public void SetDownwardOrientedValues()
		{
			if (_hasStopped) return;
			_currentForwardSpeed = minimumMoveSpeed;
			_currentDownSpeed = maximumDownSpeed;
		}
		
		public void CalculateForwardMovement() => 
			_currentMovementVector += transform.forward * (_currentForwardSpeed * Time.deltaTime);

		public void CalculateDownwardMovement() => 
			_currentMovementVector += -transform.up * (_currentDownSpeed * Time.deltaTime);

		public void ApplyMovement()
		{
			_transform.position += _currentMovementVector;
			_currentMovementVector = Vector3.zero;
		}
		
		private void OnReachEndOfTrack()
		{
			transform.DORotate(new Vector3(0f, 90f, 0f), 0.5f);
		}
		
		private void OnStopTheRollerCoaster()
		{
			_hasStopped = true;
			minimumMoveSpeed = maximumMoveSpeed = _currentForwardSpeed = 0f;
			minimumDownSpeed = maximumDownSpeed = _currentDownSpeed = 0f;
		}
	}
}