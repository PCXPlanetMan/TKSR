using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public enum Alliances
	{
		None = 0,
		Neutral = 1 << 0,
		Hero = 1 << 1,
		Enemy = 1 << 2
	}
}
