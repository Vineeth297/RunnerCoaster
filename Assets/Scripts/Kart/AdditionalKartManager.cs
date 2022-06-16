using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class AdditionalKartManager : MonoBehaviour
	{
		[SerializeField] private GameObject kartPrefab;
		[SerializeField] private float forceMultiplier, upForce;

		private List<AdditionalKartController> _additionalKarts;
		public List<GameObject> AvailablePassengers { get; private set; }

		private Wagon _lastKart;
		private MainKartController _my;


		private void OnEnable()
		{
			GameEvents.KartCrash += OnObstacleCollision;
		}

		private void OnDisable()
		{
			GameEvents.KartCrash -= OnObstacleCollision;
		}

		private void Start()
		{
			_lastKart = GetComponent<Wagon>();
			_my = GetComponent<MainKartController>();

			_additionalKarts = new List<AdditionalKartController>();
			var childCount = transform.GetChild(0).childCount;
			AvailablePassengers = new List<GameObject>
			{
				//add main kart passengers
				transform.GetChild(0).GetChild(childCount - 1).gameObject,
				transform.GetChild(0).GetChild(childCount - 2).gameObject
			};
		}

		private void PickUpThePassengers(GameObject platform)
		{
			var pickupPlatform = platform.GetComponent<PickupPlatform>();
			var kartSpawnCount = pickupPlatform.passengers.Count / 2 + 1;
			SpawnKarts(kartSpawnCount);
			pickupPlatform.JumpOnToTheKart();
		}

		public void SpawnKarts(int kartsToSpawn) => DOVirtual.DelayedCall(0.15f, SpawnNewKart).SetLoops(kartsToSpawn);

		private void SpawnNewKart()
		{
			var newKart = Instantiate(kartPrefab, transform.parent).GetComponent<AdditionalKartController>();
			_additionalKarts.Add(newKart);
			
			//add new kart passengers
			AvailablePassengers.Add(newKart.transform.GetChild(1).gameObject);
			AvailablePassengers.Add(newKart.transform.GetChild(2).gameObject);
			
			newKart.transform.GetChild(0).gameObject.SetActive(true);
			newKart.transform.GetChild(1).gameObject.SetActive(true);
			newKart.transform.GetChild(2).gameObject.SetActive(true);

			Tween checker = null;
			checker = DOVirtual.DelayedCall(0.05f, () =>
			{
				if (!newKart.isInitialised) return;
				
				_lastKart.back = newKart.Wagon;
				newKart.Wagon.Setup(_lastKart);
				newKart.KartFollow.charToFollow = _lastKart.transform;

				_lastKart = newKart.Wagon;
				
				checker.Kill();
			}).SetLoops(-1);
		}

		private void Explode(Vector3 collisionPoint)
		{
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(true);

			var direction = transform.position - collisionPoint;
			direction = direction.normalized;

			_my.BoxCollider.enabled = false;
			foreach (var kart in _additionalKarts)
			{
				for (var i = 0; i < 3; i++) 
					kart.transform.GetChild(i).gameObject.SetActive(false);

				kart.explosionKart.gameObject.SetActive(true);
				kart.BoxCollider.enabled = false;
				kart.explosionKart.AddForce(direction * forceMultiplier + Vector3.up * upForce, ForceMode.Impulse);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("PickUpPlatform")) return;
			PickUpThePassengers(other.gameObject);
			other.enabled = false;
		}

		private void OnObstacleCollision(Vector3 collisionPoint) => Explode(collisionPoint);
	}
}