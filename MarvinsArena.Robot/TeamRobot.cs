using System;

namespace MarvinsArena.Robot
{
    /// <summary>
	/// Defines the basic methods for creating a team robot. A robot must derive from the <seealso cref="T:MarvinsArena.Robot.IRobot">IRobot</seealso> interface as well.
	/// </summary>
	public class TeamRobot : EnhancedRobot
	{
		/// <summary>
		/// Internal object.
		/// </summary>
		private MarvinsArena.Robot.Core.RobotCore robotBase;

		/// <summary>
		/// Internal object.
		/// </summary>
		private MarvinsArena.Robot.Core.GameCore gameBase;

		/// <summary>
		/// This method is internally called by the engine. You must not call it!
		/// </summary>
		/// <param name="robotBase">Reference to robot core</param>
		public new void InternalSetCoreRobot(MarvinsArena.Robot.Core.RobotCore robotBase)
		{
			this.robotBase = robotBase;
			base.InternalSetCoreRobot(robotBase);

			this.robotBase.MessageReceivedFromTeammate += TeamRobot_MessageReceivedFromTeammate;
		}

		/// <summary>
		/// This method is internally called by the engine. You must not call it!
		/// </summary>
		/// <param name="gameBase">Reference to game core</param>
		public new void InternalSetCoreGame(MarvinsArena.Robot.Core.GameCore gameBase)
		{
			this.gameBase = gameBase;
			base.InternalSetCoreGame(gameBase);
		}

		#region Internal event handler
		/// <summary>
		/// Internal object for event handling.
		/// </summary>
		/// <param name="sender">The event source</param>
		/// <param name="e">The event arguments</param>
		private void TeamRobot_MessageReceivedFromTeammate(object sender,
			MarvinsArena.Robot.Core.CoreMessageReceivedFromTeammateEventArgs e)
		{
			if(MessageReceivedFromTeammate != null)
			{
				MessageReceivedFromTeammateEventArgs args = new MessageReceivedFromTeammateEventArgs(e.Team, e.SquadNumber, e.Message);
				MessageReceivedFromTeammate(sender, args);
			}
		}
		#endregion

		/// <summary>
		/// Gets the robots number within the team.
		/// </summary>
		public int SquadNumber { get { return robotBase.SquadNumber; } }

		/// <summary>
		/// Gets the team number.
		/// </summary>
		public int Team { get { return robotBase.Team; } }

		/// <summary>
		/// The event handler is called if the robot receives a message from a teammate.
		/// </summary>
		public event EventHandler<MessageReceivedFromTeammateEventArgs> MessageReceivedFromTeammate;

		/// <summary>
		/// Send a message to the team not including self.
		/// </summary>
		/// <param name="message">The message to send</param>
		public void SendMessageToTeam(string message)
		{
			if(robotBase != null)
				robotBase.SendMessageToTeam(message);
		}
	}
}
