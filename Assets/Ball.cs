using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private Rigidbody _rb;

	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;


		transform.DOJump(transform.position +
						 Vector3.Normalize(transform.forward +
										   transform.right * (.5f * (Random.value > 0.5f ? 1 : -1))) * 10
						 + Vector3.up * 10, 2f, 1, 2f);
		/*var pos = transform.position + 
				  Vector3.Normalize(transform.forward + transform.right * (.5f * (Random.value > 0.5f ? 1 : -1))) * 10
				  + Vector3.up * 10;
			
		transform.DOMoveX(pos.x, 1f).SetEase(Ease.OutQuart);
		transform.DOMoveY(pos.y, 1.5f).SetEase(Ease.OutQuart);
		transform.DOMoveZ(pos.z, 1.5f).SetEase(Ease.OutQuart);*/
		//_rb.AddForce(-transform.forward * 20f, ForceMode.Impulse);
	}
}
