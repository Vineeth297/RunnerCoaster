using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class DampCamera : MonoBehaviour
	{
		public static DampCamera only;
		
		[SerializeField] private Transform target;
		[SerializeField] private float lerpMul, cameraTransitionDuration = 0.5f;

		[SerializeField] private Transform leftCameraPos, bonusCameraPos;
	
		private Transform _transform;
		private Vector3 _initLocalPosition;
		private Quaternion _iniLocalRotation;

		private void OnEnable()
		{
			GameEvents.EnterHelix += OnEnterHelix;
			GameEvents.ExitHelix += OnExitHelix;
			GameEvents.ReachEndOfTrack += BonusCameraPosition;
		}

		private void OnDisable()
		{
			GameEvents.EnterHelix -= OnEnterHelix;
			GameEvents.ExitHelix -= OnExitHelix;
			GameEvents.ReachEndOfTrack -= BonusCameraPosition;
		}

		private void Awake()
		{
			if (only) Destroy(this);
			else only = this;
		}

		private void Start()
		{
			_transform = transform;

			_initLocalPosition = target.localPosition;
			_iniLocalRotation = target.localRotation;
		}

		private void LateUpdate()
		{
			_transform.position = target.position;
			_transform.rotation = Quaternion.SlerpUnclamped(_transform.rotation, target.rotation, Time.deltaTime * lerpMul);
		}

		private void OnEnterHelix(bool isLeftHelix)
		{
			target.DOLocalMove(leftCameraPos.localPosition, cameraTransitionDuration);
			target.DOLocalRotate( new Vector3(15f,-30f,0f) , cameraTransitionDuration); 
		}

		private void RightCurveCameraPosition()
		{
			target.DOLocalMove(target.localPosition + Vector3.right * -5.5f,cameraTransitionDuration);
			target.DOLocalRotate( new Vector3(-15f,30f,0f) , cameraTransitionDuration); 
		}

		private void OnExitHelix() => CameraResetPosition();

		private void CameraResetPosition()
		{
			target.DOLocalMove(_initLocalPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(_iniLocalRotation, cameraTransitionDuration);
		}

		private void BonusCameraPosition()
		{
			target.DOLocalMove(bonusCameraPos.localPosition, cameraTransitionDuration);
		}
	}
}
