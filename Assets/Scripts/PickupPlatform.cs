using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickupPlatform : MonoBehaviour
{
	[SerializeField] private List<Transform> passengers;
	[SerializeField] private float jumpHeight;

	public void JumpOnToTheKart(Transform kartPassenger1,Transform kartPassenger2)
	{
		var temp = 0f;
		
		DOTween.To(() => temp, value => temp = value, 1f, 0.5f).SetEase(Ease.InOutCubic)
			.OnUpdate(() =>
			{
				passengers[0].transform.position = Vector3.Lerp(passengers[0].position, kartPassenger1.position, temp);
				passengers[1].transform.position = Vector3.Lerp(passengers[1].position, kartPassenger2.position, temp);
			});

		passengers[0].DOMoveY(passengers[0].position.y + jumpHeight, 0.25f).SetEase(Ease.OutExpo)
			.OnComplete(() => passengers[0].DOMoveY(kartPassenger1.position.y, 0.25f).SetEase(Ease.InExpo)
				.OnComplete(() =>
				{
					kartPassenger1.gameObject.SetActive(true);
					passengers[0].gameObject.SetActive(false);
				}));
		
		passengers[1].DOMoveY(passengers[0].position.y + jumpHeight, 0.25f).SetEase(Ease.OutExpo)
			.OnComplete(() => passengers[0].DOMoveY(kartPassenger2.position.y, 0.25f).SetEase(Ease.InExpo)
				.OnComplete(() =>
				{
					kartPassenger2.gameObject.SetActive(true);
					passengers[1].gameObject.SetActive(false);
				}));
	}
}