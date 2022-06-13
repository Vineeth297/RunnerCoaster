using DG.Tweening;
using Kart;
using UnityEngine;

public class BonusTile : MonoBehaviour
{
	[SerializeField] private bool isFlipped;
	[HideInInspector] public MeshRenderer meshRenderer;
	private Collider _collider;
	
	private static AdditionalKartManager _rollerCoasterManager;
	private static float _lowestAllowedY = -9999f;

	private void Start()
	{
		_collider = GetComponent<Collider>();
		meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

		if (!_rollerCoasterManager)
			_rollerCoasterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AdditionalKartManager>();
		
		if(_lowestAllowedY < -999f)
			_lowestAllowedY = GameObject.FindGameObjectWithTag("BonusRamp").GetComponent<BonusRamp>().LowestPointY - 1.8f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(2);

		if (_rollerCoasterManager.AvailablePassengers.Count <= 0)
		{
			GameEvents.InvokeRunOutOfPassengers();
			return;
		}
		
		var kartPassenger = _rollerCoasterManager.AvailablePassengers[^1];
		_rollerCoasterManager.AvailablePassengers.Remove(kartPassenger);
		kartPassenger.transform.DORotateQuaternion(myPassengerChild.rotation * Quaternion.AngleAxis(180f, Vector3.up), 0.5f);
		
		kartPassenger.transform.DOJump(myPassengerChild.position,
			kartPassenger.transform.position.y - _lowestAllowedY,
			1,
			1.25f).OnComplete(() =>
		{
			myPassengerChild.gameObject.SetActive(true);
			if(isFlipped)
				myPassengerChild.rotation *= Quaternion.AngleAxis(180f, Vector3.up);
			
			//turn kart off
			//kartPassenger.transform.parent.GetChild(0).gameObject.SetActive(false);
			kartPassenger.SetActive(false);
		});
		_collider.enabled = false;
	}
}
