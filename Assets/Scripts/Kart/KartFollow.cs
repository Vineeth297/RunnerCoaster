using UnityEngine;

namespace Kart
{
	public class KartFollow : MonoBehaviour
	{
		public Transform charToFollow;
		[SerializeField] private Vector3 followOffset;
		[SerializeField] private float damping;
		
		private Transform _transform;

		private void Start() => _transform = transform;

		private void LateUpdate()
		{
			if(!charToFollow) return;
			
			var smoothPos = Vector3.Lerp(_transform.position, charToFollow.position + followOffset,
				Time.deltaTime * damping);
			_transform.position = smoothPos;
			_transform.eulerAngles = charToFollow.eulerAngles;
		}
	}
}