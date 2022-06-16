using Dreamteck.Splines;
using UnityEngine;

namespace Kart
{
	public class MainKartController : MonoBehaviour
	{
		public SplineFollower Follower { get; private set; }
		public KartTrackMovement TrackMovement { get; private set; }
		public Wagon Wagon { get; private set; }
		public TrainEngine TrainEngine { get; private set; }
		public KartFlyMovement FlyMovement { get; private set; }
		public AddedKartsManager AddedKartsManager { get; private set; }
		public Collider BoxCollider { get; private set; }
		
		public bool isInitialised;
		
		private void OnEnable()
		{
			GameEvents.ReachEndOfTrack += OnReachEndOfTrack;
		}

		private void OnDisable()
		{
			GameEvents.ReachEndOfTrack -= OnReachEndOfTrack;
		}

		private void Start()
		{
			TrackMovement = GetComponent<KartTrackMovement>();
			Follower = GetComponent<SplineFollower>();
			Wagon = GetComponent<Wagon>();
			TrainEngine = GetComponent<TrainEngine>();
			FlyMovement = GetComponent<KartFlyMovement>();
			AddedKartsManager = GetComponent<AddedKartsManager>();
			BoxCollider = GetComponent<Collider>();
			isInitialised = true;
		}

		private void OnReachEndOfTrack()
		{
			Follower.follow = false;
			Wagon.enabled = false;
			TrainEngine.enabled = false;
		}
	}
}