using System;
using System.Reflection;

namespace MarvinsArena.Core
{
	public enum LoadedRobotType
	{
		Robot,
		EnhancedRobot,
		TeamRobot
	}

	public class RobotHost : MarshalByRefObject
	{
		/// <summary>
		/// Limit call to run to every (RunLimit)th execution.
		/// </summary>
		private const int RunLimit = 4;

		private Assembly assembly;
		private MarvinsArena.Robot.IRobot irobot;
		private MarvinsArena.Robot.BaseRobot robot;
		private MarvinsArena.Robot.Core.RobotCore robotCore;
		private MarvinsArena.Robot.Core.GameCore gameCore;
		private MarvinsArena.Robot.Core.MessageExchangeFromHost messageExchangeFromHost;
		private int runLimiter;

		public LoadedRobotType RobotType { get; private set; }
		public string Name { get; private set; }
		public int SquadNumber { get; private set; }
		public int Team { get; private set; }
		public double PositionX { get { return robotCore.PositionX; } set { robotCore.PositionX = value; } }
		public double PositionY { get { return robotCore.PositionY; } set { robotCore.PositionY = value; } }
		public double Rotation { get { return robotCore.Rotation; } set { robotCore.Rotation = value; } }
		public double RotationGun { get { return robotCore.RotationGun; } set { robotCore.RotationGun = value; } }
		public double RotationRadar { get { return robotCore.RotationRadar; } set { robotCore.RotationRadar = value; } }
		public int Hitpoints { get { return robotCore.Hitpoints; } }
		public int Missiles { get { return robotCore.Missiles; } }
		public int Heat { get { return robotCore.Heat; } }
		public bool IsActive { get; private set; }
		public string DisabledMessage { get; private set; }

		public RobotHost()
		{
			//Console.WriteLine(AppDomain.CurrentDomain);
			messageExchangeFromHost = new MarvinsArena.Robot.Core.MessageExchangeFromHost();
			IsActive = true;
			DisabledMessage = String.Empty;
		}

		public void LoadAssembly(string name, int squadNumber, int team, byte[][] map)
		{
			assembly = Assembly.Load(name);
			Name = AssemblyTitle;
			this.SquadNumber = squadNumber;
			this.Team = team;

			Type[] types = assembly.GetTypes();

			foreach(Type t in types)
			{
				// This will get the first found MsgExchangeType - therefore no more than one MsgExchangeType should be in the plugin
				if(t.BaseType == typeof(MarvinsArena.Robot.BaseRobot) ||
					t.BaseType == typeof(MarvinsArena.Robot.EnhancedRobot) ||
					t.BaseType == typeof(MarvinsArena.Robot.TeamRobot))
				{
					ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
					object o = ci.Invoke(null);

					if(o == null)
						throw new MarvinsArenaException("Error while constructing robot.");

					irobot = ((MarvinsArena.Robot.IRobot)o);
					robot = ((MarvinsArena.Robot.BaseRobot)o);
					robotCore = new MarvinsArena.Robot.Core.RobotCore(Name, squadNumber, team);
					gameCore = new MarvinsArena.Robot.Core.GameCore(map);

					if(t.BaseType == typeof(MarvinsArena.Robot.EnhancedRobot))
					{
						MarvinsArena.Robot.EnhancedRobot enhancedRobot = ((MarvinsArena.Robot.EnhancedRobot)o);
						enhancedRobot.InternalSetCoreRobot(robotCore);
						enhancedRobot.InternalSetCoreGame(gameCore);
					} else if(t.BaseType == typeof(MarvinsArena.Robot.TeamRobot))
					{
						MarvinsArena.Robot.TeamRobot teamRobot = ((MarvinsArena.Robot.TeamRobot)o);
						teamRobot.InternalSetCoreRobot(robotCore);
						teamRobot.InternalSetCoreGame(gameCore);
					} else
					{
						robot.InternalSetCoreRobot(robotCore);
						robot.InternalSetCoreGame(gameCore);
					}
					irobot.Initialize();

					if(t.BaseType == typeof(MarvinsArena.Robot.BaseRobot)) RobotType = LoadedRobotType.Robot;
					if(t.BaseType == typeof(MarvinsArena.Robot.EnhancedRobot)) RobotType = LoadedRobotType.EnhancedRobot;
					if(t.BaseType == typeof(MarvinsArena.Robot.TeamRobot)) RobotType = LoadedRobotType.TeamRobot;

					return;
				}
			}

			throw new MarvinsArenaException("No MsgExchangeType found!");
		}

		public void SetRoundValues(int hitpoints, int missiles, int maxHeat)
		{
			robotCore.SetRoundValues(hitpoints, missiles, maxHeat);
		}

		public MarvinsArena.Robot.Core.MessageExchangeFromRobotEventArgs Update(double deltaTime)
		{
			// Call update normaly but don't move so empty message is returned
			if(!IsActive)
				deltaTime = 0;

			try
			{
				return robotCore.Update(deltaTime);
			} catch(Exception e)
			{
				Disable(e.ToString());
				// Avoid manipulated exceptions from sandbox
				//throw new ApplicationException("Error in update.\n" + e.ToString());
				return new MarvinsArena.Robot.Core.MessageExchangeFromRobotEventArgs();
			}
		}

		public void NotifyCollision(MarvinsArena.Robot.Core.Collision collision)
		{
			if(!IsActive)
				return;

			messageExchangeFromHost.Collisions.Add(collision);
		}

		public void NotifyTarget(MarvinsArena.Robot.Core.ScannerTarget scannerTarget)
		{
			if(!IsActive)
				return;

			messageExchangeFromHost.Targets.Add(scannerTarget);
		}

		public void NotifyMessageToTeammate(MarvinsArena.Robot.Core.CoreMessageReceivedFromTeammateEventArgs msg)
		{
			if(!IsActive)
				return;

			messageExchangeFromHost.MessageReceivedFromTeammate.Add(msg);
		}

		public void Notify()
		{
			if(!IsActive)
				return;

			try
			{
				// Notify robot about changes
				robotCore.Notify(messageExchangeFromHost);
				messageExchangeFromHost.Reset();
			} catch(Exception e)
			{
				Disable(e.ToString());
				// Avoid manipulated exceptions from sandbox
				//throw new ApplicationException("Error in Notify.\n" + e.ToString());
			}
		}

		public void Run()
		{
			try
			{
				if(runLimiter == 0 && IsActive)
					irobot.Run();

				runLimiter++;
				if(runLimiter > RunLimit)
					runLimiter = 0;
			} catch(Exception e)
			{
				Disable(e.ToString());
				// Avoid manipulated exceptions from sandbox
				/*if(e.InnerException != null)
					throw new ApplicationException("Error in run.\n" + e.ToString());
				else
					throw new ApplicationException("Error in run.\n" + e.ToString());*/
			}
		}

		public void Disable(string reason)
		{
			IsActive = false;
			DisabledMessage = reason;
		}

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if(attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if(titleAttribute.Title != "")
						return titleAttribute.Title;
				}

				return System.IO.Path.GetFileNameWithoutExtension(assembly.CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
				if(attributes.Length == 0)
					return "0.0.0.0";
				return ((AssemblyFileVersionAttribute)attributes[0]).Version;
			}
		}

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if(attributes.Length == 0)
					return "";
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if(attributes.Length == 0)
					return "";
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}
	}
}
