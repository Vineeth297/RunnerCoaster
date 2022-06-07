using System;

public static class GameEvents
{
	public static event Action Explosion;
	public static event Action GetHyped;
	public static event Action NoHype;
	public static event Action ExplosionCameraPushBack;
	public static event Action BonusCameraPushBack;
	public static event Action LeftCurveCameraShift;
	public static event Action RightCurveCameraShift;
	public static event Action ResetCameraPosition;
	public static event Action FlyToBonusRamp;
	
	
	public static void InvokeExplosion() => Explosion?.Invoke();
	public static void InvokeGetHyped() => GetHyped?.Invoke();
	public static void InvokeNoHype() => NoHype?.Invoke();
	public static void InvokeExplosionCameraPushBack() => ExplosionCameraPushBack?.Invoke();
	public static void InvokeBonusCameraPushBack() => BonusCameraPushBack?.Invoke();
	public static void InvokeLeftCurveCameraShift() => LeftCurveCameraShift?.Invoke();
	public static void InvokeRightCurveCameraShift() => RightCurveCameraShift?.Invoke();
	public static void InvokeResetCameraPosition() => ResetCameraPosition?.Invoke();
	public static void InvokeFlyToBonusRamp() => FlyToBonusRamp?.Invoke();
}
