using System;

namespace MarvinsArena.Robot.Core
{
	public class CoreScannedRobotEventArgs : EventArgs
	{
		public double PositionX { get; set; }
		public double PositionY { get; set; }

		public CoreScannedRobotEventArgs(double positionX, double positionY)
		{
			PositionX = positionX;
			PositionY = positionY;
		}
	}

	[Serializable]
	public class CoreMessageReceivedFromTeammateEventArgs : EventArgs
	{
		public int Team { get; set; }
		public int SquadNumber { get; set; }
		public string Message { get; set; }

		public CoreMessageReceivedFromTeammateEventArgs(int team, int squadNumber, string message)
		{
			Team = team;
			SquadNumber = squadNumber;
			Message = message;
		}
	}

	public class RobotCore
	{
		private const double TwoPI = 2.0 * Math.PI;
		private const double robotMoveSpeed = 0.04;
		private const double robotRotateSpeed = 0.00069813170079773183076947630739545;//0.04;
		private const double robotRotateGunSpeed = 0.00069813170079773183076947630739545;
		//private const double robotRotateRadarSpeed = 0.00069813170079773183076947630739545;
		private const double robotRotateRadarSpeed = 0.0004813170079773183076947630739545;

		private string name;

		private DateTime lastMissileFired;
		private DateTime lastBulletFired;
		private DateTime coolDownTimer;

		private double lastPositionX;
		private double lastPositionY;
		private MessageExchangeFromRobotEventArgs robotHostMessageExchange;

		public double PositionX { get; set; }
		public double PositionY { get; set; }
		public double Rotation { get; set; }
		public double RotationGun { get; set; }
		public double RotationRadar { get; set; }

		public double RemainingDistance { get; private set; }
		public double RemainingRotation { get; private set; }
		public double RemainingRotationGun { get; private set; }
		public double RemainingRotationRadar { get; private set; }

		public int Hitpoints { get; private set; }
		public int Missiles { get; private set; }
		public int MaxHeat { get; private set; }
		public int Heat { get; private set; }

		public static int Height { get { return 24; } }
		public static int Width { get { return 24; } }
		public static int Center { get { return 12; } }

		public int SquadNumber { get; private set; }
		public int Team { get; private set; }

		public event EventHandler<EventArgs> RoundStarted;
		public event EventHandler<EventArgs> HitWall;
		public event EventHandler<EventArgs> HitBullet;
		public event EventHandler<EventArgs> HitMissile;
		public event EventHandler<EventArgs> HitRobot;
		public event EventHandler<CoreScannedRobotEventArgs> ScannedRobot;
		public event EventHandler<CoreMessageReceivedFromTeammateEventArgs> MessageReceivedFromTeammate;

		public RobotCore(string name, int squadNumber, int team)
		{
			robotHostMessageExchange = new MessageExchangeFromRobotEventArgs();

			this.name = name;
			this.SquadNumber = squadNumber;
			this.Team = team;			
		}

		/// <summary>
		/// Set all values required for each round
		/// </summary>
		public void SetRoundValues(int hitpoints, int missiles, int maxHeat)
		{
			this.Hitpoints = hitpoints;
			this.Missiles = missiles;
			this.MaxHeat = maxHeat;
			this.Heat = 0;
			
			lastMissileFired = DateTime.Now - new TimeSpan(0, 0, 10); // Allow to shoot directly
			lastBulletFired = DateTime.Now - new TimeSpan(0, 0, 10);
			coolDownTimer = DateTime.Now;

			// Notify robot on new round
			if (RoundStarted != null)
			{
				RoundStarted(name, EventArgs.Empty);
			}
		}

		public MessageExchangeFromRobotEventArgs Update(double deltaTime)
		{
			// Normalize rotations
			while(Rotation > Math.PI) Rotation -= TwoPI;
			while(Rotation < -Math.PI) Rotation += TwoPI;
			while(RotationGun > Math.PI) RotationGun -= TwoPI;
			while(RotationGun < -Math.PI) RotationGun += TwoPI;
			while(RotationRadar > Math.PI) RotationRadar -= TwoPI;
			while(RotationRadar < -Math.PI) RotationRadar += TwoPI;

			// Rotate body
			double rotationdeg = robotRotateSpeed * deltaTime;
			if(RemainingRotation < 0)
				rotationdeg *= -1.0f; // Rotate right
			if(Math.Abs(RemainingRotation) - rotationdeg <= 0)
				rotationdeg = RemainingRotation;
			Rotation += rotationdeg;
			RemainingRotation -= rotationdeg;

			// Rotate gun
			double rotationdegGun = robotRotateGunSpeed * deltaTime;
			if(RemainingRotationGun < 0)
				rotationdegGun *= -1.0f; // Rotate right
			if(Math.Abs(RemainingRotationGun) - rotationdegGun <= 0)
				rotationdegGun = RemainingRotationGun;
			RotationGun += rotationdegGun;
			RemainingRotationGun -= rotationdegGun;

			// Rotate radar
			double rotationdegRadar = robotRotateRadarSpeed * deltaTime;
			if(RemainingRotationRadar < 0)
				rotationdegRadar *= -1.0f; // Rotate right
			if(Math.Abs(RemainingRotationRadar) - rotationdegRadar <= 0)
				rotationdegRadar = RemainingRotationRadar;
			RotationRadar += rotationdegRadar;
			RemainingRotationRadar -= rotationdegRadar;

			// Store last position
			lastPositionX = PositionX;
			lastPositionY = PositionY;
			// Move robot
			double movedistance = robotMoveSpeed * deltaTime;
			if(RemainingDistance < 0)
				movedistance *= -1.0f; // Move Backwards
			if(Math.Abs(RemainingDistance) - movedistance <= 0)
				movedistance = RemainingDistance;
			PositionX += movedistance * Math.Cos(Rotation);
			PositionY += movedistance * Math.Sin(Rotation);
			RemainingDistance -= movedistance;

			// Cool down gun
			if((DateTime.Now - coolDownTimer).TotalSeconds > 1)
			{
				Heat--;
				if(Heat < 0)
					Heat = 0;
				coolDownTimer = DateTime.Now;
			}

			// Notify host about changes
			MessageExchangeFromRobotEventArgs retval = (MessageExchangeFromRobotEventArgs)robotHostMessageExchange.Clone();
			robotHostMessageExchange = new MessageExchangeFromRobotEventArgs();
			return retval;
		}

		public void Notify(MessageExchangeFromHost hostMessage)
		{
			foreach(Collision col in hostMessage.Collisions)
			{
				Hitpoints -= col.Damage;
				if(Hitpoints < 0)
					Hitpoints = 0;

				switch(col.MsgExchangeType)
				{
					case MessageExchangeType.Bullet:
						if(HitBullet != null)
						{
							HitBullet(name, EventArgs.Empty);
						}
						break;
					case MessageExchangeType.Missile:
						if(HitMissile != null)
						{
							HitMissile(name, EventArgs.Empty);
						}
						break;
					case MessageExchangeType.Robot:
						RemainingDistance = 0;
						PositionX = lastPositionX;
						PositionY = lastPositionY;
						if(HitRobot != null)
						{
							HitRobot(name, EventArgs.Empty);
						}
						break;
					case MessageExchangeType.Wall:
						RemainingDistance = 0;
						PositionX = lastPositionX;
						PositionY = lastPositionY;
						if(HitWall != null)
						{
							HitWall(name, EventArgs.Empty);
						}
						break;
				}
			}

			foreach(ScannerTarget target in hostMessage.Targets)
			{
				if(ScannedRobot != null)
				{
					CoreScannedRobotEventArgs args = new CoreScannedRobotEventArgs(target.PositionX, target.PositionY);
					ScannedRobot(name, args);
				}
			}

			foreach(CoreMessageReceivedFromTeammateEventArgs target in hostMessage.MessageReceivedFromTeammate)
			{
				if(MessageReceivedFromTeammate != null)
				{
					MessageReceivedFromTeammate(name, target);
				}
			}
		}

		public void MoveForward(double distance)
		{
			RemainingDistance = distance;
		}

		public void MoveBackward(double distance)
		{
			MoveForward(-distance);
		}

		public void RotateLeft(double rad)
		{
			RemainingRotation = -rad;
		}

		public void RotateRight(double rad)
		{
			RemainingRotation = rad;
		}

		public void RotateGunLeft(double rad)
		{
			RemainingRotationGun = -rad;
		}

		public void RotateGunRight(double rad)
		{
			RemainingRotationGun = rad;
		}

		public void RotateRadarLeft(double rad)
		{
			RemainingRotationRadar = -rad;
		}

		public void RotateRadarRight(double rad)
		{
			RemainingRotationRadar = rad;
		}

		public bool FireBullet(int power)
		{
			if((DateTime.Now - lastBulletFired).TotalSeconds < 2)
				return false;

			lastBulletFired = DateTime.Now;

			if(power > MaxHeat - Heat)
				return false;// Don't fire if too hot //Power = maxHeat - heat;
			if(power < 1)
				return false;
			Heat += power;

			robotHostMessageExchange.FireProjectile = new FireProjectile(ProjectileType.Bullet, power);
			return true;
		}

		public bool FireMissile()
		{
			// No more missile
			if(Missiles < 1)
				return false;

			if((DateTime.Now - lastMissileFired).TotalSeconds < 6)
				return false;

			lastMissileFired = DateTime.Now;

			Missiles--;
	
			robotHostMessageExchange.FireProjectile = new FireProjectile(ProjectileType.Missile);
			return true;
		}

		public void Print(string message)
		{
			robotHostMessageExchange.PrintMessage.Add(message);
		}

		public void PrintDebug(string message)
		{
			robotHostMessageExchange.DebugMessage.Add(message);
		}

		public void SendMessageToTeam(string message)
		{
			CoreMessageReceivedFromTeammateEventArgs tm = new CoreMessageReceivedFromTeammateEventArgs(Team, SquadNumber, message);
			robotHostMessageExchange.TeamMessage.Add(tm);
		}
	}
}
