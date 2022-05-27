using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickupPlatform : MonoBehaviour
{
	public List<GameObject> passengers;

	public void JumpOnToTheKart()
	{
		StartCoroutine(JumpingRoutine());
	}

	private IEnumerator JumpingRoutine()
	{
		foreach (var passenger in passengers)
		{
			passenger.transform.DOJump(transform.position, 3f,1, 0.5f)
				.OnComplete(()=>passenger.SetActive(false));
			
			yield return new WaitForSeconds(0.15f);
		}
	}
}
