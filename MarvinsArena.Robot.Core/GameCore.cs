using System;

namespace MarvinsArena.Robot.Core
{
	public class GameCore
	{
		private byte[][] map;

		public GameCore(byte[][] map)
		{
			this.map = map;
		}

		public int MapFieldsHeight
		{
			get { return map[0].GetLength(0);  }
		}

		public int MapFieldsWidth
		{
			get { return map.GetLength(0); }
		}

		public int MapHeight
		{
			get { return MapFieldsHeight * 32; }
		}

		public int MapWidth
		{
			get { return MapFieldsWidth * 32; }
		}

		public int MapCanEnter(int indexWidth, int indexHeight)
		{
			if(indexWidth >= 0 && indexHeight >= 0 &&
				indexWidth < MapFieldsWidth && indexHeight < MapFieldsHeight)
			{
				return map[indexWidth][indexHeight];
			} else
			{
				return -1;
			}
		}
	}
}
