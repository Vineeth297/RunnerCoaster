using DG.Tweening;
using UnityEngine;

namespace Kart
{
	public class DampCamera : MonoBehaviour
	{
		public static DampCamera only;

		[SerializeField] private Transform target;
		private Transform _targetParent;
		[SerializeField] private float lerpMul, cameraTransitionDuration = 0.5f;

		[SerializeField] private Transform rightActionCamera, bonusCameraPos, deathCamPos, leftObstacleCam;

		private Transform _transform;
		private Quaternion _initLocalRotation;
		private Vector3 _initLocalPosition;

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
			_targetParent = target.parent;
			
			_initLocalPosition = target.localPosition;
			_initLocalRotation = target.localRotation;
		}

		private void LateUpdate()
		{
			_transform.position = target.position;
			_transform.rotation = Quaternion.SlerpUnclamped(_transform.rotation, target.rotation, Time.deltaTime * lerpMul);
		}

		public void SendToObstacleCam(bool shouldGoToLeftCam)
		{
			target.DOLocalMove(leftObstacleCam.localPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(leftObstacleCam.localRotation, cameraTransitionDuration);
		}

		public void CameraResetPosition()
		{
			target.DOLocalMove(_initLocalPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(_initLocalRotation, cameraTransitionDuration);
		}

		private void RightCurveCameraPosition()
		{
			target.DOLocalMove(target.localPosition + Vector3.right * -5.5f,cameraTransitionDuration);
			target.DOLocalRotate( new Vector3(-15f,30f,0f) , cameraTransitionDuration); 
		}

		private void OnEnterHelix(bool isLeftHelix)
		{
			target.DOLocalMove(rightActionCamera.localPosition, cameraTransitionDuration);
			target.DOLocalRotate( new Vector3(15f,-30f,0f) , cameraTransitionDuration); 
		}

		public void OnEnterSpecialCamera(Transform specialCamera)
		{
			target.parent = null;
			target.DOMove(specialCamera.position, cameraTransitionDuration);
		}

		public void OnExitSpecialCamera()
		{
			target.parent = _targetParent;
			CameraResetPosition();
		}
		
		
		private void OnExitHelix() => CameraResetPosition();

		private void OnObstacleCollision(Vector3 collisionPoint)
		{
			target.DOMove(deathCamPos.position, cameraTransitionDuration);
			target.DORotateQuaternion(deathCamPos.rotation, cameraTransitionDuration);
		}

		private void OnReachEndOfTrack()
		{
			target.DOLocalMove(bonusCameraPos.localPosition, cameraTransitionDuration);
			target.DOLocalRotateQuaternion(bonusCameraPos.localRotation, cameraTransitionDuration);
		}
	}
}
