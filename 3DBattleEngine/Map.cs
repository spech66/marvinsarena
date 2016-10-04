#region File Description
//-----------------------------------------------------------------------------
// Map.cs
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MarvinsArena.Core;
#endregion

namespace BattleEngine3D
{
	/// <summary>
	/// Map
	/// Size is internal + 2 and moved 1 field behind (0,0) as this will be the wall.
	/// </summary>
	public class Map : BattleEngineCommon.Map, IDisposable
    {
        #region Fields
        private bool isDisposed;

        // Rendering
		private Texture2D texture;
        private VertexBuffer vertexBuffer;
        private VertexDeclaration vertexDeclaration;
        private BasicEffect effect;
		private Matrix projection = Matrix.Identity;
		private Matrix view = Matrix.Identity;
		private Matrix world = Matrix.Identity;
        private GraphicsDevice device;
        #endregion

        #region Public Properties		
        public Matrix ProjectionMatrix
        {
            get { return projection; }
            set { projection = value; }
        }
        public Matrix WorldMatrix
        {
            get { return world; }
            set { world = value; }
        }
        public Matrix ViewMatrix
        {
            get { return view; }
            set { view = value; }
        }
        #endregion

		public Map(int widht, int height)
			: base(widht, height)
		{
		}

		public Map(TournamentMap map)
			: base(map)
		{
		}

		#region Loading
        public void UnloadGraphicsContent()
        {
            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
                vertexBuffer = null;
            }
            if (vertexDeclaration != null)
            {
                vertexDeclaration.Dispose();
                vertexDeclaration = null;
            }
            if (effect != null)
            {
                effect.Dispose();
                effect = null;
            }
        }

		public void LoadGraphicsContent(GraphicsDevice graphicsDevice, ContentManager content, Theme theme)
        {
            device = graphicsDevice;

            effect = new BasicEffect(device);

			switch(theme)
			{
				case Theme.Wall: texture = content.Load<Texture2D>("map_wall"); break;
				case Theme.World: texture = content.Load<Texture2D>("map_world"); break;
				case Theme.Scifi: texture = content.Load<Texture2D>("map_scifi"); break;
			}
			//texture = content.Load<Texture2D>("map_wall");
			//texture = content.Load<Texture2D>("map_world");
			//texture = content.Load<Texture2D>("map_scifi");
			
			SetUpVertices();
        }

		private void SetUpVertices()
		{
			int width = HeightmapLength0;
			int length = HeightmapLength1;

			List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();
			float sizeX, sizeZ;
			int maxWidth = width - 1; // Calculation is done positive (sizeX + mapSize)
			int maxLength = length; // Calculation is done negative (sizeZ - mapSize)
			for(int x = -1; x < maxWidth; x++)
			{
				for(int z = 0; z < maxLength; z++)
				{
					float currentheight = Heightmap[x + 1, z] * MapScale;
					sizeX = x * MapScale;
					sizeZ = z * MapScale;

					//floor
					if(currentheight == 0)
					{
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ), new Vector3(0, 1, 0), new Vector2(0, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ - MapScale), new Vector3(0, 1, 0), new Vector2(0, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ), new Vector3(0, 1, 0), new Vector2(0.5f, 0.5f)));

						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ - MapScale), new Vector3(0, 1, 0), new Vector2(0, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ - MapScale), new Vector3(0, 1, 0), new Vector2(0.5f, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ), new Vector3(0, 1, 0), new Vector2(0.5f, 0.5f)));
					} else //ceiling
					{
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ), new Vector3(0, 1, 0), new Vector2(0, 1)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ - MapScale), new Vector3(0, 1, 0), new Vector2(0, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ), new Vector3(0, 1, 0), new Vector2(0.5f, 1)));

						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ - MapScale), new Vector3(0, 1, 0), new Vector2(0, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ - MapScale), new Vector3(0, 1, 0), new Vector2(0.5f, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ), new Vector3(0, 1, 0), new Vector2(0.5f, 1)));
					}

					if(currentheight != 0)
					{
						//front wall
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, 0, sizeZ - MapScale), new Vector3(0, 0, -1), new Vector2(1, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ - MapScale), new Vector3(0, 0, -1), new Vector2(0.5f, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, 0, sizeZ - MapScale), new Vector3(0, 0, -1), new Vector2(0.5f, 0.5f)));

						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ - MapScale), new Vector3(0, 0, -1), new Vector2(0.5f, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, 0, sizeZ - MapScale), new Vector3(0, 0, -1), new Vector2(1, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ - MapScale), new Vector3(0, 0, -1), new Vector2(1, 0)));

						//back wall
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, 0, sizeZ), new Vector3(0, 0, 1), new Vector2(1, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, 0, sizeZ), new Vector3(0, 0, 1), new Vector2(0.5f, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ), new Vector3(0, 0, 1), new Vector2(0.5f, 0)));

						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ), new Vector3(0, 0, 1), new Vector2(0.5f, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ), new Vector3(0, 0, 1), new Vector2(1, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, 0, sizeZ), new Vector3(0, 0, 1), new Vector2(1, 0.5f)));

						//left wall
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, 0, sizeZ), new Vector3(-1, 0, 0), new Vector2(1, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, 0, sizeZ - MapScale), new Vector3(-1, 0, 0), new Vector2(0.5f, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ - MapScale), new Vector3(-1, 0, 0), new Vector2(0.5f, 0)));

						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ - MapScale), new Vector3(-1, 0, 0), new Vector2(0.5f, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, currentheight, sizeZ), new Vector3(-1, 0, 0), new Vector2(1, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX, 0, sizeZ), new Vector3(-1, 0, 0), new Vector2(1, 0.5f)));

						//right wall
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, 0, sizeZ), new Vector3(1, 0, 0), new Vector2(1, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ - MapScale), new Vector3(1, 0, 0), new Vector2(0.5f, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, 0, sizeZ - MapScale), new Vector3(1, 0, 0), new Vector2(0.5f, 0.5f)));

						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ - MapScale), new Vector3(1, 0, 0), new Vector2(0.5f, 0)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, 0, sizeZ), new Vector3(1, 0, 0), new Vector2(1, 0.5f)));
						verticesList.Add(new VertexPositionNormalTexture(new Vector3(sizeX + MapScale, currentheight, sizeZ), new Vector3(1, 0, 0), new Vector2(1, 0)));
					}
				}
			}

			vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), verticesList.Count, BufferUsage.None);
			vertexBuffer.SetData<VertexPositionNormalTexture>(verticesList.ToArray());
		}

        ~Map()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    //if we're manually disposing,
                    //then managed content should be unloaded
                    UnloadGraphicsContent();
                }
                isDisposed = true;
            }
        }

        #endregion

		public bool CheckCollision(WorldObject obj)
		{
			for(int x = -1; x < HeightmapLength0 - 1; x++)
			{
				int mapX = (int)(x * MapScale);
				for(int z = -1; z < HeightmapLength1 - 1; z++)
				{
					int mapZ = (int)(z * MapScale);

					if(Heightmap[x + 1, z + 1] == 1 &&
						(obj.PositionX - obj.GameObject.Radius <= mapX + MapScale) &&
						(obj.PositionX + obj.GameObject.Radius >= mapX) &&
						(obj.PositionZ - obj.GameObject.Radius <= mapZ + MapScale) &&
						(obj.PositionZ + obj.GameObject.Radius >= mapZ))
						return true;
				}
			}

			return false;
		}

        #region Drawing
        public void Draw()
        {
			effect.TextureEnabled = true;
			effect.World = Matrix.Identity;
			effect.View = view;
			effect.Projection = projection;
			effect.Texture = texture;
			effect.CurrentTechnique.Passes[0].Apply();
			foreach(EffectPass pass in effect.CurrentTechnique.Passes)
			{
				//device.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
				//device.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.SizeInBytes / VertexPositionNormalTexture.SizeInBytes / 3); //TODO!

				device.SetVertexBuffer(vertexBuffer);
				device.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount);

				pass.Apply();
			}
        }
        #endregion
    }
}
