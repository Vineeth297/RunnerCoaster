using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Dreamteck.Splines.Examples;
using UnityEngine;

public class RollerCoasterManager : MonoBehaviour
{
	private GameManager _gameManager;
	
	[SerializeField] private List<GameObject> additionalKarts;
	[SerializeField] private List<Wagon> wagons;

	[SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;


	private void Start()
	{
	//	cinemachineVirtualCamera.Follow = this.transform;
		_gameManager = GameManager.Instance;
		_gameManager.totalAdditionalKarts = additionalKarts.Count;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PickUpPlatform"))
		{
			PickUpThePassengers(other.gameObject);

			other.enabled = false;
			//cinemachineVirtualCamera.Follow = additionalKarts[0].transform;
		}

		if (other.CompareTag("ObstacleTrain")) Explode();
	}

	private void PickUpThePassengers(GameObject platform)
	{
		//additionalKarts[0].SetActive(true);
		var pickupPlatform = platform.GetComponent<PickupPlatform>();
		var kartSpawnCount = pickupPlatform.passengers.Count / 2 + 1;
		SpawnTheKarts(kartSpawnCount);
		pickupPlatform.JumpOnToTheKart();
	}

	public void SpawnTheKarts(int kartsToSpawn) => StartCoroutine(KartSpawnRoutine(kartsToSpawn));

	private IEnumerator KartSpawnRoutine(int kartsToSpawn)
	{
		for (var i = 0; i < kartsToSpawn; i++)
		{
			_gameManager.numberOfActiveKarts++;
			//_gameManager.MoveCameraBack();
			
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
			
			_gameManager.numberOfActiveKarts--;
		}
	}
	
	public void DisableTheKarts(int kartsToHide) => StartCoroutine(KartDisableRoutine(kartsToHide));

	private IEnumerator KartDisableRoutine(int kartsToHide)
	{
		if (kartsToHide > _gameManager.numberOfActiveKarts)
		{
			print("Level Fail");
		}
		else
		{
			for (var i = 0; i < kartsToHide; i++)
			{
				_gameManager.numberOfActiveKarts--;
				_gameManager.MoveCameraFront();
				
				additionalKarts[i].SetActive(false);
				additionalKarts[i].transform.GetChild(0).gameObject.SetActive(false);
				yield return new WaitForSeconds(0.15f);
				additionalKarts[i].transform.GetChild(1).gameObject.SetActive(false);
				yield return new WaitForSeconds(0.15f);
				additionalKarts[i].transform.GetChild(2).gameObject.SetActive(false);
			}	
		}
	}
}
