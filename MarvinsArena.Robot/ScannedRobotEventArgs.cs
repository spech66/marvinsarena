using System;

namespace MarvinsArena.Robot
{
	/// <summary>
	/// Contains information on scanned robots.
	/// </summary>
	public class ScannedRobotEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the x position of scanned robot
		/// </summary>
		public double PositionX { get; set; }

		/// <summary>
		/// Gets or sets the y position of scanned robot
		/// </summary>
		public double PositionY { get; set; }

		/// <summary>
		/// Initalizes a new instance of the ScannedRobotEventArgs class.
		/// </summary>
		/// <param name="positionX">X Position of scanned robot</param>
		/// <param name="positionY">Y Position of scanned robot</param>
		public ScannedRobotEventArgs(double positionX, double positionY)
		{
			PositionX = positionX;
			PositionY = positionY;
		}
	}
}
