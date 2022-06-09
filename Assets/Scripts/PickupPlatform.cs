using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickupPlatform : MonoBehaviour
{
	public List<GameObject> passengers;

	[SerializeField] private Transform[] jumpPositions;

	public void JumpOnToTheKart()
	{
		StartCoroutine(JumpingRoutine());
	}

	private IEnumerator JumpingRoutine()
	{
		foreach (var passenger in passengers)
		{
			var randomJumpPos = Random.Range(0, 2);
			print(randomJumpPos);
			passenger.transform.DOJump(jumpPositions[randomJumpPos].position, 3f, 1, 0.5f)
				.OnComplete(() => passenger.SetActive(false));

			yield return new WaitForSeconds(0.1f);
		}
	}
}