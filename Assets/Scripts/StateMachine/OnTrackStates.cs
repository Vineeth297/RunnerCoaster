using Kart;

namespace StateMachine
{
	public class TrackStateBase : InputStateBase
	{
		protected static KartTrackMovement Player { get; private set; }
		
		protected TrackStateBase() { }
		public TrackStateBase(KartTrackMovement player) => Player = player;

		protected static void CalculateForces() => Player.CalculateForces();

		protected static void CalculateBrakingForces() => Player.CalculateBraking();
	}
	
	public sealed class IdleOnTrackState : TrackStateBase
	{
		public override void Execute()
		{
			//CalculateForces();
			
			Player.Brake();

			CalculateBrakingForces();
		}
	}

	public sealed class MoveOnTrackState : TrackStateBase
	{
		public override void OnEnter() => Player.StartFollow();
		public override void Execute()
		{
			CalculateForces();
			
			Player.Accelerate();

			CalculateBrakingForces();
		}
	}
}