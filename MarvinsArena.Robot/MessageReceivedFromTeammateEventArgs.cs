using System;

namespace MarvinsArena.Robot
{
	/// <summary>
	/// Contains information on messages received from a teammate.
	/// </summary>
	public class MessageReceivedFromTeammateEventArgs : EventArgs
	{
		/// <summary>
		/// The sender of the message.
		/// </summary>
		public int Team { get; set; }

		/// <summary>
		/// The sender of the message.
		/// </summary>
		public int SquadNumber { get; set; }
		
		/// <summary>
		/// The message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Initalizes a new instance of the ScannedRobotEventArgs class.
		/// </summary>
		/// <param name="team">The team id</param>
		/// <param name="squadNumber">The players number within the team</param>
		/// <param name="message">The message</param>
		public MessageReceivedFromTeammateEventArgs(int team, int squadNumber, string message)
		{
			Team = team;
			SquadNumber = squadNumber;
			Message = message;
		}
	}
}
