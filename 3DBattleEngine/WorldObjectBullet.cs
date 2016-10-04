#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MarvinsArena.Core;
#endregion

namespace BattleEngine3D
{
	public class WorldObjectBullet : WorldObject
	{
		public WorldObjectBullet(int power)
		{
			GameObject = new GameObjectBullet(power);
		}

		/// <summary>
		/// Loads the model.
		/// </summary>
		public override void Load(ContentManager content)
		{
			// Load the model from the ContentManager.
			Model = content.Load<Model>("SphereLowPoly");

			PositionY = 14;
			Scale = 0.8f;

			base.Load(content);
		}

		/// <summary>
		/// Draws the model, using the current animation settings.
		/// </summary>
		public override void Draw(Matrix world, Matrix view, Matrix projection)
		{
			Matrix world2 = Matrix.CreateScale(Scale) * Matrix.CreateRotationY(Rotation) * Matrix.CreateTranslation(PositionX, PositionY, PositionZ);
			Model.Root.Transform = world2;

			Matrix[] boneTransforms = new Matrix[Model.Bones.Count];
			Model.CopyAbsoluteBoneTransformsTo(boneTransforms);

			// Draw the model.
			foreach(ModelMesh mesh in Model.Meshes)
			{
				foreach(BasicEffect effect in mesh.Effects)
				{
					effect.World = boneTransforms[mesh.ParentBone.Index];
					effect.View = view;
					effect.Projection = projection;

					effect.EnableDefaultLighting();
				}

				mesh.Draw();
			}
		}
	}
}
