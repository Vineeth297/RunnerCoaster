using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
	public GameObject player;        //Public variable to store a reference to the player game object

	private Vector3 offset; //Private variable to store the offset distance between the player and camera
	[SerializeField] private float damping = 1f;
	
	// Use this for initialization

	void Start () 
	{
		//Calculate and store the offset value by getting the distance between the player's position and camera's position.
		offset = transform.position - player.transform.position;
	}

	// LateUpdate is called after Update each frame
	void LateUpdate () 
	{
		// Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
		//var toMovePos = new Vector3(player.transform.position.x,0f,player.transform.position.z) + offset;
		var toMovePos = player.transform.position + offset;

		transform.position = Vector3.Lerp(transform.position, toMovePos, damping * Time.deltaTime);
		// transform.position = player.transform.position + offset;
	}
}
