using System;
using System.Xml.Serialization;

namespace MarvinsArena.Core
{
	[Serializable]
	public class TournamentMap
	{
		private byte[][] map;

		public byte[][] Map { get { return map; } set { map = value; } }
		[XmlIgnore]
		public int FreeFields { get; private set; }

		public TournamentMap()
		{
			map = new byte[][]
			{
				new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
				new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
				new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
				new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
				new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
				new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
				new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
				new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }
			};

			FreeFields = CalculateFreeFields();
		}

		public TournamentMap(byte[][] map)
		{
			this.map = map;
			FreeFields = CalculateFreeFields();
		}

		public byte this[int indexWidth, int indexHeight]
		{
			get
			{
				if(indexWidth < MapWidth && indexHeight < MapHeight)
					return map[indexWidth][indexHeight];
				else
					return 0;
			}
			set
			{
				if(indexWidth < MapWidth && indexHeight < MapHeight)
				{
					if(value == 1 && map[indexWidth][indexHeight] != 1) FreeFields--;
					if(value == 0 && map[indexWidth][indexHeight] != 0) FreeFields++;
					map[indexWidth][indexHeight] = value;
				}
			}
		}

		public int MapWidth
		{
			get { return map.GetLength(0); }
		}

		public int MapHeight
		{
			get
			{
				if(MapWidth == 0)
					return 0;

				return map[0].GetLength(0);
			}
		}

		/// <summary>
		/// Returns the number of non blocked fields
		/// </summary>
		/// <returns>Number of non blocked fields</returns>
		private int CalculateFreeFields()
		{
			int sum = 0;
			for(int i = 0; i < MapWidth; i++)
			{
				for(int j = 0; j < MapHeight; j++)
				{
					if(map[i][j] == 0)
						sum++;
				}
			}

			return sum;
		}
	}
}
