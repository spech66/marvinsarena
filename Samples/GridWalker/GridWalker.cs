using System;
using MarvinsArena.Robot;

namespace GridWalker
{
	/// <summary>
	/// My robot
	/// </summary>
	public class GridWalker : BaseRobot, IRobot
	{
		System.Random rand;
		bool start = true;
		int gridWidth, gridHeight;
		int targetX, targetY;

		/// <summary>
		/// Initialize local variables
		/// </summary>
		public void Initialize()
		{
			// TODO: Assign values to variables and event handler
			rand = new Random();

			RoundStarted += new EventHandler<EventArgs>(GridWalker_RoundStarted);
		}

		void GridWalker_RoundStarted(object sender, EventArgs e)
		{
			gridWidth = MapWidth / 32;
			gridHeight = MapHeight / 32;

			Print(String.Format("Map fields ({0}, {1})", gridWidth, gridHeight));
			SetNextTarget();

			start = false;

			RotateLeftDeg(RotationDeg);
		}

		/// <summary>
		/// Run the robot
		/// </summary>
		public void Run()
		{
			if(TargetReached())
			{
				SetNextTarget();
			} else
			{
				if(PositionX > targetX)
				{
					//if(RemainingDistance == 0)
					//	RotateLeftDeg(90);
					if(RemainingRotation == 0)
						MoveForward(1);
				} else if(PositionX < targetX)
				{
					//if (RemainingDistance == 0)
					//	RotateGunLeftDeg(-90);
					if (RemainingRotation == 0)
						MoveForward(1);
				}
			}
		}

		void SetNextTarget()
		{
			int nextGridX = rand.Next(0, gridWidth);
			int nextGridY = rand.Next(0, gridHeight);

			Print(String.Format("Moving  from ({0}, {1}) to ({2}, {3})",
				nextGridX, nextGridY,
				(PositionX - 16) / 32, (PositionY - 16) / 32));

			targetX = nextGridX * 32 + 16;
			targetY = nextGridY * 32 + 16;
		}

		bool TargetReached()
		{
			if(PositionX == targetX && PositionY == targetY)
				return true;

			return false;
		}
	}
}
