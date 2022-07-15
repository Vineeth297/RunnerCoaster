﻿using System;
using DG.Tweening;
using Kart;
using ToonyColorsPro;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
	[SerializeField] private bool isMainKart;

	private MainKartController _my;
	private bool _isKart;
	
	private static bool _isInCooldown;
	private static Tween _cooldown;

	private bool _isPlayerOnFever;

	private void OnEnable()
	{
		GameEvents.PlayerOnFever += OnFever;
		GameEvents.PlayerOffFever += OffFever;
	}

	private void OnDisable()
	{
		GameEvents.PlayerOnFever -= OnFever;
		GameEvents.PlayerOffFever -= OffFever;
	}

	private void Start()
	{
		_isKart = TryGetComponent(out Wagon _);
		if(!isMainKart) return;
		_my = GetComponent<MainKartController>();

		Tween checker = null;
		checker = DOVirtual.DelayedCall(0.1f, () =>
		{
			if (!_my.isInitialised) return;

			_my.Follower.follow = true;
			checker.Kill();
		}).SetLoops(-1);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isPlayerOnFever) return;
		
		if (!other.CompareTag("Player") && !other.CompareTag("Kart")) return;

		var collisionPoint = other.ClosestPoint(transform.position);
		if(!_isKart)
		{
			if(!TryGiveHit()) return;
			GameEvents.InvokeMainKartCrash(collisionPoint);
		}
		else if (other.TryGetComponent(out MainKartController _))
		{
			if(!TryGiveHit()) return;
			GameEvents.InvokeMainKartCrash(collisionPoint);
		}
		else if (other.TryGetComponent(out AdditionalKartController addy))
		{
			if(!TryGiveHit()) return;
			addy.RemoveKartsFromHere(collisionPoint);
			GameEvents.InvokeKartCrash(collisionPoint);
		}
	}

	private static bool TryGiveHit()
	{
		if (_isInCooldown) return false;

		_isInCooldown = true;
		DOVirtual.DelayedCall(0.5f, () => _isInCooldown = false);
		return true;
	}

	private void OnFever()
	{
		_isPlayerOnFever = true;
	}

	private void OffFever()
	{
		_isPlayerOnFever = false;
	}
}