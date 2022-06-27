using DG.Tweening;
using Kart;
using UnityEngine;

public class BonusTile : MonoBehaviour
{
	[SerializeField] private Transform leftFlag, rightFlag;
	[HideInInspector] public MeshRenderer meshRenderer;
	private bool _hasEntered, _hasExited;

	private static AddedKartsManager _addedKarts;
	private static float _lowestAllowedY = -9999f;

	private void Start()
	{
		meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();

		if (!_addedKarts)
			_addedKarts = GameObject.FindGameObjectWithTag("Player").GetComponent<AddedKartsManager>();
		
		if(_lowestAllowedY < -999f)
			_lowestAllowedY = GameObject.FindGameObjectWithTag("BonusRamp").GetComponent<BonusRamp>().LowestPointY - 1.8f;
	}

	private static void EjectPassenger(Transform myPassengerChild)
	{
		if (_addedKarts.PassengerCount <= 0)
		{
			GameEvents.InvokeRunOutOfPassengers();
			return;
		}
		
		var kartPassenger = _addedKarts.PopPassenger;

		if (_addedKarts.PassengerCount % 2 == 0)
		{
			DampCamera.only.UpdateFilledKartCount(_addedKarts.PassengerCount / 2);
			
			if(_addedKarts.AddedKartCount > 0) _addedKarts.PopKart();
		}
		
		kartPassenger.transform.DORotateQuaternion(myPassengerChild.rotation * Quaternion.AngleAxis(180f, Vector3.up), 0.5f);
		
		kartPassenger.transform.DOJump(myPassengerChild.position,
			kartPassenger.transform.position.y - _lowestAllowedY,
			1,
			1.25f).OnComplete(() =>
		{
			myPassengerChild.gameObject.SetActive(true);
			kartPassenger.gameObject.SetActive(false);
		});
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_hasEntered) return;
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(2);

		_hasEntered = true;
		
		leftFlag.DOLocalRotate(Vector3.up * -90f, 0.5f).SetEase(Ease.OutElastic);
		rightFlag.DOLocalRotate(Vector3.up * 90f, 0.5f).SetEase(Ease.OutElastic);
		
		EjectPassenger(myPassengerChild);
		
		if(AudioManager.instance)
		{
			AudioManager.instance.Play("BonusTile");
			AudioManager.instance.Play("Hype" + Random.Range(1, 5));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(_hasExited) return;
		if (!other.CompareTag("Player")) return;
		var myPassengerChild = transform.GetChild(3);

		_hasExited = true;

		EjectPassenger(myPassengerChild);
		
		if(AudioManager.instance)
			AudioManager.instance.Play("BonusTile");
	}
}
