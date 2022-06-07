using System;
using DG.Tweening;
using UnityEditor.Rendering;
using UnityEngine;

public class DampCamera : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private float lerpMul;
	
	private Transform _transform;
	[SerializeField] private Transform leftCameraPos;
	[SerializeField] private Transform bonusCameraPos;
	
	private void OnEnable()
	{
		GameEvents.RightCurveCameraShift += RightCurveCameraPosition;
		GameEvents.LeftCurveCameraShift += LeftCurveCameraPosition;
		GameEvents.ResetCameraPosition += CameraResetPosition;
		GameEvents.FlyToBonusRamp += BonusCameraPosition;
	}

	private void OnDisable()
	{
		GameEvents.RightCurveCameraShift -= RightCurveCameraPosition;
		GameEvents.LeftCurveCameraShift -= LeftCurveCameraPosition;		
		GameEvents.ResetCameraPosition -= CameraResetPosition;
		GameEvents.FlyToBonusRamp -= BonusCameraPosition;
	}

	private void Start() => _transform = transform;

	private void LateUpdate()
	{
		//_transform.position = Vector3.Lerp(_transform.position, target.position, Time.deltaTime * lerpMul);
		_transform.position = target.position;
		_transform.rotation = Quaternion.SlerpUnclamped(_transform.rotation, target.rotation, Time.deltaTime * lerpMul);
	}

	private void LeftCurveCameraPosition()
	{
		//target.DOLocalMove(target.localPosition + Vector3.right * 5.5f,0.5f);
		target.DOLocalMove(leftCameraPos.localPosition,0.5f);
		target.DOLocalRotate( new Vector3(15f,-30f,0f) , 0.5f); 
	}
	
	private void RightCurveCameraPosition()
	{
		target.DOLocalMove(target.localPosition + Vector3.right * -5.5f,0.5f);
		target.DOLocalRotate( new Vector3(-15f,30f,0f) , 0.5f); 
	}

	private void CameraResetPosition()
	{
		target.DOLocalMove(new Vector3(0f,12.18f,-21.14f), 0.5f);
		target.DOLocalRotate(new Vector3(22.276f,0f,0f) , 0.5f); 		
	}

	private void BonusCameraPosition()
	{
		target.DOLocalMove(bonusCameraPos.localPosition ,0.5f);
	}
}
