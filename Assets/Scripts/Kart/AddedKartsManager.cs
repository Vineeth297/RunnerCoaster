using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class AddedKartsManager : MonoBehaviour
	{
		[SerializeField] private GameObject kartPrefab;
		[SerializeField] private float forceMultiplier, upForce;
		[SerializeField] private float passengerJumpDelayStep;

		private List<AdditionalKartController> AddedKarts { get; set; }

		private List<Passenger> _availablePassengers;

		private Wagon _lastKart;
		private MainKartController _my;

		public int PassengerCount => _availablePassengers.Count;

		public GameObject PopPassenger
		{
			get
			{
				var x = _availablePassengers[^1];
				_availablePassengers.RemoveAt(_availablePassengers.Count - 1);
				return x.gameObject;
			}
		}

		private void OnEnable()
		{
			GameEvents.PassengerJump += OnPassengerJump;
			GameEvents.KartCrash += OnObstacleCollision;
		}

		private void OnDisable()
		{
			GameEvents.PassengerJump -= OnPassengerJump;
			GameEvents.KartCrash -= OnObstacleCollision;
		}

		private void Start()
		{
			_lastKart = GetComponent<Wagon>();
			_my = GetComponent<MainKartController>();

			AddedKarts = new List<AdditionalKartController>();
			_availablePassengers = new List<Passenger>
			{
				//add main kart passengers
				_my.passenger1, _my.passenger2
			};
		}

		public void SpawnKarts(int kartsToSpawn) => DOVirtual.DelayedCall(0.15f, SpawnNewKart).SetLoops(kartsToSpawn);

		private void SpawnNewKart()
		{
			var newKart = Instantiate(kartPrefab, transform.parent).GetComponent<AdditionalKartController>();
			AddedKarts.Add(newKart);
			
			//add new kart passengers
			_availablePassengers.Add(newKart.passenger1);
			_availablePassengers.Add(newKart.passenger2);
			
			newKart.transform.GetChild(0).gameObject.SetActive(true);
			newKart.transform.GetChild(1).gameObject.SetActive(true);
			newKart.transform.GetChild(2).gameObject.SetActive(true);

			Tween checker = null;
			checker = DOVirtual.DelayedCall(0.05f, () =>
			{
				if (!newKart.isInitialised) return;
				
				_lastKart.back = newKart.Wagon;
				newKart.Wagon.Setup(_lastKart);
				newKart.KartFollow.SetKartToFollow(_lastKart.transform);

				_lastKart = newKart.Wagon;
				
				checker.Kill();
			}).SetLoops(-1);
		}

		private void Explode(Vector3 collisionPoint)
		{
			//default main cart disable
			transform.GetChild(0).gameObject.SetActive(false);

			var direction = transform.position - collisionPoint;
			direction = direction.normalized;

			_my.BoxCollider.enabled = false;

			_my.ExplosionKart.gameObject.SetActive(true);
			_my.ExplosionKart.AddForce(direction * forceMultiplier + Vector3.up * upForce, ForceMode.Impulse);
			foreach (var kart in AddedKarts)
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
			
			var pickUpPlatform = other.GetComponent<PickupPlatform>();
			var kartPassenger1 = transform.GetChild(0).transform.GetChild(5);
			var kartPassenger2 = transform.GetChild(0).transform.GetChild(6);
			pickUpPlatform.JumpOnToTheKart(kartPassenger1,kartPassenger2);
			
			other.enabled = false; 
		}

		private void OnPassengerJump()
		{
			var delay = -passengerJumpDelayStep;
			
			for (var index = 0; index < _availablePassengers.Count; index++)
			{
				if (index % 2 == 0) delay += passengerJumpDelayStep;
				_availablePassengers[index].MakePassengerJump(delay);
			}
		}

		private void OnObstacleCollision(Vector3 collisionPoint) => Explode(collisionPoint);
	}
}