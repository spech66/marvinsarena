using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;

namespace BattleEngineCommon
{
	public class ViewSelectionMenu
	{
		private SpriteFont fontSmall;
		private Vector2 centerScreenPosition;
		private Texture2D texturePieMenuBack;

		public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
		{
			// Load Font
			fontSmall = content.Load<SpriteFont>("Hud/smallfont");
			
			centerScreenPosition = new Vector2(graphicsDevice.Viewport.Width / 2,
				graphicsDevice.Viewport.Height / 2);

			texturePieMenuBack = content.Load<Texture2D>("PieMenuBack");
		}

		public void Draw(SpriteBatch spriteBatch, IEnumerable<GameObjectRobot> robots)
		{
			int teams = robots.Max(x => x.Team);

			if (teams > 1)
			{
				double itemWidth = 32;
				double itemHeight = 32;
				double step = (2.0 * Math.PI) / (1.0 * (teams));
				double rotation = (90.0 / 180.0) * Math.PI;
				double radius = Math.Sqrt(itemWidth) * Math.Sqrt(1.0 * teams) * 6;

				for (int i = 0; i < teams; i++)
				{
					//todo: center is wrong
					float x = Convert.ToSingle(Math.Cos(i * step - rotation) * radius - (itemWidth / 2.0) + centerScreenPosition.X - itemWidth / 2.0);
					float y = Convert.ToSingle(Math.Sin(i * step - rotation) * radius - (itemHeight / 2.0) + centerScreenPosition.Y - itemHeight / 2.0);
					spriteBatch.Draw(texturePieMenuBack, new Vector2(x, y), Color.White);
					spriteBatch.DrawString(fontSmall, (i+1).ToString(), new Vector2(x + 28, y + 24), Color.White);

					DrawSubMenu(spriteBatch, robots, i, x - Convert.ToSingle(itemWidth / 2.0), y - Convert.ToSingle(itemHeight / 2.0));
				}
			}
			else
			{
				DrawSubMenu(spriteBatch, robots, 1, centerScreenPosition.X, centerScreenPosition.Y);
			}
		}

		private void DrawSubMenu(SpriteBatch spriteBatch, IEnumerable<GameObjectRobot> robots, int team, float centerX, float centerY)
		{
			var robotsSelection = from robot in robots where robot.Team == team select robot;

			int robotCounts = robotsSelection.Count();
			double itemWidth = 32;
			double itemHeight = 32;
			double step = (2.0 * Math.PI) / (1.0 * (robotCounts));
			double rotation = (90.0 / 180.0) * Math.PI;
			double radius = Math.Sqrt(itemWidth) * Math.Sqrt(1.0 * robotCounts) * 6;
			
			int i = 0;			
			foreach(GameObjectRobot robot in robotsSelection)
			{
				float x = Convert.ToSingle(Math.Cos(i * step - rotation) * radius - (itemWidth / 2.0) + centerX - itemWidth / 2.0);
				float y = Convert.ToSingle(Math.Sin(i * step - rotation) * radius - (itemHeight / 2.0) + centerY - itemHeight / 2.0);
				spriteBatch.Draw(texturePieMenuBack, new Vector2(x, y), Color.White);
				spriteBatch.DrawString(fontSmall, robot.Team + "." + robot.SquadNumber, new Vector2(x + 20, y + 24), Color.White);
				i++;
			}
		}
	}
}
