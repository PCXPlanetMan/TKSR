using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class DefeatAllEnemiesVictoryCondition : BaseVictoryCondition 
	{
		protected override void CheckForGameOver ()
		{
			base.CheckForGameOver();
			if (Victor == Alliances.None && PartyDefeated(Alliances.Enemy))
				Victor = Alliances.Hero;
		}
	}
}
