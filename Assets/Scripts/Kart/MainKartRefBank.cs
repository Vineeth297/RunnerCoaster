using Dreamteck.Splines;
using UnityEngine;

namespace Kart
{
	public class MainKartRefBank : MonoBehaviour
	{
		public SplineFollower Follower { get; private set; }
		public KartTrackMovement TrackMovement { get; private set; }
		public KartFlyMovement FlyMovement { get; private set; }
		
		public AdditionalKartManager AdditionalKartManager { get; private set; }
		
		private void Start()
		{
			TrackMovement = GetComponent<KartTrackMovement>();
			FlyMovement = GetComponent<KartFlyMovement>();
			Follower = GetComponent<SplineFollower>();
			AdditionalKartManager = GetComponent<AdditionalKartManager>();
		}
	}
}