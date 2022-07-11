using System;
using DG.Tweening;
using Dreamteck.Splines.Primitives;
using Kart;
using Unity.Mathematics;
using UnityEngine;

public class Collectible_Money : MonoBehaviour
{
	private static MainCanvasController _main;
	private static MainKartController _mainKart;

	[SerializeField] private float travelDuration = 0.5f;

	private void Start()
	{
		if (!_main)
		{
			_main = GameObject.FindWithTag("MainCanvas").GetComponent<MainCanvasController>();
		}

		if (!_mainKart)
		{
			_mainKart = GameObject.FindWithTag("Player").GetComponent<MainKartController>();
		}

		//transform.rotation = Quaternion.AngleAxis(45, Vector3.forward);
		transform.DORotate(Vector3.one * 45f, 1f, RotateMode.FastBeyond360)
			.SetLoops(-1, LoopType.Incremental)
			.SetEase(Ease.Linear);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		
		_main.IncreaseMoneyCount();
		transform.parent = DampCamera.only.transform;
		transform.DOScale(transform.lossyScale * 0.75f, travelDuration);//.SetEase(Ease.OutElastic);
		transform.DOLocalMove(DampCamera.only.MoneyDestination.localPosition, travelDuration).
			OnComplete(()=>
			{
				_main.ScaleMoneyImage();
				gameObject.SetActive(false);
			});
		
	//	gameObject.SetActive(false);
	}
}
