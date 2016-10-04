#region Using Statements
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngine3D
{
	public class Hud : BattleEngineCommon.Hud
	{
		public void DrawrobotNames(SpriteBatch spriteBatch, WorldObjectRobot robot, GraphicsDevice graphicsDevice, ArcBallCamera camera, Matrix projection)
		{
			GameObjectRobot gameObjectRobot = (GameObjectRobot)robot.GameObject;
			Vector3 screenSpace = graphicsDevice.Viewport.Project(Vector3.Zero,
												projection,
												camera.ViewMatrix,
												Matrix.CreateTranslation(robot.PositionX, robot.PositionY, robot.PositionZ));

			// Get 2D coordinates from screenspace vector
			Vector2 textPosition;
			textPosition.X = screenSpace.X;
			textPosition.Y = screenSpace.Y;
			
			// Center the text
			Vector2 stringCenter = FontSmall.MeasureString(gameObjectRobot.FullName) * 0.5f;
			
			// Calculate position
			textPosition.X = (int)(textPosition.X - stringCenter.X);
			//textPosition.Y = (int)(textPosition.Y - stringCenter.Y);
			textPosition.Y = (int)(textPosition.Y + stringCenter.Y);

			// Skip if out of screen
			if(textPosition.X < 0 || textPosition.X > graphicsDevice.Viewport.Width)
				return;
			if(textPosition.Y < 0 || textPosition.Y > graphicsDevice.Viewport.Height)
				return;

			// Draw the text
			spriteBatch.DrawString(FontSmall, gameObjectRobot.FullName, textPosition, Color.White);
		}		
	}
}
