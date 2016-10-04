using System;

namespace MarvinsArena.Core
{
	public class GameObjectMissile : GameObject
	{
		private const double missileMoveSpeed = 0.09;

		public int Power { get; private set; }
		public override double Radius { get { return 8.0f; } }

		public GameObjectMissile()
		{
			Name = "Missile";
			Power = 10;
		}

		public override void Update(double deltaTime)
		{
			PositionX += missileMoveSpeed * deltaTime * Math.Cos(Rotation);
			PositionY += missileMoveSpeed * deltaTime * Math.Sin(Rotation);
		}
	}
}
