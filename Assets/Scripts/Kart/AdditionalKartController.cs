using Dreamteck.Splines;
using UnityEngine;

namespace Kart
{
	public class AdditionalKartController : MonoBehaviour
	{
		public Rigidbody explosionKart;
		public SplinePositioner Positioner { get; private set; }
		public Wagon Wagon { get; private set; }
		public KartFollow KartFollow { get; private set; }
		public Collider BoxCollider { get; private set; }

		public Passenger passenger1, passenger2;

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
			Wagon = GetComponent<Wagon>();
			KartFollow = GetComponent<KartFollow>();
			isInitialised = true;
			Positioner = GetComponent<SplinePositioner>();
			BoxCollider = GetComponent<Collider>();
		}

		private void OnReachEndOfTrack()
		{
			Positioner.enabled = false;
			Wagon.enabled = false;
			KartFollow.enabled = true;
		}

		public void RemoveKartsFromHere(Vector3 collisionPoint)
		{
			KartFollow.SetKartToFollow(null);

			AddedKartsManager GetMainKart()
			{
				Wagon currentFront = null;
				Wagon candidate = GetComponent<Wagon>();
			
				do
				{
					candidate = candidate.front;
					if (candidate)
						currentFront = candidate;
				} while (candidate);

				return currentFront != null ? currentFront.GetComponent<AddedKartsManager>() : null;
			}

			int GetNumberOfRearKarts()
			{
				var candidate = Wagon;

				var count = 1;
				do
				{
					candidate = candidate.back;
					if (candidate)
						count++;
				} while (candidate);

				return count;
			}
			
			Positioner.enabled = false;
			Wagon.enabled = false;

			print($"maink {GetMainKart()} noOfrearK {GetNumberOfRearKarts()}");
			
			GetMainKart().ExplodeMultipleKarts(GetNumberOfRearKarts(), collisionPoint);
		}
	}
}