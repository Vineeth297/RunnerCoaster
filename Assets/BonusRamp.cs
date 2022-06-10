using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kart;
using UnityEngine;

public class BonusRamp : MonoBehaviour
{
	[SerializeField] private List<GameObject> bonusRampPassengers;
	private MainKartRefBank _mainKart;

	private void Start()
	{
		_mainKart = GameObject.FindGameObjectWithTag("Player").GetComponent<MainKartRefBank>();
	}

	private void OnTriggerEnter(Collider other)
	{
		print("Here");
		if(other.CompareTag("Player"))
			DisablePassengersForBonusRamp();
	}

	private void DisablePassengersForBonusRamp()
	{
		StartCoroutine(BonusRampPassengersDisablingRoutine(bonusRampPassengers));
	}
	
	private IEnumerator BonusRampPassengersDisablingRoutine(List<GameObject> bonusRampPassengers)
	{
		var i = 0;
		_mainKart.TrackMovement.SetNormalSpeedValues();
		_mainKart.TrackMovement.currentSpeed = 10f;
		foreach (var passenger in _mainKart.AdditionalKartManager.GetAvailablePassengers())
		{
			passenger.transform.DOJump(bonusRampPassengers[i].transform.position, 5f,1,0.5f).
				OnComplete(() => passenger.SetActive(false));
			bonusRampPassengers[i++].SetActive(true);
			yield return new WaitForSeconds(0.20f);
		}

		GameEvents.InvokeBonusCameraPushBack();
	}
}
