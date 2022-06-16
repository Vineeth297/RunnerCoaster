using System;
using DG.Tweening;
using Kart;
using TMPro;
using UnityEngine;

public class BodyChangeGate : MonoBehaviour
{
	private enum ChangeType { Add, Subtract, Multiply, Divide }

	[SerializeField] private AdditionalKartManager rollerCoasterManager;

	[SerializeField] private float moveDuration = 3f;
	[SerializeField] private float horizontalDistance;

	[SerializeField] private int factor;
	[SerializeField] private TMP_Text text;
	[SerializeField] private ChangeType changeType;
	[SerializeField] private Color positiveColor, negativeColor;

	private bool _hasBeenUsed;
	private bool _isAtExtreme;

	private void Start()
	{
		if (changeType == ChangeType.Add || changeType == ChangeType.Multiply)
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = positiveColor;
		else
			transform.GetChild(0).GetComponent<MeshRenderer>().material.color = negativeColor;

		transform.position -= transform.right * horizontalDistance / 2;
		transform.DOMove(transform.position + transform.right * horizontalDistance, moveDuration)
			.SetEase(Ease.Linear)
			.SetLoops(-1, LoopType.Yoyo);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_hasBeenUsed) return;
		if (!other.CompareTag("Player")) return;
		
		_hasBeenUsed = true;
		switch (changeType)
		{
			case ChangeType.Add:
				rollerCoasterManager.SpawnKarts(factor);
				break;
			case ChangeType.Subtract:
				//rollerCoasterManager.HideKarts(factor);
				print("hi");
				break;
			case ChangeType.Multiply:
				rollerCoasterManager.SpawnKarts(factor);
				break;
			case ChangeType.Divide:
				//rollerCoasterManager.HideKarts(factor);
				print("hi");
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void OnValidate()
	{
		name = changeType switch
		{
			ChangeType.Add => text.text = " + " + factor,
			ChangeType.Subtract => text.text = " - " + factor,
			ChangeType.Multiply => text.text = " + " + factor,
			ChangeType.Divide => text.text = " - " + factor,
			_ => name
		};
	}
}