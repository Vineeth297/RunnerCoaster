using System;
using UnityEngine;

public static class GameEvents
{
	public static event Action TapToPlay;
	public static event Action<bool> UpdateHype;
	public static event Action<bool> EnterHelix;
	public static event Action ExitHelix;
	
	public static event Action<int> StartParade;
	public static event Action<int> AttackPlayer;
	
	public static event Action<Vector3> KartCrash;
	public static event Action ReachEndOfTrack;
	public static event Action RunOutOfPassengers;

	//invoked when the coasters completely stops on the bonus ramp
	public static event Action GameWin;

	public static void InvokeTapToPlay() => TapToPlay?.Invoke();

	public static void InvokeKartCrash(Vector3 collisionPoint) => KartCrash?.Invoke(collisionPoint);
	public static void InvokeUpdateHype(bool status) => UpdateHype?.Invoke(status);
	public static void InvokeEnterHelix(bool isLeftHelix) => EnterHelix?.Invoke(isLeftHelix);
	public static void InvokeExitHelix() => ExitHelix?.Invoke();

	public static void InvokeReachEndOfTrack() => ReachEndOfTrack?.Invoke();

	public static void InvokeRunOutOfPassengers() => RunOutOfPassengers?.Invoke();

	public static void InvokeGameWin() => GameWin?.Invoke();
	public static void InvokeStartParade(int currentAreaCode) => StartParade?.Invoke(currentAreaCode);
	public static void InvokeAttackPlayer(int currentAreaCode) => AttackPlayer?.Invoke(currentAreaCode);
}