#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngine2D
{
	public class WorldObjectRobot : WorldObject
	{
		private Texture2D textureRadar;
		private Texture2D textureTurret;

		public Color Color { get; private set; }

		public WorldObjectRobot(GameObject gameObject)
		{
			GameObject = gameObject;

			System.Drawing.Color sysColor = ((GameObjectRobot)gameObject).TeamColor();
			Color = new Color(sysColor.R, sysColor.G, sysColor.B);
		}

		/// <summary>
		/// Loads the texture.
		/// </summary>
		public override void Load(ContentManager content)
		{
			// Load the texture from the ContentManager.
			Texture = content.Load<Texture2D>("2D_robot");
			textureTurret = content.Load<Texture2D>("2D_Turret");
			textureRadar = content.Load<Texture2D>("2D_Radar");

			base.Load(content);
		}

		public override void Update(double deltaTime)
		{
			base.Update(deltaTime);
		}

		/// <summary>
		/// Draws the texture
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch, Vector2 camera, Vector2 cameraScreen)
		{
			Vector2 position = new Vector2(PositionX, PositionY);
			position.X += camera.X;
			position.Y += camera.Y;

			// Skip out of screen elements
			if(position.X + Texture.Width > cameraScreen.X || position.X < -Texture.Width ||
				position.Y + Texture.Height > cameraScreen.Y || position.Y < -Texture.Height)
				return;

			GameObjectRobot robot = (GameObjectRobot)GameObject;
			Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

			spriteBatch.Draw(Texture, position, null, Color,
							 (float)robot.Rotation, origin, 1, SpriteEffects.None, 0);
			spriteBatch.Draw(textureTurret, position, null, Color.White,
							 (float)robot.RotationGun, origin, 1, SpriteEffects.None, 0);
			spriteBatch.Draw(textureRadar, position, null, Color.White,
							 (float)robot.RotationRadar, origin, 1, SpriteEffects.None, 0);
		}
	}
}
