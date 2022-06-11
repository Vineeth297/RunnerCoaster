using Dreamteck.Splines;
using UnityEngine;

namespace Kart
{
	public class AdditionalKartRefBank : MonoBehaviour
	{
		public Rigidbody explosionKart;
		public SplinePositioner Positioner { get; private set; }
		public Wagon Wagon { get; private set; }
		public KartFollow KartFollow { get; private set; }
		public Collider BoxCollider { get; private set; }

		public bool isInitialised;

		private void Start()
		{
			Wagon = GetComponent<Wagon>();
			KartFollow = GetComponent<KartFollow>();
			isInitialised = true;
			Positioner = GetComponent<SplinePositioner>();
			BoxCollider = GetComponent<Collider>();
		}
	}
}