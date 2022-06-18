using Kart;
using UnityEngine;

public class SplineTriggerHelper : MonoBehaviour
{
	private MainKartController _player;

	private void Start()
	{
		_player = GameObject.FindWithTag("Player").GetComponent<MainKartController>();
	}

	public void EnterHighSpeed() => EnterAction();

	public void EnterNormalSpeed() => EnterNormalcy();

	public void EnterLeftHelix()
	{
		EnterAction();
		
		CameraFxController.only.SetSpeedLinesStatus(false);
		GameEvents.InvokeEnterHelix(true);
	}
	
	public void EnterRightHelix()
	{
	    EnterAction();
    		
		CameraFxController.only.SetSpeedLinesStatus(false);
		GameEvents.InvokeEnterHelix(false);
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

	public void OnReachTrackEnd()
	{
		GameEvents.InvokeReachEndOfTrack();
		GameEvents.InvokeUpdateHype(true);
	}

	public void EnterArea(int currentAreaCode)
	{
		GameEvents.InvokeStartParade(currentAreaCode);
	}

	public void AttackAction(int currentAreaCode)
	{
		GameEvents.InvokeAttackPlayer(currentAreaCode);
	}

	public void OnEnterSpecialCamera(Transform specialCamera)
	{
		DampCamera.only.OnEnterSpecialCamera(specialCamera);
	}

	public void OnExitSpecialCamera()
	{
		DampCamera.only.OnExitSpecialCamera();	
	}


}
