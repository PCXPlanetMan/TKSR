using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalRPG {	
	public class InfiniteAbilityRange : AbilityRange 
	{
		public override bool positionOriented { get { return false; }}
	
		public override List<Tile> GetTilesInRange (Board board)
		{
			return new List<Tile>(board.tiles.Values);
		}
	}
}
