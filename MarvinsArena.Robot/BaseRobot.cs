using System;

namespace MarvinsArena.Robot
{
	/// <summary>
	/// Defines the basic methods for creating a robot. A robot must derive from the <seealso cref="T:MarvinsArena.Robot.IRobot">IRobot</seealso> interface as well.
	/// </summary>
	public class BaseRobot
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
		public void InternalSetCoreRobot(MarvinsArena.Robot.Core.RobotCore robotBase)
		{
			this.robotBase = robotBase;

			this.robotBase.RoundStarted += RobotBase_RoundStarted;
			this.robotBase.HitBullet += RobotBase_HitBullet;
			this.robotBase.HitMissile += RobotBase_HitMissile;
			this.robotBase.HitRobot += RobotBase_HitRobot;
			this.robotBase.HitWall += RobotBase_HitWall;
			this.robotBase.ScannedRobot += RobotBase_ScannedRobot;
		}

		/// <summary>
		/// This method is internally called by the engine. You must not call it!
		/// </summary>
		/// <param name="gameBase">Reference to game core</param>
		public void InternalSetCoreGame(MarvinsArena.Robot.Core.GameCore gameBase)
		{
			this.gameBase = gameBase;
		}

		#region Internal event handler		
		/// <summary>
		/// Internal object for event handling.
		/// </summary>
		/// <param name="sender">The event source</param>
		/// <param name="e">The event arguments</param>
		private void RobotBase_RoundStarted(object sender, EventArgs e)
		{
			if (RoundStarted != null)
				RoundStarted(sender, e);
		}

		/// <summary>
		/// Internal object for event handling.
		/// </summary>
		/// <param name="sender">The event source</param>
		/// <param name="e">The event arguments</param>
		private void RobotBase_HitBullet(object sender, EventArgs e)
		{
			if(HitBullet != null)
				HitBullet(sender, e);
		}

		/// <summary>
		/// Internal object for event handling.
		/// </summary>
		/// <param name="sender">The event source</param>
		/// <param name="e">The event arguments</param>
		private void RobotBase_HitMissile(object sender, EventArgs e)
		{
			if(HitMissile != null)
				HitMissile(sender, e);
		}

		/// <summary>
		/// Internal object for event handling.
		/// </summary>
		/// <param name="sender">The event source</param>
		/// <param name="e">The event arguments</param>
		private void RobotBase_HitRobot(object sender, EventArgs e)
		{
			if(HitRobot != null)
				HitRobot(sender, e);
		}

		/// <summary>
		/// Internal object for event handling.
		/// </summary>
		/// <param name="sender">The event source</param>
		/// <param name="e">The event arguments</param>
		private void RobotBase_HitWall(object sender, EventArgs e)
		{
			if(HitWall != null)
				HitWall(sender, e);
		}

		/// <summary>
		/// Internal object for event handling.
		/// </summary>
		/// <param name="sender">The event source</param>
		/// <param name="e">The event arguments</param>
		private void RobotBase_ScannedRobot(object sender, MarvinsArena.Robot.Core.CoreScannedRobotEventArgs e)
		{
			if(ScannedRobot != null)
			{
				ScannedRobotEventArgs args = new ScannedRobotEventArgs(e.PositionX, e.PositionY);
				ScannedRobot(sender, args);
			}
		}
		#endregion

		/// <summary>
		/// Gets the robots X position. The origin is the center of the robot.
		/// </summary>
		public double PositionX { get { return robotBase.PositionX; } }

		/// <summary>
		/// Gets the robots Y position. The origin is the center of the robot.
		/// </summary>
		public double PositionY { get { return robotBase.PositionY; } }

		/// <summary>
		/// Gets the robots rotation in radiant
		/// </summary>
		public double Rotation { get { return robotBase.Rotation; } }

		/// <summary>
		/// Gets the robots rotation in degree
		/// </summary>
		public double RotationDeg { get { return RadToDeg(robotBase.Rotation); } }

		/// <summary>
		/// Gets the robots gun rotation in radiant
		/// </summary>
		public double RotationGun { get { return robotBase.RotationGun; } }

		/// <summary>
		/// Gets the robots gun rotation in degree
		/// </summary>
		public double RotationGunDeg { get { return RadToDeg(robotBase.RotationGun); } }

		/// <summary>
		/// Gets the robots radar rotation in radiant
		/// </summary>
		public double RotationRadar { get { return robotBase.RotationRadar; } }

		/// <summary>
		/// Gets the robots radar rotation in degree
		/// </summary>
		public double RotationRadarDeg { get { return RadToDeg(robotBase.RotationRadar); } }

		/// <summary>
		/// Gets the distance the robot needs to cover until it stops.
		/// </summary>
		public double RemainingDistance { get { return robotBase.RemainingDistance; } }

		/// <summary>
		/// Gets the angle the robot needs to rotate until it stops.
		/// </summary>
		public double RemainingRotation { get { return robotBase.RemainingRotation; } }

		/// <summary>
		/// Gets the angle the robots gun needs to rotate until it stops.
		/// </summary>
		public double RemainingRotationGun { get { return robotBase.RemainingRotationGun; } }

		/// <summary>
		/// Gets the angle the robots radar needs to rotate until it stops.
		/// </summary>
		public double RemainingRotationRadar { get { return robotBase.RemainingRotationRadar; } }

		/// <summary>
		/// Gets the width of the map in units.
		/// </summary>
		/// <example>This sample shows how to detect a collision.
		/// <code><![CDATA[
		/// if(PositionX < Radius ||
		/// 	PositionX > MapWidth - Radius ||
		/// 	PositionY < Radius||
		/// 	PositionY > MapHeight - Radius)
		/// {
		///		// The robot hits a wall
		/// }]]>
		/// </code>
		/// </example>
		public int MapWidth { get { return gameBase.MapWidth; } }

		/// <summary>
		/// Gets the height of the map in units.
		/// </summary>
		/// <example>This sample shows how to detect a collision.
		/// <code><![CDATA[
		/// if(PositionX < Radius ||
		/// 	PositionX > MapWidth - Radius ||
		/// 	PositionY < Radius||
		/// 	PositionY > MapHeight - Radius)
		/// {
		///		// The robot hits a wall
		/// }]]>
		/// </code>
		/// </example>
		public int MapHeight { get { return gameBase.MapHeight; } }

		/// <summary>
		/// Gets the current hitpoints.
		/// </summary>
		public int Hitpoints { get { return robotBase.Hitpoints; } }

		/// <summary>
		/// Gets the current amount of missiles.
		/// </summary>
		public int Missiles { get { return robotBase.Missiles; } }

		/// <summary>
		/// Gets the maximum heat of the gun. If the gun is too hot it is not possible to fire bullets.
		/// </summary>
		public int MaximumHeat { get { return robotBase.MaxHeat; } }

		/// <summary>
		/// Gets the current heat. This can never be greter than <seealso cref="MaximumHeat"/>.
		/// </summary>
		public int Heat { get { return robotBase.Heat; } }

		/// <summary>
		/// Gets the height of the robot. This is equal to the height.
		/// </summary>
		public int Height { get { return MarvinsArena.Robot.Core.RobotCore.Height; } }

		/// <summary>
		/// Gets the width of the robot. This is equal to the width.
		/// </summary>
		public int Width { get { return MarvinsArena.Robot.Core.RobotCore.Width; } }

		/// <summary>
		/// Gets the center of the robots "square". This is half the width and height.
		/// </summary>
		public int Center { get { return MarvinsArena.Robot.Core.RobotCore.Center; } }

		/// <summary>
		/// Gets the radius of the collision circle. This is equal to the <seealso cref="Center"/>.
		/// </summary>
		public int Radius { get { return MarvinsArena.Robot.Core.RobotCore.Center; } }

		/// <summary>
		/// The event handler is called every time a new round is started.
		/// This is a good place to (re)set variables.
		/// </summary>
		public event EventHandler<EventArgs> RoundStarted;

		/// <summary>
		/// The event handler is called if the robot hits a wall.
		/// </summary>
		public event EventHandler<EventArgs> HitWall;

		/// <summary>
		/// The event handler is called if the robot is hit by a bullet.
		/// </summary>
		public event EventHandler<EventArgs> HitBullet;

		/// <summary>
		/// The event handler is called if the robot is hit by a missile.
		/// </summary>
		public event EventHandler<EventArgs> HitMissile;

		/// <summary>
		/// The event handler is called if the robot hits or is hit by another robot.
		/// </summary>
		public event EventHandler<EventArgs> HitRobot;

		/// <summary>
		/// The event handler is called if another robot is within the scanner range.
		/// This event will get raised until the robot leaves the scanner range.
		/// </summary>
		public event EventHandler<ScannedRobotEventArgs> ScannedRobot;

		/// <summary>
		/// Set the distance the robot should travel.
		/// </summary>
		/// <param name="units">Distance to add</param>
		public void MoveForward(int units)
		{
			if(robotBase != null)
				robotBase.MoveForward(units);
		}

		/// <summary>
		/// Set the distance the robot should travel.
		/// </summary>
		/// <param name="units">Distance to add</param>
		public void MoveBackward(int units)
		{
			if(robotBase != null)
				robotBase.MoveBackward(units);
		}

		/// <summary>
		/// Added for convenience. Stops the movement. This is equal to Move*(0).
		/// </summary>
		public void StopMove()
		{
			if(robotBase != null)
				robotBase.MoveForward(0);
		}

		/// <summary>
		/// Set the angle the robot should rotate.
		/// </summary>
		/// <param name="rad">Angle to rotate in radiant</param>
		public void RotateLeft(double rad)
		{
			if(robotBase != null)
				robotBase.RotateLeft(rad);
		}

		/// <summary>
		/// Set the angle the robot should rotate.
		/// </summary>
		/// <param name="deg">Angle to rotate in degree</param>
		public void RotateLeftDeg(double deg)
		{
			double rad = DegToRad(deg);
			RotateLeft(rad);
		}

		/// <summary>
		/// Set the angle the robot should rotate.
		/// </summary>
		/// <param name="rad">Angle to rotate in radiant</param>
		public void RotateRight(double rad)
		{
			if(robotBase != null)
				robotBase.RotateRight(rad);
		}

		/// <summary>
		/// Set the angle the robot should rotate.
		/// </summary>
		/// <param name="deg">Angle to rotate in degree</param>
		public void RotateRightDeg(double deg)
		{
			double rad = DegToRad(deg);
			RotateRight(rad);
		}

		/// <summary>
		/// Added for convenience. Stops the rotation. This is equal to Rotate*(0).
		/// </summary>
		public void StopRotate()
		{
			if(robotBase != null)
				robotBase.RotateRight(0);
		}

		/// <summary>
		/// Set the angle the robots gun should rotate.
		/// </summary>
		/// <param name="rad">Angle to rotate in radiant</param>
		public void RotateGunLeft(double rad)
		{
			if(robotBase != null)
				robotBase.RotateGunLeft(rad);
		}

		/// <summary>
		/// Set the angle the robots gun should rotate.
		/// </summary>
		/// <param name="deg">Angle to rotate in degree</param>
		public void RotateGunLeftDeg(double deg)
		{
			double rad = DegToRad(deg);
			RotateGunLeft(rad);
		}

		/// <summary>
		/// Set the angle the robots gun should rotate.
		/// </summary>
		/// <param name="rad">Angle to rotate in radiant</param>
		public void RotateGunRight(double rad)
		{
			if(robotBase != null)
				robotBase.RotateGunRight(rad);
		}

		/// <summary>
		/// Set the angle the robots gun should rotate.
		/// </summary>
		/// <param name="deg">Angle to rotate in degree</param>
		public void RotateGunRightDeg(double deg)
		{
			double rad = DegToRad(deg);
			RotateGunRight(rad);
		}

		/// <summary>
		/// Added for convenience. Stops the guns rotation. This is equal to RotateGun*(0).
		/// </summary>
		public void StopRotateGun()
		{
			if(robotBase != null)
				robotBase.RotateGunRight(0);
		}

		/// <summary>
		/// Set the angle the robots radar should rotate.
		/// </summary>
		/// <param name="rad">Angle to rotate in radiant</param>
		public void RotateRadarLeft(double rad)
		{
			if(robotBase != null)
				robotBase.RotateRadarLeft(rad);
		}

		/// <summary>
		/// Set the angle the robots radar should rotate.
		/// </summary>
		/// <param name="deg">Angle to rotate in degree</param>
		public void RotateRadarLeftDeg(double deg)
		{
			double rad = DegToRad(deg);
			RotateRadarLeft(rad);
		}

		/// <summary>
		/// Increase the angle the robots radar will rotate.
		/// </summary>
		/// <param name="rad">Angle to rotate in radiant</param>
		public void RotateRadarRight(double rad)
		{
			if(robotBase != null)
				robotBase.RotateRadarRight(rad);
		}

		/// <summary>
		/// Increase the angle the robots radar will rotate.
		/// </summary>
		/// <param name="deg">Angle to rotate in degree</param>
		public void RotateRadarRightDeg(double deg)
		{
			double rad = DegToRad(deg);
			RotateRadarRight(rad);
		}

		/// <summary>
		/// Added for convenience. Stops the radar rotation. This is equal to RotateRadar*(0).
		/// </summary>
		public void StopRotateRadar()
		{
			if(robotBase != null)
				robotBase.RotateRadarRight(0);
		}

		/// <summary>
		/// Fire a bullet with specified power.
		/// The power specifies the amount of damage the bullet will do as well as the heat required to launch it.
		/// The power must be less than the difference of heat and maximum heat.
		/// The power must be one or bigger.
		/// A bullet can only be shoot every two seconds.
		/// </summary>
		/// <param name="power">The energy of the bullet</param>
		/// <returns>True if the bullet was fired</returns>
		public bool FireBullet(int power)
		{
			if(robotBase != null)
				return robotBase.FireBullet(power);

			return false;
		}

		/// <summary>
		/// Fire a missile if the rocket launcher is not empty.
		/// A missile can only be shoot every six seconds.
		/// </summary>
		/// <returns>True if the missile was fired</returns>
		public bool FireMissile()
		{
			if(robotBase != null)
				return robotBase.FireMissile();

			return false;
		}

		/// <summary>
		/// Let the robot say a message.
		/// </summary>
		/// <param name="message">The text to send</param>
		public void Print(string message)
		{
			if(robotBase != null)
				robotBase.Print(message);
		}

		/// <summary>
		/// Print a debug message. The message is only shown if the battle engine was started using "-debug".
		/// </summary>
		/// <param name="message">The information to send</param>
		public void PrintDebug(string message)
		{
			if(robotBase != null)
				robotBase.PrintDebug(message);
		}

		/// <summary>
		/// Convert radians to degrees
		/// </summary>
		/// <param name="rad">Radiant to convert</param>
		/// <returns>Degrees</returns>
		public static double RadToDeg(double rad)
		{
			return rad * 180.0 / Math.PI;
		}

		/// <summary>
		/// Convert degrees to radians
		/// </summary>
		/// <param name="deg">Degrees to convert</param>
		/// <returns>Radiant</returns>
		public static double DegToRad(double deg)
		{
			deg = deg % 360;
			////double t = 2 * Math.PI * (-deg / 360.0);
			return (Math.PI / 180) * deg;
		}
	}
}