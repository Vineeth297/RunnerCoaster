using UnityEngine;

namespace Kart
{
	public class KartFollow : MonoBehaviour
	{
		public Vector3 followOffset;
		public Transform charToFollow;
		public float damping;
		
		private void LateUpdate()
		{
			if(!charToFollow) return;
			
			var smoothPos = Vector3.Lerp(transform.position, charToFollow.position + followOffset,
				Time.deltaTime * damping);
			transform.position = smoothPos;
			transform.eulerAngles = charToFollow.eulerAngles;
		}
	}
}