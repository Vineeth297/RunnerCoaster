using Dreamteck.Splines;
using UnityEngine;

namespace Player
{
	public class PlayerRefBank : MonoBehaviour
	{
		public SplineFollower Follower { get; private set; }
		public PlayerTrackMovement TrackMovement { get; private set; }
		public PlayerFlyMovement FlyMovement { get; private set; }
		
		private void Start()
		{
			TrackMovement = GetComponent<PlayerTrackMovement>();
			FlyMovement = GetComponent<PlayerFlyMovement>();
			Follower = GetComponent<SplineFollower>();
		}
	}
}