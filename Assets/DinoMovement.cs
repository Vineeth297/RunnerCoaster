using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DinoMovement : MonoBehaviour
{
	public float speed;
	public List<Transform> patrolPoints;
	public float waitTime;
	private int _currentPointIndex;

	private bool _once;

	private Animator _animator;
	private static readonly int ToRunHash = Animator.StringToHash("ToRun");

	
	
	private void Start()
	{
		_animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (transform.position != patrolPoints[_currentPointIndex].position)
		{
			transform.position = Vector3.MoveTowards(transform.position, patrolPoints[_currentPointIndex].position,speed * Time.deltaTime);
		}
		else
		{
			if (!_once)
			{
				_once = true;
				StartCoroutine(Wait());
			}
		}

		
	}
	private IEnumerator Wait()
	{
		_animator.SetBool(ToRunHash,false);
		yield return new WaitForSeconds(waitTime);
		if (_currentPointIndex + 1 < patrolPoints.Count)
		{
			var currentIndex = _currentPointIndex;
			_currentPointIndex++;
			RotateTheDino(currentIndex, _currentPointIndex);
			_animator.SetBool(ToRunHash,true);
		}
		else
		{
			var currentIndex = _currentPointIndex;
			_currentPointIndex = 0;
			RotateTheDino(currentIndex, _currentPointIndex);
			_animator.SetBool(ToRunHash,true);
		}

		_once = false;
	}

	private void RotateTheDino(int currentIndex,int toRotatePositionIndex)
	{
		var toRotateDirection = patrolPoints[toRotatePositionIndex].position - patrolPoints[currentIndex].position;
		transform.DORotateQuaternion(Quaternion.LookRotation(toRotateDirection), 1f);
		print("current index = " + toRotatePositionIndex);
	}
}
