using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BonusRamp : MonoBehaviour
{
	public RollerCoasterManager rollerCoasterManager;
	
	[SerializeField] private List<GameObject> bonusRampPassengers;

	public PlayerControllerOld player;

	private void OnTriggerEnter(Collider other)
	{
		print("Here");
		if(other.CompareTag("Player"))
			DisablePassengersForBonusRamp();
		
	}

	private void DisablePassengersForBonusRamp()
	{
		StartCoroutine(BonusRampPassengersDisablingRoutine());
	}

	private IEnumerator BonusRampPassengersDisablingRoutine()
	{
		var i = 0;
		player.minSpeed = 10f;
		player.maxSpeed = 40f;
		player.speed = 10f;
		foreach (var passenger in rollerCoasterManager.availablePassengers)
		{
			passenger.transform.DOJump(bonusRampPassengers[i].transform.position, 5f,1,0.5f).
				OnComplete(()=>passenger.SetActive(false));
			bonusRampPassengers[i].SetActive(true);
			i++;
			yield return new WaitForSeconds(0.20f);
		}

		GameEvents.InvokeBonusCameraPushBack();
	}
	
}
