using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCollider : MonoBehaviour
{
	private Collider _collider;

	private void Start()
	{
		_collider = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		_collider.enabled = false;
		enabled = false;
	}
}
