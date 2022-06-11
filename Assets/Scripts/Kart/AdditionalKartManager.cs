using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class AdditionalKartManager : MonoBehaviour
	{
		[SerializeField] private GameObject kartPrefab;
		[SerializeField] private float forceMultiplier, upForce;

		private List<GameObject> _availablePassengers;
		private List<AdditionalKartRefBank> _additionalKarts;

		private Wagon _lastKart;
		private MainKartRefBank _my;

		private void OnEnable()
		{
			GameEvents.ObstacleCollision += OnObstacleCollision;
		}

		private void OnDisable()
		{
			GameEvents.ObstacleCollision -= OnObstacleCollision;
		}

		private void Start()
		{
			_lastKart = GetComponent<Wagon>();
			_my = GetComponent<MainKartRefBank>();

			_additionalKarts = new List<AdditionalKartRefBank>();
			_availablePassengers = new List<GameObject>();
			
			//add main kart passengers
			_availablePassengers.Add(transform.GetChild(1).gameObject);
			_availablePassengers.Add(transform.GetChild(2).gameObject);
		}

		private void PickUpThePassengers(GameObject platform)
		{
			var pickupPlatform = platform.GetComponent<PickupPlatform>();
			var kartSpawnCount = pickupPlatform.passengers.Count / 2 + 1;
			SpawnKarts(kartSpawnCount);
			pickupPlatform.JumpOnToTheKart();
		}

		public List<GameObject> GetAvailablePassengers() => _availablePassengers;
		
		public void SpawnKarts(int kartsToSpawn) => DOVirtual.DelayedCall(0.15f, SpawnNewKart).SetLoops(kartsToSpawn);

		private void SpawnNewKart()
		{
			var newKart = Instantiate(kartPrefab, transform.parent).GetComponent<AdditionalKartRefBank>();
			_additionalKarts.Add(newKart);
			
			//add new kart passengers
			_availablePassengers.Add(newKart.transform.GetChild(1).gameObject);
			_availablePassengers.Add(newKart.transform.GetChild(2).gameObject);
			
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
				GameManager.Instance.numberOfActiveKarts++;
				
				checker.Kill();
			}).SetLoops(-1);
		}

		public void HideKarts(int kartsToHide)
		{
			StartCoroutine(KartDisableRoutine(kartsToHide));
		}

		private IEnumerator KartDisableRoutine(int kartsToHide)
		{
			if (kartsToHide > GameManager.Instance.numberOfActiveKarts)
				print("Level Fail");
			else
				for (var i = 0; i < kartsToHide; i++)
				{
					GameManager.Instance.numberOfActiveKarts--;

					_additionalKarts[i].gameObject.SetActive(false);
					_additionalKarts[i].transform.GetChild(0).gameObject.SetActive(false);
					yield return new WaitForSeconds(0.15f);
					_additionalKarts[i].transform.GetChild(1).gameObject.SetActive(false);
					yield return new WaitForSeconds(0.15f);
					_additionalKarts[i].transform.GetChild(2).gameObject.SetActive(false);
				}
		}

		//Disable All The Karts
		//Enable The passengers on the bonus Ramp one by one simultaneously by disabling the the passengers in the kart
		private void DisablePassengersForBonusRamp()
		{
			StartCoroutine(BonusRampPassengersDisablingRoutine());
		}

		private IEnumerator BonusRampPassengersDisablingRoutine()
		{
			var player = GetComponent<KartTrackMovement>();
			player.SetNormalSpeedValues();
			player.currentSpeed = 30f;
			foreach (var passenger in _availablePassengers)
			{
				passenger.SetActive(false);
				yield return new WaitForSeconds(0.20f);
			}
		}

		public void PreparationsForBonusRamp()
		{
			GetComponent<Wagon>().enabled = false;
			GetComponent<TrainEngine>().enabled = false;

			foreach (var kart in _additionalKarts)
			{
				kart.Positioner.enabled = false;
				kart.Wagon.enabled = false;
				kart.KartFollow.enabled = true;
			}

			print("end");
			GameEvents.InvokeReachEndOfTrack();
		}

		public void JumpOnToBonusPlatform()
		{
			var count = _availablePassengers.Count;
			var kart = _additionalKarts[^1].transform;
			_availablePassengers.RemoveAt(count - 1);

			kart.gameObject.SetActive(false);
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
				GameManager.Instance.numberOfActiveKarts--;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("PickUpPlatform"))
			{
				PickUpThePassengers(other.gameObject);
				other.enabled = false;
			}
			/*if (other.CompareTag("BonusRamp"))
			{
				DisablePassengersForBonusRamp();
				other.enabled = false;
			}*/
		}

		private void OnObstacleCollision(Vector3 collisionPoint) => Explode(collisionPoint);
	}
}