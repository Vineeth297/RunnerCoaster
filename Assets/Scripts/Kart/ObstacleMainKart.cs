using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class ObstacleMainKart : MonoBehaviour
	{
		private MainKartController _my;
		private void Start()
		{
			_my = GetComponent<MainKartController>();

			Tween checker = null;
			checker = DOVirtual.DelayedCall(0.1f, () =>
			{
				if (!_my.isInitialised) return;

				_my.Follower.follow = true;
				checker.Kill();
			}).SetLoops(-1);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;
			
			var collisionPoint = other.ClosestPoint(transform.position);
			GameEvents.InvokeObstacleCollision(collisionPoint);
		}
	}
}