using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Exhaust : MonoBehaviour
{
	[SerializeField] private GameObject exhaustParticleSystem;
	private Vector3 _initScale;
	[SerializeField] private float scaleMultiplier = 0.2f;
	private void OnEnable()
	{
		GameEvents.PlayerOnFever += ExhaustOn;
		GameEvents.PlayerOffFever += ExhaustOff;
	}

	private void OnDisable()
	{
		GameEvents.PlayerOnFever -= ExhaustOn;
		GameEvents.PlayerOffFever -= ExhaustOff;		
	}

	private void Start()
	{
		_initScale = exhaustParticleSystem.transform.lossyScale;
	}
	
	private void ExhaustOn()
	{
		exhaustParticleSystem.SetActive(true);
		exhaustParticleSystem.transform.DOScale(_initScale * scaleMultiplier, 0.5f).SetLoops(2, LoopType.Yoyo);
	}

	private void ExhaustOff() => exhaustParticleSystem.SetActive(false);
}
