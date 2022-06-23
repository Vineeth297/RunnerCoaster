using DG.Tweening;
using Kart;
using StateMachine;
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
	
	public void EnterHypeArea() => 	GameEvents.InvokeUpdateHype(true);
	public void ExitHypeArea() => 	GameEvents.InvokeUpdateHype(false);

	public void RemoveInputControl() => MoveOnTrackState.ChangeStatePersistence(true);
	public void RestoreInputControl() => MoveOnTrackState.ChangeStatePersistence(false);

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

	public void PassengerJump()
	{
		GameEvents.InvokePassengerJump();
		CameraFxController.only.DoCustomFov(75);
		TimeController.only.SlowDownTime();
		DOVirtual.DelayedCall(0.75f, () => TimeController.only.RevertTime());
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

	public void EnterArea(int currentAreaCode) => GameEvents.InvokeStartParade(currentAreaCode);

	public void AttackAction(int currentAreaCode) => GameEvents.InvokeAttackPlayer(currentAreaCode);

	public void OnEnterSpecialCamera(Transform specialCamera) => DampCamera.only.OnEnterSpecialCamera(specialCamera);
	public void OnEnterSpecialCameraSlow(Transform specialCamera) => DampCamera.only.OnEnterSpecialCamera(specialCamera, true);

	public void OnExitSpecialCamera() => DampCamera.only.OnExitSpecialCamera();
}
