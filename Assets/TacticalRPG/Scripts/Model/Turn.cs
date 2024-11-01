using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalRPG {	
	public class Turn 
	{
		public Unit actor;
		public bool hasUnitMoved;
		public bool hasUnitActed;
		public bool lockMove;
		public Ability ability;
		public List<Tile> targets;
		public PlanOfAttack plan;
		Tile startTile;
		Directions startDir;
	
		public void Change (Unit current)
		{
			actor = current;
			hasUnitMoved = false;
			hasUnitActed = false;
			lockMove = false;
			startTile = actor.tile;
			startDir = actor.dir;
			plan = null;
		}
	
		public void UndoMove ()
		{
			hasUnitMoved = false;
			actor.Place(startTile);
			actor.dir = startDir;
			actor.Match();
		}
	}
}
