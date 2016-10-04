using System;
using MarvinsArena.Robot;

namespace Test
{
	public class Test : TeamRobot, IRobot
	{
		private bool firstRun;

		/// <summary>
		/// Initialize local variables
		/// </summary>
		public void Initialize()
		{
			ScannedRobot += new EventHandler<ScannedRobotEventArgs>(OnScannedRobot);
			HitWall += new EventHandler<EventArgs>(OnHitWall);
			HitRobot += new EventHandler<EventArgs>(OnHitRobot);
			MessageReceivedFromTeammate += new EventHandler<MessageReceivedFromTeammateEventArgs>(Test_MessageReceivedFromTeammate);

			firstRun = true;
		}

		void Test_MessageReceivedFromTeammate(object sender, MessageReceivedFromTeammateEventArgs e)
		{
			Print(String.Format("Received message from {0}.{1}: {2}", e.Team, e.SquadNumber, e.Message));
		}

		/// <summary>
		/// Run the robot
		/// </summary>
		public void Run()
		{
			if(firstRun)
			{
				SendMessageToTeam("Test");
				/*int test = MapHeightFields;

				Print("OnlyOne");
				PrintDebug("OnlyOne");

				//RotateRight(Math.PI - Rotation);
				RotateGunLeftDeg(90);
				//RotateRight(90);
				//RotateGunLeft(90);
				//RotateRadarLeft(359);
				//RotateRadarRightDeg(359);
				//RotateRadarRight(2.0 * Math.PI);
				//throw new Exception("aua");*/	
				/*System.Random rand = new Random(DateTime.Now.Millisecond);
				MoveForward(rand.Next(0, 300));
				MoveBackward(rand.Next(0, 300));
				RotateLeft(rand.Next(0, 360));
				RotateRight(rand.Next(0, 360));*/
				//System.Threading.Thread.Sleep(260);
				//RotateLeft(45);
				//MoveForward(128);
				//MoveForward(32); //32 == 1 field
				/*MoveForward(32);
				 */
			}
			firstRun = false;

			double x = PositionX;
			double y = PositionY;

			if(RemainingRotation == 0)
			{
				MoveForward(32);
			}
		}

		void OnHitRobot(object sender, EventArgs e)
		{
			RotateRightDeg(90);
		}

		void OnHitWall(object sender, EventArgs e)
		{
			WalkWalls();
		}

		void OnScannedRobot(object sender, ScannedRobotEventArgs e)
		{
			FireMissile();
		}

		private void WalkWalls()
		{
			double rot = 90 - RotationDeg % 90;
			if(rot > 90)
				rot = 90;

			RotateRightDeg(rot);
			RotateGunRightDeg(RotationDeg - RotationGunDeg + 180);
			RotateRadarRightDeg(RotationDeg - RotationRadarDeg + 180);
		}
	}
}
