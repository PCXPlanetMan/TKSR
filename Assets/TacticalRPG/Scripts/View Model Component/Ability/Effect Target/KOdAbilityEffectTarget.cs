using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class KOdAbilityEffectTarget : AbilityEffectTarget 
	{
		public override bool IsTarget (Tile tile)
		{
			if (tile == null || tile.content == null)
				return false;
	
			Stats s = tile.content.GetComponent<Stats>();
			return s != null && s[StatTypes.HP] <= 0;
		}
	}
}
