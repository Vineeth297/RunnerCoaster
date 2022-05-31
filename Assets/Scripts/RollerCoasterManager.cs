using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Dreamteck.Splines.Examples;
using UnityEngine;

public class RollerCoasterManager : MonoBehaviour
{
	[SerializeField] private List<GameObject> additionalKarts;
	[SerializeField] private List<Wagon> wagons;

	[SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

	

	private void Start()
	{
	//	cinemachineVirtualCamera.Follow = this.transform;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PickUpPlatform"))
		{
			PickUpThePassengers(other.gameObject);

			other.enabled = false;
			//cinemachineVirtualCamera.Follow = additionalKarts[0].transform;
		}

		if (other.CompareTag("ObstacleTrain"))
		{
			Explode();
		}
	}

	private void PickUpThePassengers(GameObject platform)
	{
		//additionalKarts[0].SetActive(true);
		var pickupPlatform = platform.GetComponent<PickupPlatform>();
		var kartSpawnCount = pickupPlatform.passengers.Count / 2 + 1;
		SpawnTheKarts(kartSpawnCount);
		pickupPlatform.JumpOnToTheKart();
	}

	private void SpawnTheKarts(int cartsToSpawn)
	{
		StartCoroutine(KartSpawnRoutine(cartsToSpawn));
	}

	private IEnumerator KartSpawnRoutine(int cartsToSpawn)
	{
		//GetComponent<Wagon>().back = wagons[0];
		for (int i = 0; i < cartsToSpawn; i++)
		{
			additionalKarts[i].SetActive(true);
			/*if(i != cartsToSpawn - 1)
				additionalKarts[i].GetComponent<Wagon>().back = wagons[i + 1];*/		
			additionalKarts[i].transform.GetChild(0).gameObject.SetActive(true);
			yield return new WaitForSeconds(0.15f);
			additionalKarts[i].transform.GetChild(1).gameObject.SetActive(true);
			yield return new WaitForSeconds(0.15f);
			additionalKarts[i].transform.GetChild(2).gameObject.SetActive(true);
		}
	}
	
	private void Explode()
	{
		transform.GetChild(3).gameObject.SetActive(true);
		
		foreach (var kart in additionalKarts)
		{
			for (var i = 0; i < 3; i++)
			{
				kart.transform.GetChild(i).gameObject.SetActive(false);
			}
			kart.transform.GetChild(3).gameObject.SetActive(true);
		}
	}
}
