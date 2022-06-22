using UnityEngine;

public class Helicopter : MonoBehaviour
{
	private Animator _animator;
	private Rigidbody _rb;

	[SerializeField] private float forceForExplosion;
	private void Start()
	{
		_animator = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		
		//Hulk Smash
		HeliDeath(forceForExplosion);
		var collisionPoint = other.ClosestPoint(transform.position);
		GameEvents.InvokeKartCrash(collisionPoint);
	}

	public void StopHelicopter()
	{
		_animator.enabled = false;
	}

	private void HeliDeath(float explosionForce)
	{
		_rb.useGravity = true;
		_rb.isKinematic = false;
		_rb.constraints = RigidbodyConstraints.None;

		_rb.AddForce(Vector3.left * 5f * explosionForce + Vector3.down * explosionForce + Vector3.back * 40f * explosionForce, ForceMode.Impulse);
		_rb.AddTorque(Vector3.up * 180f, ForceMode.Acceleration);
	}
}
