using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MarvinsArena.Core;

namespace BattleEngineCommon
{
	public class Map
	{
		private int[,] heightmap;
		private List<Vector2> robotSpawnPlaces = new List<Vector2>();
		private Random rand = new Random();

		#region Public Properties
		public int MapHeight
		{
			get { return heightmap.GetLength(1) - 2; }
		}
		public int MapWidth
		{
			get { return heightmap.GetLength(0) - 2; }
		}
		public int HeightmapLength0
		{
			get { return heightmap.GetLength(0); }
		}
		public int HeightmapLength1
		{
			get { return heightmap.GetLength(1); }
		}
		public float MapScale
		{
			get { return 32.0f; }
		}
		public int[,] Heightmap
		{
			get { return heightmap; }
			set { heightmap = value; }
		}
		#endregion

		#region Constructors and Loading
		public Map(int mapSize)
			: this(mapSize, mapSize)
		{
		}

		public Map(int widht, int height)
		{
			heightmap = new int[widht + 2, height + 2];
			for (int i = 0; i < widht + 2; i++)
			{
				for (int j = 0; j < height + 2; j++)
				{
					if (i == 0 || j == 0 || i == widht + 1 || j == height + 1)
						heightmap[i, j] = 1;
				}
			}

			/* heightmap = new int[,]
			  {
			  {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			  {1,0,0,1,1,0,0,0,1,1,0,0,1,0,1},
			  {1,0,0,1,1,0,0,0,1,0,0,0,1,0,1},
			  {1,0,0,0,1,1,0,1,1,0,0,0,0,0,1},
			  {1,0,0,0,0,0,0,0,0,0,0,1,0,0,1},
			  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			  {1,0,1,1,0,0,0,1,0,0,0,0,0,0,1},
			  {1,0,1,0,0,0,0,0,0,0,0,0,0,0,1},
			  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
			  {1,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
			  {1,0,0,0,0,1,0,0,0,1,0,0,0,0,1},
			  {1,0,1,0,0,0,0,0,0,1,0,0,0,0,1},
			  {1,0,1,1,0,0,0,0,1,1,0,0,0,1,1},
			  {1,0,0,0,0,0,0,0,1,1,0,0,0,1,1},
			  {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
			 };*/

			BuildNewSpawnList();
		}

		public Map(TournamentMap map)
		{
			heightmap = new int[map.MapWidth + 2, map.MapHeight + 2];
			for (int i = 0; i < map.MapWidth + 2; i++)
			{
				for (int j = 0; j < map.MapHeight + 2; j++)
				{
					if (i == 0 || j == 0 || i == map.MapWidth + 1 || j == map.MapHeight + 1)
					{
						heightmap[i, j] = 1;
					}
					else
					{
						heightmap[i, j] = map[i - 1, j - 1];
					}
				}
			}

			BuildNewSpawnList();
		}
		#endregion

		public void BuildNewSpawnList()
		{
			// Set Spawn places to center of map nodes
			robotSpawnPlaces.Clear();
			for (int i = 0; i < HeightmapLength0; i++)
			{
				for (int j = 0; j < HeightmapLength1; j++)
				{
					if (heightmap[i, j] == 0)
						robotSpawnPlaces.Add(new Vector2(i * 32.0f - 16.0f, j * 32.0f - 16.0f));
				}
			}
		}

		/// <summary>
		/// Find a nice and free place for the robot
		/// </summary>
		/// <param name="robot"></param>
		public void PlaceRobot(GameObjectRobot robot)
		{
			int r = rand.Next(robotSpawnPlaces.Count);
			robot.PositionX = robotSpawnPlaces[r].X;
			robot.PositionY = robotSpawnPlaces[r].Y;
			robotSpawnPlaces.Remove(robotSpawnPlaces[r]);

			robot.Rotation = rand.Next(0, (int)(Math.PI * 2));
			robot.RotationGun = robot.Rotation;
			robot.RotationRadar = robot.Rotation;
		}
	}
}
