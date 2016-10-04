#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngine2D
{
	public class WorldObject
	{
		#region Fields
		#endregion

		#region Properties
		public GameObject GameObject { get; set; }
		public float Rotation { get { return (float)GameObject.Rotation; } set { GameObject.Rotation = value; } }
		public float PositionX { get { return (float)GameObject.PositionX; } set { GameObject.PositionX = value; } }
		public float PositionY { get { return (float)GameObject.PositionY; } set { GameObject.PositionY = value; } }
		public Texture2D Texture { get; set; }
		public bool Dead { get; set; }
		#endregion

		public WorldObject()
		{
			Dead = false;
		}

		public virtual void Load(ContentManager content)
		{
		}

		public virtual void Update(double deltaTime)
		{
			GameObject.Update(deltaTime);
		}

		public virtual void Draw(SpriteBatch spriteBatch, Vector2 camera, Vector2 cameraScreen)
		{
		}

		public bool CheckForCollisions(WorldObject object2)
		{
			return GameObject.CheckForCollisions(object2.GameObject);
		}
	}
}
