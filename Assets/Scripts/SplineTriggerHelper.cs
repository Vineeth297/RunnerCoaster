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

	public void RemoveInputControl() => MoveOnTrackState.ChangeStatePersistence(true);
	public void RestoreInputControl() => MoveOnTrackState.ChangeStatePersistence(false);

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
	
}
