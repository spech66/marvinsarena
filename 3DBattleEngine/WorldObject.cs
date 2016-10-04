#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngine3D
{
	public class WorldObject
	{
		#region Fields
		private GameObject gameObject;
		private float realPositionY;
		private float scale;
		private Model model;
		//private BoundingSphere mergedSphere;
		#endregion

		#region Properties
		public GameObject GameObject { get { return gameObject; } set { gameObject = value; } }
		public float Rotation { get { return (float)gameObject.Rotation; } set { gameObject.Rotation = value; } }
		public float PositionX { get { return (float)gameObject.PositionX; } set { gameObject.PositionX = value; } }
		public float PositionY { get { return realPositionY; } set { realPositionY = value; } }
		public float PositionZ { get { return (float)gameObject.PositionY; } set { gameObject.PositionY = value; } }
		public float Scale { get { return scale; } set { scale = value; } }
		public Model Model { get { return model; } set { model = value; } }
		//public BoundingSphere BoundingSphere { get { return mergedSphere; } }
		public bool Dead { get; set; }
		#endregion

		public WorldObject()
		{
			Dead = false;
		}

		public virtual void Load(ContentManager content)
		{
			//CalculateBoundingSphere();
		}

		public virtual void Update(double deltaTime)
		{
			gameObject.Update(deltaTime);
		}

		public virtual void Draw(Matrix world, Matrix view, Matrix projection)
		{
		}

		public bool CheckForCollisions(WorldObject object2)
		{
			/*BoundingSphere bs = BoundingSphere;
			bs = bs.Transform(Matrix.CreateScale(Scale) * Matrix.CreateTranslation(PositionX, PositionY, PositionZ));

			BoundingSphere c2BoundingSphere = object2.BoundingSphere;
			c2BoundingSphere = c2BoundingSphere.Transform(Matrix.CreateScale(object2.Scale) * Matrix.CreateTranslation(object2.PositionX, object2.PositionY, object2.PositionZ));

			if(bs.Intersects(c2BoundingSphere))
			{
				return true;
			}

			return false;*/
			return GameObject.CheckForCollisions(object2.GameObject);
		}

		/*public void RenderBoundingSphere(GraphicsDevice graphicsDevice,
			Matrix view,
			Matrix projection,
			Color color)
		{
			BoundingSphere bs = mergedSphere;
			bs = bs.Transform(Matrix.CreateScale(Scale) * Matrix.CreateTranslation(PositionX, PositionY, PositionZ));
			BoundingSphereRenderer.Render(bs, graphicsDevice, view, projection, color);
		}

		/// <summary>
		/// Combine all spheres to one large sphere
		/// </summary>
		protected void CalculateBoundingSphere()
		{
			mergedSphere = new BoundingSphere();
			BoundingSphere[] boundingSpheres;
			int index = 0;
			int meshCount = Model.Meshes.Count;

			boundingSpheres = new BoundingSphere[meshCount];
			foreach(ModelMesh mesh in Model.Meshes)
			{
				boundingSpheres[index++] = mesh.BoundingSphere;
			}

			mergedSphere = boundingSpheres[0];
			if((Model.Meshes.Count) > 1)
			{
				index = 1;
				do
				{
					mergedSphere = BoundingSphere.CreateMerged(mergedSphere, boundingSpheres[index]);
					index++;
				} while(index < Model.Meshes.Count);
			}
			mergedSphere.Center.Y = 0;
		}*/
	}
}
