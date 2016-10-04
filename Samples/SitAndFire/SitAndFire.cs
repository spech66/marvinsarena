using System;
using MarvinsArena.Robot;

namespace SitAndFire
{
	/// <summary>
	/// The SitAndFire robot will only rotate the gun and fire if possible
	/// </summary>
	public class SitAndFire : BaseRobot, IRobot
	{
		private int bulletCounter;

		/// <summary>
		/// Initialize local variables
		/// </summary>
		public void Initialize()
		{
			bulletCounter = 0;
		}

		/// <summary>
		/// Run the robot
		/// </summary>
		public void Run()
		{
			if(RemainingRotationGun == 0)
				RotateGunLeft(1);

			if (Heat < MaximumHeat && bulletCounter != 10)
			{
				if (FireBullet(1))
				{
					bulletCounter++;
				}
			}
			
			if(bulletCounter == 10)
			{
				bulletCounter = 0;
				FireMissile();
			}
		}
	}
}
