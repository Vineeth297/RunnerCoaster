using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Kart
{
	public class Fever : MonoBehaviour
	{
		[SerializeField] private Image fever;
		[SerializeField] private float feverFillMultiplier;
		[SerializeField] private float feverEmptyMultiplier;
		[SerializeField] private GameObject feverOverlayPanel;

		[SerializeField] private GameObject feverText;
		
		private bool _toConsumeFever;


		private void OnEnable()
		{
			GameEvents.PlayerOnFever += OnFeverOverlay;
			GameEvents.PlayerOnFever += PlayFeverHype;
			GameEvents.PlayerOffFever += OffFeverOverlay;
		}
		
		private void OnDisable()
		{
			GameEvents.PlayerOnFever -= OnFeverOverlay;
			GameEvents.PlayerOnFever -= PlayFeverHype;
			GameEvents.PlayerOffFever -= OffFeverOverlay;
		}
		

		private void Start()
		{
			fever.fillAmount = 0f;
		}

		public void HandleFeverAmount()
		{
			if(_toConsumeFever)
				ConsumeFever();
			else
				IncreaseFeverAmount();
		}

		public void DecreaseFeverAmount()
		{
			if (fever.fillAmount <= 0f)
			{
				if (_toConsumeFever)
				{
					GameEvents.InvokePlayerOffFever();
					DampCamera.only.StopMediatingTarget();
				}
				return;
			}

			fever.fillAmount -= Time.deltaTime * feverEmptyMultiplier;
		}

		private void ConsumeFever()
		{
			if (fever.fillAmount <= 0f)
			{
				_toConsumeFever = false;
				GameEvents.InvokePlayerOffFever();
				DampCamera.only.StopMediatingTarget();
				return;
			}
			//CameraFxController.only.UpdateRumblerRotation(fever.fillAmount);
			fever.fillAmount -= Time.deltaTime * feverFillMultiplier;
		}

		private void IncreaseFeverAmount()
		{
			if (fever.fillAmount >= 1f)
			{
				_toConsumeFever = true;
				GameEvents.InvokePlayerOnFever(); 
				return;
			}
			if(!_toConsumeFever)
				fever.fillAmount += Time.deltaTime * feverFillMultiplier;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_toConsumeFever) return;
			
			if (other.CompareTag("ObstacleTrain") || other.CompareTag("ObstacleKart"))
			{
				print("pop");
				//Explode Obstacle karts
				var wag = other.TryGetComponent(out AdditionalKartController additionalKart);
				if (wag)
				{
					//GameEvents.InvokeMainKartCrash(other.ClosestPoint(other.transform.position));
					additionalKart.RemoveKartsFromHere(other.transform.position);
				}
			}
		}

		private void OnFeverOverlay()
		{
			feverOverlayPanel.SetActive(true);
			feverText.SetActive(true);
		}

		private void OffFeverOverlay()
		{
			feverOverlayPanel.SetActive(false);
			feverText.SetActive(false);
		}
		
		private void PlayFeverHype()
		{
			AudioManager.instance.Play("Jump1" );
		}
	}
	
	
	
}