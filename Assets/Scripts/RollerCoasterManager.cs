using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Dreamteck.Splines;
using Dreamteck.Splines.Examples;
using UnityEngine;

public class RollerCoasterManager : MonoBehaviour
{
	[SerializeField] private GameObject kartPrefab;
	
	[SerializeField] private List<GameObject> additionalKarts;
	[SerializeField] private List<Wagon> wagons;
	[SerializeField] private List<KartFollow> kartFollows;

	public List<GameObject> availablePassengers;
	private Wagon _lastKart;

	private void Start()
	{
		GameManager.Instance.totalAdditionalKarts = additionalKarts.Count;
		_lastKart = GetComponent<Wagon>();
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
		if (other.CompareTag("ObstacleTrain"))
		{
			GameEvents.InvokeObstacleCollision();
			Explode();
		}
	}

	private void PickUpThePassengers(GameObject platform)
	{
		var pickupPlatform = platform.GetComponent<PickupPlatform>();
		var kartSpawnCount = pickupPlatform.passengers.Count / 2 + 1;
		SpawnTheKarts(kartSpawnCount);
		pickupPlatform.JumpOnToTheKart();
	}

	public void SpawnTheKarts(int kartsToSpawn)
	{
		//StartCoroutine(KartSpawnRoutine(kartsToSpawn));

		DOVirtual.DelayedCall(0.15f, SpawnNewKart).SetLoops(kartsToSpawn);
	}

	private void SpawnNewKart()
	{
		var newKart = Instantiate(kartPrefab, transform.parent);

		newKart.transform.GetChild(0).gameObject.SetActive(true);
		newKart.transform.GetChild(1).gameObject.SetActive(true);
		newKart.transform.GetChild(2).gameObject.SetActive(true);
		var wagon = newKart.GetComponent<Wagon>();

		_lastKart.back = wagon;
		wagon.Setup(_lastKart);

		_lastKart = wagon;
	}
	
	private IEnumerator KartSpawnRoutine(int kartsToSpawn)
	{
		for (var i = 0; i < kartsToSpawn; i++)
		{
			additionalKarts[i].SetActive(true);

			additionalKarts[i].transform.GetChild(0).gameObject.SetActive(true);
			yield return new WaitForSeconds(0.15f);
			additionalKarts[i].transform.GetChild(1).gameObject.SetActive(true);
			availablePassengers.Add(additionalKarts[i].transform.GetChild(1).gameObject);
			GameManager.Instance.numberOfActiveKarts++;
			yield return new WaitForSeconds(0.15f);
			additionalKarts[i].transform.GetChild(2).gameObject.SetActive(true);
			availablePassengers.Add(additionalKarts[i].transform.GetChild(2).gameObject);
			GameManager.Instance.numberOfActiveKarts++;
		}
	}

	private void Explode()
	{
		for (var i = 0; i < 7; i++) transform.GetChild(i).gameObject.SetActive(false);

		transform.GetChild(7).gameObject.SetActive(true);

		foreach (var kart in additionalKarts)
		{
			for (var i = 0; i < 3; i++) kart.transform.GetChild(i).gameObject.SetActive(false);
			kart.transform.GetChild(3).gameObject.SetActive(true);

			GameManager.Instance.numberOfActiveKarts--;
		}
	}

	public void DisableTheKarts(int kartsToHide)
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

				additionalKarts[i].SetActive(false);
				additionalKarts[i].transform.GetChild(0).gameObject.SetActive(false);
				yield return new WaitForSeconds(0.15f);
				additionalKarts[i].transform.GetChild(1).gameObject.SetActive(false);
				yield return new WaitForSeconds(0.15f);
				additionalKarts[i].transform.GetChild(2).gameObject.SetActive(false);
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
		var player = GetComponent<PlayerControllerOld>();
		player.ResetMaxSpeed();
		player.speed = 30f;
		foreach (var passenger in availablePassengers)
		{
			passenger.SetActive(false);
			yield return new WaitForSeconds(0.20f);
		}
	}

	public void PreparationsForBonusRamp()
	{
		print("InTheLoop");
		GetComponent<Wagon>().enabled = false;
		GetComponent<TrainEngine>().enabled = false;

		for (var index = 0; index < wagons.Count; index++)
		{
			var wagon = wagons[index];
			wagon.transform.GetComponent<SplinePositioner>().enabled = false;
			wagon.enabled = false;
			kartFollows[index].enabled = true;
		}

		print("end");
		GameEvents.InvokeReachEndOfTrack();
	}

	public void JumpOnToBonusPlatform()
	{
		var count = availablePassengers.Count;
		var kart = additionalKarts[^1].transform;
		availablePassengers.RemoveAt(count - 1);

		kart.gameObject.SetActive(false);
	}
}