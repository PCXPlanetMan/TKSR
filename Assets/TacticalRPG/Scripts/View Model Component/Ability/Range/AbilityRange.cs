using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalRPG {	
	public abstract class AbilityRange : MonoBehaviour 
	{
		public int horizontal = 1;
		public int vertical = int.MaxValue;
		public virtual bool positionOriented { get { return true; }}
		public virtual bool directionOriented { get { return false; }}
		protected Unit unit { get { return GetComponentInParent<Unit>(); }}
		
		public abstract List<Tile> GetTilesInRange (Board board);
	}
}
