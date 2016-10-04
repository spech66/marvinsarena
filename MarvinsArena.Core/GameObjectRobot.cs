using System;
using System.Drawing;
using MarvinsArena.Robot.Core;

namespace MarvinsArena.Core
{
	public class GameObjectRobot : GameObject
	{
		private RobotLoader loader;
		private RobotHost robotHost;

		public override double PositionX { get { return robotHost.PositionX; } set { robotHost.PositionX = value; } }
		public override double PositionY { get { return robotHost.PositionY; } set { robotHost.PositionY = value; } }
		public override double Rotation { get { return robotHost.Rotation; } set { robotHost.Rotation = value; } }
		public double RotationGun { get { return robotHost.RotationGun; } set { robotHost.RotationGun = value; } }
		public double RotationRadar { get { return robotHost.RotationRadar; } set { robotHost.RotationRadar = value; } }
		public override string Name { get { return robotHost.Name; } set { } }
		public override string FullName { get { return String.Format("{0} {1}.{2}", Name, Team, SquadNumber); } }
		public int SquadNumber { get { return robotHost.SquadNumber; } }
		public int Team { get { return robotHost.Team; } }
		public int Hitpoints { get { return robotHost.Hitpoints; } }
		public int Missiles { get { return robotHost.Missiles; } }
		public int Heat { get { return robotHost.Heat; } }
		public override double Radius { get { return 12.0f; } }
		public bool IsActive { get { return robotHost.IsActive; } }
		public string DisabledMessage { get { return robotHost.DisabledMessage; } }

		public event EventHandler<MessageExchangeFromRobotEventArgs> HandleRobotEvents;

		public GameObjectRobot(RobotLoader loader)
		{
			this.loader = loader;
			robotHost = loader.RobotHost;
		}

		/// <summary>
		/// Returns the robots color based on the team
		/// </summary>
		/// <returns>The team color</returns>
		public Color TeamColor()
		{
			switch(Team)
			{
				case 1: return Color.FromArgb(255, 0, 0);      // Red
				case 2: return Color.FromArgb(0, 0, 255);      // Blue
				case 3: return Color.FromArgb(0, 255, 0);      // Green
				case 4: return Color.FromArgb(153, 31, 54);    // Arizona Cardinals Red
				case 5: return Color.FromArgb(247, 181, 18);   // AC Yellow
				case 6: return Color.FromArgb(0, 115, 54);     // Celtic Green
				case 7: return Color.FromArgb(0, 51, 171);     // Buffalo Bills Royal Blue
				case 8: return Color.FromArgb(156, 31, 46);    // Kansas City Chiefs Red
				case 9: return Color.FromArgb(36, 87, 69);     // Green Bay
				case 10: return Color.FromArgb(186, 18, 43);   // Atlanta Falcons Red
				case 11: return Color.FromArgb(217, 79, 0);    // Clevland Browns Orange
				case 12: return Color.FromArgb(181, 179, 140); // Clevland Browns Champangne
				case 13: return Color.FromArgb(115, 133, 140); // Dallas Cowboys Metallic Silver Blue
				default: return Color.FromArgb(255, 255, 255); // Black
			}
		}

		public override void Update(double deltaTime)
		{
			//robotHost.Run();

			System.Threading.Thread threadRun = new System.Threading.Thread(robotHost.Run);
			threadRun.Start();

			if(CoreMain.Instance.Timeout)
			{
				// Terminate after 250ms
				if(!threadRun.Join(250))
				{
					robotHost.Disable("Timeout of 250ms reached");
				}
			} else
			{
				threadRun.Join();
			}

			MessageExchangeFromRobotEventArgs ex = robotHost.Update(deltaTime);

			if(HandleRobotEvents != null)
			{
				HandleRobotEvents(this, ex);
			}
		}

		public void NotifyMessageToTeammate(MarvinsArena.Robot.Core.CoreMessageReceivedFromTeammateEventArgs msg)
		{
			robotHost.NotifyMessageToTeammate(msg);
		}

		public void NotifyCollision(Collision collision)
		{
			robotHost.NotifyCollision(collision);
		}

		public void NotifyTarget(ScannerTarget scannerTarget)
		{
			robotHost.NotifyTarget(scannerTarget);
		}

		public void Notify()
		{
			robotHost.Notify();
		}

		public void SetRoundValues(TournamentRules rules)
		{
			robotHost.SetRoundValues(rules.Hitpoints, rules.Missiles, rules.MaxHeat);
		}
	}
}
