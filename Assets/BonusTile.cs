using DG.Tweening;
using Kart;
using UnityEngine;

public class BonusTile : MonoBehaviour
{
	[HideInInspector] public MeshRenderer meshRenderer;
	private Collider _collider;
	
	private static AdditionalKartManager _rollerCoasterManager;

	private void Start()
	{
		_collider = GetComponent<Collider>();
		meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

		if (!_rollerCoasterManager)
			_rollerCoasterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AdditionalKartManager>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(2);

		if (_rollerCoasterManager.AvailablePassengers.Count <= 0)
		{
			GameEvents.InvokeStopOnBonusRamp();
			return;
		}
		
		var kartPassenger = _rollerCoasterManager.AvailablePassengers[^1];
		_rollerCoasterManager.AvailablePassengers.Remove(kartPassenger);
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
