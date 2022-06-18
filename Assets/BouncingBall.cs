using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BouncingBall : MonoBehaviour
{
	[SerializeField] private float jumpHeight = 2.5f;
	[SerializeField] private float jumpDuration = 0.5f;
	[SerializeField] private float delay = 0.5f;

	private Vector3 _startPosition;
	private Sequence _sequence;

	private Vector3 _initialScale;
	private void Start()
	{
		_initialScale = transform.lossyScale;
		_startPosition = transform.localPosition;
		
		_sequence = DOTween.Sequence();
		_sequence.Append(transform.DOLocalMove(_startPosition + Vector3.up * jumpHeight, jumpDuration)
			.SetEase(Ease.InExpo));
		_sequence.AppendInterval(delay);
		_sequence.Append(transform.DOLocalMove(_startPosition, jumpDuration).SetEase(Ease.OutExpo));
		_sequence.SetLoops(-1);	
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		
		//Hulk Smash
		var collisionPoint = other.ClosestPoint(transform.position);
		GameEvents.InvokeObstacleCollision(collisionPoint);
	}

	public void StopBouncing()
	{
		_sequence.Kill();
	}
}
