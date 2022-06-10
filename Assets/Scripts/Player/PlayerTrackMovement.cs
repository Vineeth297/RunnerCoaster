using UnityEngine;

namespace Player
{
	[System.Serializable] public struct Limits { public float min, max; }
	public class PlayerTrackMovement : MonoBehaviour
	{
		private PlayerRefBank _my;
		
		[SerializeField] private AnimationCurve speedGain, speedLoss;

		[SerializeField] private Limits plainSpeedLimits, highSpeedLimits;
		[SerializeField] private float speed = 10f;
		[SerializeField] private float brakeSpeed = 0f, brakeReleaseSpeed = 0f;

		[SerializeField] private float frictionForce = 0.1f, gravityForce = 1f;
		[SerializeField] private float slopeRange = 60f;

		private Limits _currentLimits;
		private const float BrakeTime = 0f;
		private float _brakeForce = 0f, _addForce = 0f;
		private bool _toMove;

		private void OnEnable()
		{
			GameEvents.ObstacleCollision += OnExplosion;
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
		}

		private void OnDisable()
		{
			GameEvents.ObstacleCollision -= OnExplosion;
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
		}

		private void Start()
		{
			_my = GetComponent<PlayerRefBank>();
			_currentLimits = plainSpeedLimits;
		}

		public void CalculateForces()
		{
			var dot = Vector3.Dot(transform.forward, Vector3.down);
			var dotPercent = Mathf.Lerp(-slopeRange / 90f, slopeRange / 90f, (dot + 1f) / 2f);
			speed -= Time.deltaTime * frictionForce * (1f - _brakeForce);
			var speedAdd = 0f;
			var speedPercent = Mathf.InverseLerp(_currentLimits.min, _currentLimits.max, speed);
			if (dotPercent > 0f)
				speedAdd = gravityForce * dotPercent * speedGain.Evaluate(speedPercent) * Time.deltaTime;
			else
				speedAdd = gravityForce * dotPercent * speedLoss.Evaluate(1f - speedPercent) * Time.deltaTime;
			speed += speedAdd * (1f - _brakeForce);
			speed = Mathf.Clamp(speed, _currentLimits.min, _currentLimits.max);
			if (_addForce < 0f) return;
		
			var lastAdd = _addForce;
			_addForce = Mathf.MoveTowards(_addForce, 0f, Time.deltaTime * 30f);
			speed += lastAdd - _addForce;
		}

		public void Accelerate()
		{
			_my.Follower.followSpeed = speed;
			_my.Follower.followSpeed *= 1f - _brakeForce;
		}

		public void Brake()
		{
			_my.Follower.followSpeed -= Time.deltaTime * speed;
		}

		public void CalculateBraking()
		{
			_brakeForce = BrakeTime > Time.time ?
				Mathf.MoveTowards(_brakeForce, 1f, Time.deltaTime * brakeSpeed) :
				Mathf.MoveTowards(_brakeForce, 0f, Time.deltaTime * brakeReleaseSpeed);
		}

		public void StartFollow() => _my.Follower.follow = true;
		public void SetHighSpeedValues() => _currentLimits = highSpeedLimits;
		public void SetNormalSpeedValues() => _currentLimits = plainSpeedLimits;

		private void StopFollowingTrack() => _my.Follower.follow = false;

		private void OnExplosion() => StopFollowingTrack();

		private void OnReachEndOfTrack() => StopFollowingTrack();
	}
}