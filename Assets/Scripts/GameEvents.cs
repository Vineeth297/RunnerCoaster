using System;

public static class GameEvents
{
	public static event Action<bool> UpdateHype;
	public static event Action<bool> EnterHelix;
	public static event Action ExitHelix;
	public static event Action ObstacleCollision;
	public static event Action ResetCameraPosition;
	public static event Action ReachEndOfTrack;
	public static event Action BonusCameraPushBack;
	public static event Action StopOnBonusRamp;


	public static void InvokeObstacleCollision() => ObstacleCollision?.Invoke();

	public static void InvokeUpdateHype(bool status) => UpdateHype?.Invoke(status);
	public static void InvokeEnterHelix(bool isLeftHelix) => EnterHelix?.Invoke(isLeftHelix);
	public static void InvokeExitHelix() => ExitHelix?.Invoke();

	public static void InvokeBonusCameraPushBack() => BonusCameraPushBack?.Invoke();

	public static void InvokeResetCameraPosition() => ResetCameraPosition?.Invoke();

	public static void InvokeReachEndOfTrack() => ReachEndOfTrack?.Invoke();

	public static void InvokeStopOnBonusRamp() => StopOnBonusRamp?.Invoke();
}