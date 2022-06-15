using Kart;

namespace StateMachine
{
	public class TrackStateBase : InputStateBase
	{
		protected static KartTrackMovement Player { get; private set; }
		
		protected TrackStateBase() { }
		public TrackStateBase(KartTrackMovement player) => Player = player;

		protected static float BasicDecelerate() => Player.BasicDecelerate();
		protected static void CalculateForces(float dotPercent) => Player.CalculateForces(dotPercent);

		protected static void CalculateBrakingForces() => Player.CalculateBraking();
	}
	
	public sealed class IdleOnTrackState : TrackStateBase
	{
		public override void Execute()
		{
			Player.BasicDecelerate();
			
			Player.Brake();

			CalculateBrakingForces();
		}
	}

	public sealed class MoveOnTrackState : TrackStateBase
	{
		public override void OnEnter() => Player.StartFollow();
		public override void Execute()
		{
			CalculateForces(BasicDecelerate());
			
			Player.Accelerate();

			CalculateBrakingForces();
		}
	}
}