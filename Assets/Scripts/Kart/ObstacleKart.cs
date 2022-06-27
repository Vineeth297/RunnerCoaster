using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class ObstacleKart : MonoBehaviour
	{
		private static event Action StopAllObstacleTrains, StartAllObstacleTrains;
		
		[SerializeField] private bool isMainKart;
		
		private MainKartController _my;
		private ObstacleKart _mainKart;

		private readonly HashSet<Collider> _playerColliders = new HashSet<Collider>();
		private Tween _waitToCheckIfOutOfCollision;
		private static bool _outOfCollision;
		private bool _shouldIgnoreFunctionality;

		private void OnEnable()
		{
			if (!isMainKart) return;
			StopAllObstacleTrains += OnStopAllObstacleTrains;
			StartAllObstacleTrains += OnStartAllObstacleTrains;
		}

		private void OnDisable()
		{
			if (!isMainKart) return;
			StopAllObstacleTrains -= OnStopAllObstacleTrains;
			StartAllObstacleTrains -= OnStartAllObstacleTrains;
		}

		private void Start()
		{
			InitMainKart();
			
			if(!isMainKart) return;
			_my = GetComponent<MainKartController>();

			Tween checker = null;
			checker = DOVirtual.DelayedCall(0.1f, () =>
			{
				if (!_my.isInitialised) return;

				_my.Follower.follow = true;
				checker.Kill();
			}).SetLoops(-1);

			_waitToCheckIfOutOfCollision.SetRecyclable(true);
			_waitToCheckIfOutOfCollision.SetAutoKill(false);
		}
		
		private void InitMainKart()
		{
			if (isMainKart) _mainKart = this;

			Wagon currentFront = null;
			Wagon candidate = GetComponent<Wagon>();
			
			//other obstacles also use this script, but dont have wagons
			if(!candidate)
			{
				_shouldIgnoreFunctionality = true;
				return;
			}
			
			do
			{
				candidate = candidate.front;
				if (candidate)
					currentFront = candidate;
			} while (candidate);

			if(currentFront != null)
				_mainKart = currentFront.GetComponent<ObstacleKart>();
		}

		private void MainKartCollisionEnter(Collider other)
		{
			if(!_playerColliders.Contains(other)) _playerColliders.Add(other);

			if (_playerColliders.Count <= 0) return;
			
			InvokeStopAllObstacleTrains();
		}
		
		private void MainKartCollisionExit(Collider other)
		{
			if(_playerColliders.Contains(other)) _playerColliders.Remove(other);

			if (_playerColliders.Count != 0) return;
			
			InvokeStartAllObstacleTrains();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;

			var collisionPoint = other.ClosestPoint(transform.position);
			GameEvents.InvokeKartCrash(collisionPoint);
			
			if(_shouldIgnoreFunctionality) return;

			Wagon hit = null;
			if (other.TryGetComponent(out AdditionalKartController addy)) hit = addy.Wagon;
			else if (other.TryGetComponent(out MainKartController addu)) hit = addu.Wagon;
			
			

			_mainKart.MainKartCollisionEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			if(_shouldIgnoreFunctionality) return;
			if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;
			
			_mainKart.MainKartCollisionExit(other);

			if (_waitToCheckIfOutOfCollision.IsActive()) _waitToCheckIfOutOfCollision.Kill();
			_waitToCheckIfOutOfCollision = DOVirtual.DelayedCall(4f, () =>
			{
				InvokeStartAllObstacleTrains();
				_playerColliders.Clear();
			});
		}

		private static void InvokeStopAllObstacleTrains() => StopAllObstacleTrains?.Invoke();

		private static void InvokeStartAllObstacleTrains() => StartAllObstacleTrains?.Invoke();

		private void OnStopAllObstacleTrains() => _my.TrackMovement.StopFollowingTrack();

		private void OnStartAllObstacleTrains() => _my.TrackMovement.StartFollowingTrack();
	}
}