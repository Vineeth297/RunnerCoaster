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

		private Wagon _lastKart;
		private MainKartController _my;

		public static bool IsInKartCollisionCooldown; 
		private static int _audioIndex;
		
		private Tween _kartCollisionCooldown;

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

			AddedKarts = new List<AdditionalKartController>();
			_availablePassengers = new List<Passenger>
			{
				//add main kart passengers
				_my.passenger1, _my.passenger2
			};
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

		public void MakePassengersJump(float duration)
		{
			var delay = 0f;
			
			for (var index = 0; index < _availablePassengers.Count; index++)
			{
				if (index % 2 == 0) delay += passengerJumpDelayStep;
				
				_availablePassengers[index].MakePassengerJump(duration, delay);
			}
		}

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

		private void ExplodeRearKart(Vector3 collisionPoint)
		{
			var kartToPop = AddedKarts[^1];

			for (var i = 0; i < 4; i++) 
				kartToPop.transform.GetChild(i).gameObject.SetActive(false);

			kartToPop.gameObject.name += " fallen";
			kartToPop.explosionKart.gameObject.SetActive(true);

			var direction = collisionPoint - kartToPop.transform.position;
			direction = direction.normalized;

			var perpendicular = direction;
			perpendicular.x = -direction.z;
			perpendicular.z = direction.x;

			kartToPop.BoxCollider.enabled = false;
			kartToPop.Positioner.enabled = false;

			kartToPop.explosionKart.gameObject.SetActive(true);
			kartToPop.explosionKart.AddForce(direction * forceMultiplier + Vector3.up * upForce + Vector3.left * sideForce, ForceMode.Impulse);
			kartToPop.explosionKart.AddTorque(perpendicular * forceMultiplier + Vector3.left * sideForce, ForceMode.Impulse);

			AddedKarts.RemoveAt(AddedKarts.Count - 1);
			AddedKarts[^1].Wagon.back = null;

			_availablePassengers.RemoveAt(_availablePassengers.Count - 1);
			_availablePassengers.RemoveAt(_availablePassengers.Count - 1);

			if(AudioManager.instance)
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

			if(AudioManager.instance) AudioManager.instance.Play("Death" + ((++_audioIndex % 4) + 1));
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
			if(IsInKartCollisionCooldown) return;

			IsInKartCollisionCooldown = true;
			_kartCollisionCooldown = DOVirtual.DelayedCall(0.1f, () => IsInKartCollisionCooldown = false);
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