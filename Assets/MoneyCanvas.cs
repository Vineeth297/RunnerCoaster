using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyCanvas : MonoBehaviour
{
	[SerializeField] public TextMeshProUGUI moneyText;
	[SerializeField] private GameObject moneyImage;
	[SerializeField] private Transform moneyDestination;

	private Tweener _moneyTween;
	private int _moneyCount, _moneyMultiplier;

	private void Start()
	{
		_moneyCount = ShopStateController.CurrentState.GetState().CoinCount;
		moneyText.text = _moneyCount.ToString();
		GetComponent<Canvas>().worldCamera = Camera.main;
	}

	public void IncreaseMoneyCount()
	{
		_moneyCount += 1 * _moneyMultiplier;
		ShopStateController.CurrentState.GetState().CoinCount = _moneyCount;

		moneyText.text = _moneyCount.ToString();
	}

	public void UpdateMultiplier(int multiplier) => _moneyMultiplier = 1 * multiplier;

	public void ScaleMoneyImage()
	{
		if (_moneyTween.IsActive()) _moneyTween.Kill(true);
		
		_moneyTween = moneyImage.transform.DOScale(Vector3.one * 1.15f, 0.1f)
			.SetLoops(2,LoopType.Yoyo);
	}

	public Transform GetMoneyDestination() => moneyDestination;
}
