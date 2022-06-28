using DG.Tweening;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
	private Animator _animator;
	private Rigidbody _rb;

	[SerializeField] private float forceForExplosion;
	private AudioSource _audio;

	private void Start()
	{
		_animator = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody>();
		_audio = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;

		var direction = other.transform.position - transform.position;
		//Hulk Smash
		HeliDeath(direction.normalized, forceForExplosion);
		var collisionPoint = other.ClosestPoint(transform.position);
		GameEvents.InvokeMainKartCrash(collisionPoint);
	}

	public void StopHelicopter()
	{
		_animator.enabled = false;
	}

	private void HeliDeath(Vector3 direction, float explosionForce)
	{
		_rb.useGravity = true;
		_rb.isKinematic = false;
		_rb.constraints = RigidbodyConstraints.None;

		_rb.AddForce(direction * 5f * explosionForce + Vector3.up * 10f * explosionForce, ForceMode.Impulse);
		_rb.AddTorque(Vector3.up * 180f, ForceMode.Acceleration);

		DOTween.To(() => _audio.pitch, value => _audio.pitch = value, 0f, 1.5f);
	}
}
