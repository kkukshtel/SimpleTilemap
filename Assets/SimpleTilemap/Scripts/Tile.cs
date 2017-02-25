using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimpleTilemap
{
    public class Tile {
        public int x;
		public int y;
		public Tile(int xCord, int yCord)
		{
			x = xCord;
			y = yCord;
		}
    }
}