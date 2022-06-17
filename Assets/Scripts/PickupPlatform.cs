using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickupPlatform : MonoBehaviour
{
	public List<GameObject> passengers;
	
	[SerializeField] private Transform[] jumpPositions;

	public void JumpOnToTheKart(GameObject kartPassenger1,GameObject kartPassenger2)
	{
		passengers[0].transform.DOJump(jumpPositions[0].position, 3f, 1, 0.5f).OnComplete(() =>
		{
			kartPassenger1.SetActive(true);
			passengers[0].SetActive(false);
		});
		passengers[1].transform.DOJump(jumpPositions[1].position, 3f, 1, 0.5f).OnComplete(() =>
		{
			kartPassenger2.SetActive(true);
			passengers[1].SetActive(false);
		});
	}
}