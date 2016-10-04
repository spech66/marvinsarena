using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleEngine3D
{
	class Sky
	{
		private Model skySphere;
		private Effect skySphereEffect;
		private TextureCube skyboxTexture;

		public void Load(Microsoft.Xna.Framework.Content.ContentManager content, Theme theme)
		{
			switch(theme)
			{
				case Theme.Wall: skyboxTexture = content.Load<TextureCube>("GK_CM_SK013_cm_rgba"); break;
				case Theme.World: skyboxTexture = content.Load<TextureCube>("GK_CM_SK008_cm_rgba"); break;
				case Theme.Scifi: skyboxTexture = content.Load<TextureCube>("GK_CM_SK001_cm_rgba"); break;
			}

			skySphereEffect = content.Load<Effect>("skyEffect");
			skySphere = content.Load<Model>("SkySphere");
			skySphereEffect.Parameters["SkyboxTexture"].SetValue(skyboxTexture);
		}

		public void Draw(GraphicsDevice graphics, ArcBallCamera camera, Matrix projection)
		{
			// Set the parameters of the effect
			skySphereEffect.Parameters["ViewMatrix"].SetValue(camera.ViewMatrix);
			skySphereEffect.Parameters["ProjectionMatrix"].SetValue(projection);			

			foreach(ModelMesh mesh in skySphere.Meshes)
			{
				foreach(ModelMeshPart part in mesh.MeshParts)
				{
					part.Effect = skySphereEffect;
				}
			}

			// Draw model
			foreach(ModelMesh mesh in skySphere.Meshes)
			{
				mesh.Draw();
			}

			// Undo the renderstate settings from the shader
			graphics.RasterizerState = RasterizerState.CullCounterClockwise;
			graphics.DepthStencilState = DepthStencilState.Default;

		}
	}
}
