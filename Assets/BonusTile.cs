using DG.Tweening;
using Kart;
using UnityEngine;

public class BonusTile : MonoBehaviour
{
	[SerializeField] private Transform leftFlag, rightFlag;
	[SerializeField] private bool isFlipped;
	[HideInInspector] public MeshRenderer meshRenderer;
	private bool _hasEntered, _hasExited;

	private static AdditionalKartManager _rollerCoasterManager;
	private static float _lowestAllowedY = -9999f;

	private void Start()
	{
		meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

		if (!_rollerCoasterManager)
			_rollerCoasterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<AdditionalKartManager>();
		
		if(_lowestAllowedY < -999f)
			_lowestAllowedY = GameObject.FindGameObjectWithTag("BonusRamp").GetComponent<BonusRamp>().LowestPointY - 1.8f;
	}

	private void EjectPassenger(Transform myPassengerChild)
	{
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
			kartPassenger.SetActive(false);
		});
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_hasEntered) return;
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(2);

		_hasEntered = true;
		
		leftFlag.DOLocalRotate(Vector3.up * -90f, 0.5f).SetEase(Ease.OutElastic);
		//leftFlag.DOPunchScale(leftFlag.localScale * 1.25f, 0.25f);
		rightFlag.DOLocalRotate(Vector3.up * 90f, 0.5f).SetEase(Ease.OutElastic);
		//rightFlag.DOPunchScale(rightFlag.localScale * 1.25f, 0.25f);
		
		EjectPassenger(myPassengerChild);
	}

	private void OnTriggerExit(Collider other)
	{
		if(_hasExited) return;
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(3);

		_hasExited = true;

		EjectPassenger(myPassengerChild);
	}
}
