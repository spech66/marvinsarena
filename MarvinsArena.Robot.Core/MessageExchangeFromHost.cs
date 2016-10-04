using System;
using System.Collections.ObjectModel;

namespace MarvinsArena.Robot.Core
{
	public enum MessageExchangeType
	{
		Robot,
		Bullet,
		Missile,
		Wall
	}

	[Serializable]
	public class Collision
	{
		public string Source { get; set; }
		public int Damage { get; set; }
		public MessageExchangeType MsgExchangeType { get; set; }

		public Collision(string source, int damage, MessageExchangeType type)
		{
			this.Source = source;
			this.Damage = damage;
			this.MsgExchangeType = type;
		}
	}

	[Serializable]
	public class ScannerTarget
	{
		public double PositionX { get; set; }
		public double PositionY { get; set; }
		public string Source { get; set; }
		public MessageExchangeType MsgExchangeType { get; set; }

		public ScannerTarget(double positionX, double positionY, string source, MessageExchangeType type)
		{
			this.PositionX = positionX;
			this.PositionY = positionY;
			this.Source = source;
			this.MsgExchangeType = type;
		}
	}

	/// <summary>
	/// Class to handle messages send from the engine to the robot.
	/// </summary>
	[Serializable]
	public class MessageExchangeFromHost
	{
		public Collection<Collision> Collisions { get; set; }
		public Collection<ScannerTarget> Targets { get; set; }
		public Collection<CoreMessageReceivedFromTeammateEventArgs> MessageReceivedFromTeammate { get; set; }

		public MessageExchangeFromHost()
		{
			Collisions = new Collection<Collision>();
			Targets = new Collection<ScannerTarget>();
			MessageReceivedFromTeammate = new Collection<CoreMessageReceivedFromTeammateEventArgs>();
		}

		/// <summary>
		/// Set all values to default. Call this after notifying robot.
		/// </summary>
		public void Reset()
		{
			Collisions.Clear();
			Targets.Clear();
			MessageReceivedFromTeammate.Clear();
		}
	}
}
