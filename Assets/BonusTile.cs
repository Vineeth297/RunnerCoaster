using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BonusTile : MonoBehaviour
{
	[SerializeField] private RollerCoasterManager rollerCoasterManager;
	private Collider _collider;

	private void Start()
	{
		_collider = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(1);

		if (rollerCoasterManager.availablePassengers.Count <= 0)
		{
			GameEvents.InvokeStopTheRollerCoaster();
			return;
		}
		
		var kartPassenger = rollerCoasterManager.availablePassengers[^1];
		rollerCoasterManager.availablePassengers.Remove(kartPassenger);
		kartPassenger.transform.DOJump(myPassengerChild.position,
			5f,
			1,
			0.5f).OnComplete(() =>
		{
			myPassengerChild.gameObject.SetActive(true);
			//rollerCoasterManager.JumpOnToBonusPlatform();
			kartPassenger.transform.parent.GetChild(0).gameObject.SetActive(false);
			kartPassenger.SetActive(false);
		});
		_collider.enabled = false;
	}
}
