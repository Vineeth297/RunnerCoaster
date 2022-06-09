using Player;

namespace StateMachine
{
	public class FlyingStateBase : InputStateBase
	{
		protected static PlayerFlyMovement Fly;

		protected FlyingStateBase() { }
		public FlyingStateBase(PlayerFlyMovement fly) => Fly = fly;

		public override void Execute()
		{
			Fly.CalculateForwardMovement();
			Fly.CalculateDownwardMovement();
			Fly.ApplyMovement();
		}
	}
	
	public sealed class FallingFlyingState : FlyingStateBase { }

	public sealed class ForwardFlyingState : FlyingStateBase
	{
		public override void OnEnter() => Fly.SetForwardOrientedValues();

		public override void OnExit() => Fly.SetDownwardOrientedValues();
	}
}