using UnityEngine;

public class SplineTriggerHelper : MonoBehaviour
{
	private Player.PlayerRefBank _player;

	private void Start()
	{
		_player = GameObject.FindWithTag("Player").GetComponent<Player.PlayerRefBank>();
	}

	public void EnterDownhill()
	{
		_player.TrackMovement.SetDownhillSpeedValues();
		CameraFxController.only.SetSpeedLinesStatus(true);
		GameEvents.InvokeGetHyped();
		GameManager.Instance.SpeedPushEffect();
	}

	public void EnterPlain()
	{
		_player.TrackMovement.SetPlainSpeedValues();
		CameraFxController.only.SetSpeedLinesStatus(false);
	}

	public void EnterHelix()
	{
		_player.TrackMovement.SetDownhillSpeedValues();
		GameEvents.InvokeGetHyped();
		GameManager.Instance.SpeedPushEffect();
	}
}
