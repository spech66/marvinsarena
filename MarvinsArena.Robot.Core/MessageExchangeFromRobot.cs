using System;
using System.Collections.Generic;

namespace MarvinsArena.Robot.Core
{		
	public enum ProjectileType
	{
		Bullet,
		Missile
	}

	[Serializable]
	public class FireProjectile
	{
		public ProjectileType FireType { get; set; }
		public int Power { get; set; }

		public FireProjectile(ProjectileType projectileType)
		{
			FireType = projectileType;
			Power = 1;
		}

		public FireProjectile(ProjectileType projectileType, int power)
		{
			FireType = projectileType;
			Power = power;
		}
	}

	/// <summary>
	/// Class to notify host about changes.
	/// </summary>
	[Serializable]
	public class MessageExchangeFromRobotEventArgs : EventArgs, ICloneable
	{
		public FireProjectile FireProjectile { get; set; }
		public List<string> PrintMessage { get; set; }
		public List<string> DebugMessage { get; set;}
		public List<CoreMessageReceivedFromTeammateEventArgs> TeamMessage { get; set; }

		public MessageExchangeFromRobotEventArgs()
		{
			PrintMessage = new List<string>();
			DebugMessage = new List<string>();
			TeamMessage = new List<CoreMessageReceivedFromTeammateEventArgs>();
		}

		public object Clone()
		{
			MessageExchangeFromRobotEventArgs exchange = new MessageExchangeFromRobotEventArgs();
			exchange.FireProjectile = FireProjectile;
			exchange.PrintMessage = PrintMessage;
			exchange.DebugMessage = DebugMessage;
			exchange.TeamMessage = TeamMessage;
			return exchange;
		}
	}
}
