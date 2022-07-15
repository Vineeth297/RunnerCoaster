using System.Collections.Generic;
using DG.Tweening;
using Kart;
using UnityEngine;

public class Collectible_Money : MonoBehaviour
{
	[SerializeField] private float travelDuration = 0.5f;
	
	private static List<Transform> _units = new List<Transform>();
	private static MoneyCanvas _moneyCanvas;
	private static MainKartController _mainKart;
	
	private static bool _isFirstSelected;
	private bool _isFirst;
	
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

		if (!_isFirstSelected)
		{
			_isFirst = _isFirstSelected = true;
			transform.DORotate(Vector3.one * 45f, 1f, RotateMode.FastBeyond360)
				.SetLoops(-1, LoopType.Incremental)
				.SetEase(Ease.Linear)
				.OnUpdate(() => UpdateRotations(transform));
		}
		else
			_units.Add(transform);
	}

	private void UpdateRotations(Transform rotation)
	{
		foreach (var unit in _units) unit.rotation = rotation.rotation;
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
				if(!_isFirst)
					gameObject.SetActive(false);
			});
		AudioManager.instance.Play("MoneyCollection" );
	//	gameObject.SetActive(false);
	}
}