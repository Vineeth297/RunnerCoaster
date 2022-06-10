using DG.Tweening;
using Kart;
using UnityEngine;

public class BonusTile : MonoBehaviour
{
	[SerializeField] private AdditionalKartManager rollerCoasterManager;
	private Collider _collider;

	private void Start()
	{
		_collider = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(1);

		if (rollerCoasterManager.GetAvailablePassengers().Count <= 0)
		{
			GameEvents.InvokeStopOnBonusRamp();
			return;
		}
		
		var kartPassenger = rollerCoasterManager.GetAvailablePassengers()[^1];
		rollerCoasterManager.GetAvailablePassengers().Remove(kartPassenger);
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
