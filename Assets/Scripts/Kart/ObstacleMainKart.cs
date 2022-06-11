using UnityEngine;

namespace Kart
{
	public class ObstacleMainKart : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;
			
			var collisionPoint = other.ClosestPoint(transform.position);
			GameEvents.InvokeObstacleCollision(collisionPoint);
		}
	}
}