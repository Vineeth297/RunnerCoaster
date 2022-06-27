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
		}
		
		private void InitMainKart()
		{
			if (isMainKart) _mainKart = this;

			Wagon currentFront = null;
			Wagon candidate = GetComponent<Wagon>();
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
			
			_mainKart.MainKartCollisionEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;
			
			_mainKart.MainKartCollisionExit(other);
		}

		private static void InvokeStopAllObstacleTrains() => StopAllObstacleTrains?.Invoke();
		private static void InvokeStartAllObstacleTrains() => StartAllObstacleTrains?.Invoke();

		private void OnStopAllObstacleTrains() => _my.TrackMovement.StopFollowingTrack();

		private void OnStartAllObstacleTrains() 
		{
			_my.TrackMovement.StartFollowingTrack();
		}
	}
}