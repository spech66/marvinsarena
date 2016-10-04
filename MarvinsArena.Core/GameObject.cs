using System;

namespace MarvinsArena.Core
{
	public class GameObject
	{
		public virtual double Rotation { get; set; }
		public virtual double PositionX { get; set; }
		public virtual double PositionY { get; set; }
		public virtual string Name { get; set; }
		public virtual string FullName { get { return Name; } }
		public virtual double Radius { get { return 16.0f; } }

		public virtual void Update(double deltaTime)
		{
		}

		public bool CheckForCollisions(GameObject obj2)
		{
			return CheckForCollisions(this, obj2);
		}

		public static bool CheckForCollisions(GameObject obj1, GameObject obj2)
		{
			double xdif = obj1.PositionX - obj2.PositionX;
			double ydif = obj1.PositionY - obj2.PositionY;

			double distance = Math.Sqrt(xdif * xdif + ydif * ydif);

			if(distance <= obj1.Radius + obj2.Radius)
				return true;

			return false;
		}

		/// <summary>
		/// Check if target is in scanner range of robot
		/// </summary>
		/// <param name="scanner">The scanning robot</param>
		/// <param name="target">The target object</param>
		/// <returns>true if in range</returns>
		public static bool ObjectInScannerRange(GameObjectRobot scanner, GameObject target)
		{
			double scanRadius = 0.69813170079773183076947630739545; //40°
			double scanRadiusHalf = scanRadius / 2.0;

			double scannerAngleInRad = scanner.RotationRadar - scanRadiusHalf;
			double scannerAngleInRad2 = scanner.RotationRadar + scanRadiusHalf;
			double angleBetweenObjects = Math.Atan2(target.PositionY - scanner.PositionY, target.PositionX - scanner.PositionX);

			if(angleBetweenObjects >= scannerAngleInRad && angleBetweenObjects <= scannerAngleInRad2)
				return true;

			if(scannerAngleInRad < -Math.PI && angleBetweenObjects == Math.PI && target.PositionX < scanner.PositionX)
				return true;

			return false;
		}
	}
}
