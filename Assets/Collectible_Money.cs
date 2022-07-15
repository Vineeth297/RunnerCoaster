using System;
using DG.Tweening;
using Dreamteck;
using Dreamteck.Splines.Primitives;
using Kart;
using Unity.Mathematics;
using UnityEngine;

public class Collectible_Money : MonoBehaviour
{
	private static MoneyCanvas _moneyCanvas;
	private static MainKartController _mainKart;

	[SerializeField] private float travelDuration = 0.5f;

	private void Start()
	{
		if (!_moneyCanvas)
		{
			_moneyCanvas = GameObject.FindWithTag("MoneyCanvas").GetComponent<MoneyCanvas>();
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
		
		_moneyCanvas.IncreaseMoneyCount();
		transform.parent = _moneyCanvas.GetMoneyDestination();
		transform.DOScale(Vector3.zero, travelDuration).SetEase(Ease.InCirc);

		transform.DOLocalMove(Vector3.zero, travelDuration).
			OnComplete(()=>
			{
				_moneyCanvas.ScaleMoneyImage();
				gameObject.SetActive(false);
			});
		AudioManager.instance.Play("MoneyCollection" );
	//	gameObject.SetActive(false);
	}
}