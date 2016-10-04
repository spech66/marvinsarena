#region File Description
//-----------------------------------------------------------------------------
// Map.cs
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MarvinsArena.Core;
#endregion

namespace BattleEngine2D
{
	/// <summary>
	/// Map
	/// Size is internal + 2 and moved 1 field behind (0,0) as this will be the wall.
	/// </summary>
    public class Map : BattleEngineCommon.Map
    {
		private Texture2D texture;

		public Map(int widht, int height)
			: base(widht, height)
		{
		}

		public Map(TournamentMap map)
			: base(map)
		{
		}

        #region Loading
		public void LoadGraphicsContent(ContentManager content)
        {
			Random rand = new Random();
			switch(rand.Next(0, 3))
			{
				case 0: texture = content.Load<Texture2D>("map_wall"); break;
				case 1: texture = content.Load<Texture2D>("map_world"); break;
				case 2: texture = content.Load<Texture2D>("map_scifi"); break;
			}

			//texture = content.Load<Texture2D>("map_wall");
			//texture = content.Load<Texture2D>("map_world");
			//texture = content.Load<Texture2D>("map_scifi");
        }
        #endregion

		public bool CheckCollision(WorldObject obj)
		{
			for(int x = -1; x < HeightmapLength0 - 1; x++)
			{
				int mapX = (int)(x * MapScale);
				for(int y = -1; y < HeightmapLength1 - 1; y++)
				{
					int mapY = (int)(y * MapScale);

					if(Heightmap[x + 1, y + 1] == 1 &&
						(obj.PositionX - obj.GameObject.Radius <= mapX + MapScale) &&
						(obj.PositionX + obj.GameObject.Radius >= mapX) &&
						(obj.PositionY - obj.GameObject.Radius <= mapY + MapScale) &&
						(obj.PositionY + obj.GameObject.Radius >= mapY))
						return true;
				}
			}

			return false;
		}

        #region Drawing
		public void Draw(SpriteBatch spriteBatch, Vector2 camera, Vector2 cameraScreen)
        {
			int texPartWidth = texture.Width / 2;
			int texPartHeight = texture.Height / 2;

			for(int x = -1; x < HeightmapLength0 - 1; x++)
			{
				int posX = (int)(x * MapScale + camera.X);
				for(int y = -1; y < HeightmapLength1 - 1; y++)
				{
					int posY = (int)(y * MapScale + camera.Y);

					// Skip out of screen elements
					if(posX + MapScale > cameraScreen.X || posX < -MapScale ||
						posY + MapScale > cameraScreen.Y || posY < -MapScale)
						continue;
					
					if(Heightmap[x + 1, y + 1] == 0)
					{
						spriteBatch.Draw(texture, new Rectangle(posX, posY, (int)MapScale, (int)MapScale),
							new Rectangle(0, 0, texPartWidth, texPartHeight), Color.White);
					} else
					{
						spriteBatch.Draw(texture, new Rectangle(posX, posY, (int)MapScale, (int)MapScale),
							new Rectangle(0, texPartHeight, texPartWidth, texPartHeight), Color.White);
					}
				}
			}
        }
		#endregion
	}
}
