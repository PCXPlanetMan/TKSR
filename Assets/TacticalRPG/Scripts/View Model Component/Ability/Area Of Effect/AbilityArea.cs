using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalRPG {	
	public abstract class AbilityArea : MonoBehaviour
	{
		public abstract List<Tile> GetTilesInArea (Board board, Point pos);
	}
}
