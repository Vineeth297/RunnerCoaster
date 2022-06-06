using System;

public static class GameEvents
{
	public static event Action explosion;
	public static event Action getHyped;
	public static event Action noHype;

	public static event Action ExplosionCameraPushBack;
	public static event Action BonusCameraPushBack;
	
	public static void InvokeExplosion() => explosion?.Invoke();
	public static void InvokeGetHyped() => getHyped?.Invoke();
	public static void InvokeNoHype() => noHype?.Invoke();

	public static void InvokeExplosionCameraPushBack() => ExplosionCameraPushBack?.Invoke();
	public static void InvokeBonusCameraPushBack() => BonusCameraPushBack?.Invoke();
}
