#region Using Statements
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngine2D
{
	public class Hud : BattleEngineCommon.Hud
	{
		public void DrawrobotNames(SpriteBatch spriteBatch, GameObjectRobot robot, Vector2 camera)
		{
			Vector2 fontOriginRobotName = FontSmall.MeasureString(robot.FullName) / 2;
			fontOriginRobotName.X = (float)Math.Ceiling(fontOriginRobotName.X); // Move to full pixel
			fontOriginRobotName.Y = (float)Math.Ceiling(fontOriginRobotName.Y);
			spriteBatch.DrawString(FontSmall, robot.FullName,
				new Vector2((int)(camera.X + robot.PositionX), (int)(camera.Y + robot.PositionY - robot.Radius)),
				Color.White, 0, fontOriginRobotName, 1.0f, SpriteEffects.None, 0.5f);
		}
	}
}
