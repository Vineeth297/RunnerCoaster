using Kart;
using UnityEngine;

public class SpeedBooster : MonoBehaviour
{
	private MainKartController _player;
	private bool _usedUp;
	
	private void Start()
	{
		_player = GameObject.FindWithTag("Player").GetComponent<MainKartController>();
	}

	private void AddSpeedBoost() => _player.TrackMovement.AddSpeedBoost();

	private void OnTriggerEnter(Collider other)
	{
		if(_usedUp) return;
		if(!other.CompareTag("Player")) return;

		_usedUp = true;
		AddSpeedBoost();
	}
}