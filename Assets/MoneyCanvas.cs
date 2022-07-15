using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyCanvas : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI moneyText;
	[SerializeField] private GameObject moneyImage;
	[SerializeField] private Transform moneyDestination;
	
	private Tweener _moneyTween;
	private int _moneyCount;

	public void IncreaseMoneyCount()
	{
		_moneyCount += 1;
		moneyText.text = _moneyCount.ToString();
	}

	public void ScaleMoneyImage()
	{
		if (_moneyTween.IsActive()) _moneyTween.Kill(true);
		
		_moneyTween = moneyImage.transform.DOScale(Vector3.one * 1.15f, 0.1f)
			.SetLoops(2,LoopType.Yoyo);
	}

	public Transform GetMoneyDestination() => moneyDestination;

}
