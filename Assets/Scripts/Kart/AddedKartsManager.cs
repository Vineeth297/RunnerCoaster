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

		public bool isObstacleMainKart;
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
				_my.Passenger1, _my.Passenger2
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

				if (isObstacleMainKart || !AudioManager.instance) return;
				
				AudioManager.instance.Play("AddKart", -1f, pitch);
				pitch += 0.1f;
			}).SetLoops(kartsToSpawn);
		}

		public void PopKart()
		{
			var kartToPop = AddedKarts[^1];

			print("kachra, bug");
			kartToPop.transform.GetChild(0).gameObject.SetActive(false);
			kartToPop.transform.GetChild(3).gameObject.SetActive(false);
			kartToPop.kartCollider.transform.GetChild(1).gameObject.SetActive(false);
			kartToPop.kartCollider.transform.GetChild(2).gameObject.SetActive(false);

			kartToPop.gameObject.name += " fallen";
			kartToPop.KartFollow.SetKartToFollow(null);
			kartToPop.kartCollider.gameObject.SetActive(true);

			var direction = -kartToPop.transform.forward + Vector3.up;
			direction = direction.normalized;

			kartToPop.BoxCollider.enabled = false;
			kartToPop.Positioner.enabled = false;

			kartToPop.kartCollider.gameObject.SetActive(true);
			kartToPop.kartCollider.attachedRigidbody.AddForce(direction * forceMultiplier + Vector3.up * upForce +
															  Vector3.left * sideForce, ForceMode.Impulse);

			AddedKarts.RemoveAt(AddedKarts.Count - 1);
			if (AddedKartCount > 0) AddedKarts[^1].Wagon.back = null;
		}

		public void MakePassengersJump(float duration)
		{
			var delay = 0f;
			print(_availablePassengers.Count);

			for (var index = 0; index < _availablePassengers.Count; index++)
			{
				if (index % 2 == 0) delay += passengerJumpDelayStep;

				Debug.Log(_availablePassengers.Count + ", " + index + ", " + _availablePassengers[index], _availablePassengers[index]);
				_availablePassengers[index].MakePassengerJump(duration, delay);
			}
		}

		public void ExplodeMultipleKarts(int number, Vector3 collisionPoint)
		{
			for (var i = 0; i < number; i++) ExplodeRearKart(collisionPoint);
		}

		private void SpawnNewKart()
		{
			var newKart = Instantiate(kartPrefab, transform.parent).GetComponent<AdditionalKartController>();
			
			if(!isObstacleMainKart) DOVirtual.DelayedCall(0.1f, newKart.scaleUp.ScaleMeUp);

			Tween checker = null;
			checker = DOVirtual.DelayedCall(0.05f, () =>
			{
				if (!newKart.isInitialised) return;

				//add new kart passengers
				_availablePassengers.Add(newKart.Passenger1);
				_availablePassengers.Add(newKart.Passenger2);
				print(newKart.Passenger1 + $"Pass1 {_availablePassengers[^1]} {_availablePassengers[^2]}");

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

			kartToPop.gameObject.name += " fallen";
			kartToPop.tag = "Untagged";
			kartToPop.baseCollider.SetActive(false);
			kartToPop.KartFollow.SetKartToFollow(null);
			kartToPop.kartCollider.transform.parent = null;

			var direction = collisionPoint - kartToPop.transform.position;
			direction = direction.normalized;

			var perpendicular = direction;
			perpendicular.x = -direction.z;
			perpendicular.z = direction.x;

			kartToPop.BoxCollider.enabled = false;
			kartToPop.Positioner.enabled = false;

			var directionMultiplier = (Random.value > 0.5f ? 1f : -1f);

			kartToPop.kartCollider.isTrigger = false;
			
			var attachedRigidbody = kartToPop.kartCollider.attachedRigidbody;
			attachedRigidbody.isKinematic = false;
			attachedRigidbody.AddForce(
				direction * forceMultiplier + Vector3.up * upForce  * (explodeMultipleKarts ? 0.5f : 1f) +
				Vector3.left * sideForce * directionMultiplier, ForceMode.Impulse);
			attachedRigidbody.AddTorque(perpendicular * forceMultiplier + Vector3.left * sideForce * directionMultiplier, ForceMode.Impulse);

			RemoveKartFromRear();
			if (AddedKartCount > 0) AddedKarts[^1].Wagon.back = null;

			_availablePassengers.RemoveAt(_availablePassengers.Count - 1);
			_availablePassengers.RemoveAt(_availablePassengers.Count - 1);

			if (AudioManager.instance)
				AudioManager.instance.Play("Death" + ((++_audioIndex % 4) + 1));
		}

		private void ExplodeMainKart(Vector3 collisionPoint)
		{
			var direction = collisionPoint - transform.position;
			direction = direction.normalized;

			var perpendicular = direction;
			perpendicular.x = -direction.z;
			perpendicular.z = direction.x;

			_my.BoxCollider.enabled = false;
			_my.kartCollider.isTrigger = false;

			var attachedRigidbody = _my.kartCollider.attachedRigidbody;
			attachedRigidbody.isKinematic = false;
			attachedRigidbody.AddForce(direction * forceMultiplier + Vector3.up * upForce + Vector3.left * upForce, ForceMode.Impulse);
			attachedRigidbody.AddTorque(perpendicular * forceMultiplier + Vector3.left * upForce, ForceMode.Impulse);

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
			var kartPassenger1 = _my.characterPairsParent.transform.GetChild(0).GetChild(0).transform;
			var kartPassenger2 = _my.characterPairsParent.transform.GetChild(0).GetChild(1).transform;
			pickUpPlatform.JumpOnToTheKart(kartPassenger1,kartPassenger2);
			
			other.enabled = false; 
		}

		private void OnObstacleCollision(Vector3 collisionPoint)
		{
			if(isObstacleMainKart) return;
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

			_my.PlayExplosionParticle(collisionPoint);
			TimeController.only.SlowDownTime();
			DOVirtual.DelayedCall(0.75f, () => TimeController.only.RevertTime());
		}
	}
}