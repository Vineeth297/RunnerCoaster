using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class AddedKartsManager : MonoBehaviour
	{
		[SerializeField] private GameObject kartPrefab;
		[SerializeField] private float forceMultiplier, upForce, sideForce;
		[SerializeField] private float passengerJumpDelayStep;

		private List<AdditionalKartController> AddedKarts { get; set; }

		private List<Passenger> _availablePassengers;
		private MainKartController _my;

		private static bool _isInKartCollisionCooldown;
		private static int _audioIndex;

		public int AddedKartCount => AddedKarts.Count;
		public int PassengerCount => _availablePassengers.Count;

		[SerializeField] private bool isObstacleMainKart;
		[SerializeField] private GameObject additionalObstacleKartPrefab;
		[SerializeField] private int totalObstacleAdditionalKarts = 5;
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
			GameEvents.MainKartCrash += OnObstacleCollision;
		}

		private void OnDisable()
		{
			GameEvents.MainKartCrash -= OnObstacleCollision;
		}

		private void Start()
		{
			_my = GetComponent<MainKartController>();

			AddedKarts = new List<AdditionalKartController>();
			_availablePassengers = new List<Passenger>
			{
				//add main kart passengers
				_my.passenger1, _my.passenger2
			};

			
			if (isObstacleMainKart)
			{
				kartPrefab = additionalObstacleKartPrefab;
				SpawnKarts(totalObstacleAdditionalKarts);
			}
		}

		public void SpawnKarts(int kartsToSpawn)
		{
			var pitch = 0.9f;
			DOVirtual.DelayedCall(0.15f, () =>
			{
				SpawnNewKart();

				if (AudioManager.instance)
					AudioManager.instance.Play("AddKart", -1f, pitch);

				pitch += 0.1f;
			}).SetLoops(kartsToSpawn);
		}

		public void PopKart()
		{
			var kartToPop = AddedKarts[^1];

			kartToPop.transform.GetChild(0).gameObject.SetActive(false);
			kartToPop.transform.GetChild(3).gameObject.SetActive(false);
			kartToPop.explosionKart.transform.GetChild(1).gameObject.SetActive(false);
			kartToPop.explosionKart.transform.GetChild(2).gameObject.SetActive(false);

			kartToPop.gameObject.name += " fallen";
			kartToPop.KartFollow.SetKartToFollow(null);
			kartToPop.explosionKart.gameObject.SetActive(true);

			var direction = -kartToPop.transform.forward + Vector3.up;
			direction = direction.normalized;

			kartToPop.BoxCollider.enabled = false;
			kartToPop.Positioner.enabled = false;

			kartToPop.explosionKart.gameObject.SetActive(true);
			kartToPop.explosionKart.AddForce((direction * forceMultiplier + Vector3.up * upForce) + Vector3.left * sideForce, ForceMode.Impulse);

			AddedKarts.RemoveAt(AddedKarts.Count - 1);
			if (AddedKartCount > 0) AddedKarts[^1].Wagon.back = null;
		}

		public void MakePassengersJump(float duration)
		{
			var delay = 0f;

			for (var index = 0; index < _availablePassengers.Count; index++)
			{
				if (index % 2 == 0) delay += passengerJumpDelayStep;

				_availablePassengers[index].MakePassengerJump(duration, delay);
			}
		}

		public void ExplodeMultipleKarts(int number, Vector3 collisionPoint)
		{
			for (var i = 0; i < number; i++) ExplodeRearKart(collisionPoint, true);
		}

		private void SpawnNewKart()
		{
			var newKart = Instantiate(kartPrefab, transform.parent).GetComponent<AdditionalKartController>();

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

				if (AddedKarts.Count == 0)
				{
					_my.Wagon.back = newKart.Wagon;
					newKart.Wagon.Setup(_my.Wagon);
					newKart.KartFollow.SetKartToFollow(transform);
				}
				else
				{
					AddedKarts[^1].Wagon.back = newKart.Wagon;
					newKart.Wagon.Setup(AddedKarts[^1].Wagon);
					newKart.KartFollow.SetKartToFollow(AddedKarts[^1].Wagon.transform);
				}

				AddNewKart(newKart);
				checker.Kill();
			}).SetLoops(-1);
		}

		private void ExplodeRearKart(Vector3 collisionPoint, bool explodeMultipleKarts = false)
		{
			var kartToPop = AddedKarts[^1];

			for (var i = 0; i < 4; i++)
				kartToPop.transform.GetChild(i).gameObject.SetActive(false);

			kartToPop.gameObject.name += " fallen";
			kartToPop.tag = "Untagged";
			kartToPop.KartFollow.SetKartToFollow(null);
			kartToPop.explosionKart.gameObject.SetActive(true);
			kartToPop.explosionKart.transform.parent = null;

			var direction = collisionPoint - kartToPop.transform.position;
			direction = direction.normalized;

			var perpendicular = direction;
			perpendicular.x = -direction.z;
			perpendicular.z = direction.x;

			kartToPop.BoxCollider.enabled = false;
			kartToPop.Positioner.enabled = false;

			var directionMultiplier = (Random.value > 0.5f ? 1f : -1f);

			kartToPop.explosionKart.AddForce(
				direction * forceMultiplier + Vector3.up * upForce * (explodeMultipleKarts ? 0.5f : 1f) +
				Vector3.left * sideForce * directionMultiplier, ForceMode.Impulse);
			kartToPop.explosionKart.AddTorque(perpendicular * forceMultiplier + Vector3.left * sideForce * directionMultiplier, ForceMode.Impulse);

			RemoveKartFromRear();
			if (AddedKartCount > 0) AddedKarts[^1].Wagon.back = null;

			_availablePassengers.RemoveAt(_availablePassengers.Count - 1);
			_availablePassengers.RemoveAt(_availablePassengers.Count - 1);

			if (AudioManager.instance)
				AudioManager.instance.Play("Death" + ((++_audioIndex % 4) + 1));
		}

		private void ExplodeMainKart(Vector3 collisionPoint)
		{
			//default main cart disable
			transform.GetChild(0).gameObject.SetActive(false);

			var direction = collisionPoint - transform.position;
			direction = direction.normalized;

			var perpendicular = direction;
			perpendicular.x = -direction.z;
			perpendicular.z = direction.x;

			_my.BoxCollider.enabled = false;

			_my.ExplosionKart.gameObject.SetActive(true);
			_my.ExplosionKart.AddForce(direction * forceMultiplier + Vector3.up * upForce + Vector3.left * upForce, ForceMode.Impulse);
			_my.ExplosionKart.AddTorque(perpendicular * forceMultiplier + Vector3.left * upForce, ForceMode.Impulse);

			if (AudioManager.instance) AudioManager.instance.Play("Death" + ((++_audioIndex % 4) + 1));
		}

		private void AddNewKart(AdditionalKartController newKart)
		{
			AddedKarts.Add(newKart);
			_my.KartCounter.UpdateText(AddedKarts.Count + 1);
		}

		private void RemoveKartFromRear()
		{
			AddedKarts.RemoveAt(AddedKarts.Count - 1);
			_my.KartCounter.UpdateText(AddedKarts.Count + 1);
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

		private void OnObstacleCollision(Vector3 collisionPoint)
		{
			//Explode(collisionPoint);
			if(_isInKartCollisionCooldown) return;

			_isInKartCollisionCooldown = true;
			DOVirtual.DelayedCall(0.1f, () => _isInKartCollisionCooldown = false);
			CameraFxController.only.ScreenShake(5f);

			if (AddedKarts.Count > 0)
				ExplodeRearKart(collisionPoint);
			else
			{
				ExplodeMainKart(collisionPoint);
				GameEvents.InvokePlayerDeath();
			}

			TimeController.only.SlowDownTime();
			DOVirtual.DelayedCall(0.75f, () => TimeController.only.RevertTime());
		}
	}
}