using Kart;
using UnityEngine;

public class SplineTriggerHelper : MonoBehaviour
{
	private MainKartRefBank _player;

	private void Start()
	{
		_player = GameObject.FindWithTag("Player").GetComponent<MainKartRefBank>();
	}

	public void EnterHighSpeed() => EnterAction();

	public void EnterNormalSpeed() => EnterNormalcy();

	public void EnterHelix(bool isLeftHelix)
	{
		EnterAction();
		
		CameraFxController.only.SetSpeedLinesStatus(false);
		GameEvents.InvokeEnterHelix(isLeftHelix);
	}

	public void ExitHelix()
	{
		EnterNormalcy();
		
		GameEvents.InvokeExitHelix();
	}

	private void EnterNormalcy()
	{
		_player.TrackMovement.SetNormalSpeedValues();
		GameEvents.InvokeUpdateHype(false);
		CameraFxController.only.SetSpeedLinesStatus(false);
		CameraFxController.only.DoNormalFov();
	}

	private void EnterAction()
	{
		_player.TrackMovement.SetHighSpeedValues();
		CameraFxController.only.SetSpeedLinesStatus(true);
		GameEvents.InvokeUpdateHype(true);
		CameraFxController.only.DoWideFov();
	}
	
	public void OnReachTrackEnd() => GameEvents.InvokeUpdateHype(true);
}
