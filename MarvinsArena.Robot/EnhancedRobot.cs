using System;

namespace MarvinsArena.Robot
{
	/// <summary>
	/// Defines the basic methods for creating a enhanced robot. A robot must derive from the <seealso cref="T:MarvinsArena.Robot.IRobot">IRobot</seealso> interface as well.
	/// </summary>
	public class EnhancedRobot : BaseRobot
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

		/// <summary>
		/// Gets the width of the map in fields. This is equal to MapWidth / MapScale;
		/// </summary>
		public int MapWidthFields { get { return gameBase.MapFieldsWidth; } }

		/// <summary>
		/// Gets the height of the map in fields. This is equal to MapHeight / MapScale;
		/// </summary>
		public int MapHeightFields { get { return gameBase.MapFieldsHeight; } }

		/// <summary>
		/// Gets the size of the map blocks.
		/// </summary>
		public int MapScale { get { return 32; } }

		/// <summary>
		/// Check if the specified field is accessible by the robot.
		/// </summary>
		/// <param name="indexX">Field index X</param>
		/// <param name="indexY">Field index Y</param>
		/// <returns>0 if the field is free, 1 if the field is blocked and -1 in case the indices are out of range.</returns>
		public int MapCanEnter(int indexX, int indexY)
		{
			return gameBase.MapCanEnter(indexX, indexY);
		}
	}
}
