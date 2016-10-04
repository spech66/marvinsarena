#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngine2D
{
	public class WorldObjectBullet : WorldObject
	{
		public WorldObjectBullet(int power)
		{
			GameObject = new GameObjectBullet(power);
		}

		/// <summary>
		/// Loads the texture.
		/// </summary>
		public override void Load(ContentManager content)
		{
			// Load the texture from the ContentManager.
			Texture = content.Load<Texture2D>("2D_Bullet");

			base.Load(content);
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

			Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

			spriteBatch.Draw(Texture, position, null, Color.White,
							 (float)Rotation, origin, 1, SpriteEffects.None, 0);
		}
	}
}
