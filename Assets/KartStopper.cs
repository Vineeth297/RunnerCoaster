using UnityEngine;

public class KartStopper : MonoBehaviour
{
	private Collider _collider;

	private void Start()
	{
		_collider = GetComponent<Collider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player")) return;
		GameEvents.InvokeStopTheRollerCoaster();
		_collider.enabled = false;
	}
}
