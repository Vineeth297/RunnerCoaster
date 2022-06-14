using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class DampCamera : MonoBehaviour
	{
		public static DampCamera only;
		
		[SerializeField] private Transform target;
		[SerializeField] private float lerpMul, cameraTransitionDuration = 0.5f;

		[SerializeField] private Transform rightActionCamera, bonusCameraPos, deathCamPos, leftObstacleCam;

		private Transform _transform, _lastCamTarget;
		private Vector3 _initLocalPosition;
		private Quaternion _iniLocalRotation;

		private void OnEnable()
		{
			GameEvents.EnterHelix += OnEnterHelix;
			GameEvents.ExitHelix += OnExitHelix;

			GameEvents.ObstacleCollision += OnObstacleCollision;
			
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
		}

		private void OnDisable()
		{
			GameEvents.EnterHelix -= OnEnterHelix;
			GameEvents.ExitHelix -= OnExitHelix;
			
			GameEvents.ObstacleCollision -= OnObstacleCollision;
			
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
		}

		private void Awake()
		{
			if (only) Destroy(this);
			else only = this;
		}

		private void Start()
		{
			_transform = transform;
			_transform.parent = null;

			_initLocalPosition = target.localPosition;
			_iniLocalRotation = target.localRotation;
		}

		private void LateUpdate()
		{
			_transform.position = target.position;
			_transform.rotation = Quaternion.SlerpUnclamped(_transform.rotation, target.rotation, Time.deltaTime * lerpMul);
		}

		private void RightCurveCameraPosition()
		{
			target.DOLocalMove(target.localPosition + Vector3.right * -5.5f,cameraTransitionDuration);
			target.DOLocalRotate( new Vector3(-15f,30f,0f) , cameraTransitionDuration); 
		}

		private void CameraResetPosition()
		{
			target.DOLocalMove(_initLocalPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(_iniLocalRotation, cameraTransitionDuration);
		}

		private void OnEnterHelix(bool isLeftHelix)
		{
			target.DOLocalMove(rightActionCamera.localPosition, cameraTransitionDuration);
			target.DOLocalRotate( new Vector3(15f,-30f,0f) , cameraTransitionDuration); 
		}

		private void OnExitHelix() => CameraResetPosition();

		private void OnObstacleCollision(Vector3 collisionPoint) => target.DOMove(deathCamPos.position, cameraTransitionDuration);

		private void OnReachEndOfTrack()
		{
			target.DOLocalMove(bonusCameraPos.localPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(bonusCameraPos.localRotation, cameraTransitionDuration);
		}

		public void ReturnFromObstacleCam()
		{
			target.DOLocalMove(_lastCamTarget.position, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(_lastCamTarget.localRotation, cameraTransitionDuration);
		}

		public void SendToObstacleCam(bool shouldGoToLeftCam)
		{
			_lastCamTarget = target;
			target.DOLocalMove(leftObstacleCam.position, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(leftObstacleCam.localRotation, cameraTransitionDuration);
		}
	}
}
