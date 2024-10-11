using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public abstract class AbilityEffectTarget : MonoBehaviour 
	{
		public abstract bool IsTarget (Tile tile);
	}
}
