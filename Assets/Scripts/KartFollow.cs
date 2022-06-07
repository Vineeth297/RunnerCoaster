using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartFollow : MonoBehaviour
{
	public Vector3 followOffset;
	public Transform charToFollow;
	public float damping;

	public Vector3 pos;

	public bool isRolling;
	private void LateUpdate()
	{
		Vector3 smoothPos = Vector3.Lerp(transform.position, charToFollow.position + followOffset , Time.deltaTime * damping);
		transform.position = smoothPos;
		transform.eulerAngles = charToFollow.eulerAngles;

		/*transform.position = charToFollow.transform.position + followOffset;
		transform.eulerAngles = charToFollow.eulerAngles;*/
	}
}
