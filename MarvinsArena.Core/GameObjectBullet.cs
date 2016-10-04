using System;

namespace MarvinsArena.Core
{
	public class GameObjectBullet : GameObject
	{
		private const double bulletMoveSpeed = 0.1;

		public int Power { get; private set; }
		public override double Radius { get { return 4.0f; } }

		public GameObjectBullet(int power)
		{
			Name = "Bullet";
			Power = power;
		}

		public override void Update(double deltaTime)
		{
			PositionX += bulletMoveSpeed * deltaTime * Math.Cos(Rotation);
			PositionY += bulletMoveSpeed * deltaTime * Math.Sin(Rotation);
		}
	}
}
