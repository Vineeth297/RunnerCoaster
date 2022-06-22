using DG.Tweening;
using UnityEngine;

public class BouncingBall : MonoBehaviour
{
	private Animator _animator;
	private void Start()
	{
		_animator = GetComponent<Animator>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		
		//Hulk Smash
		var collisionPoint = other.ClosestPoint(transform.position);
		GameEvents.InvokeKartCrash(collisionPoint);
	}

	public void StopBouncing()
	{
		_animator.enabled = false;
	}
}
