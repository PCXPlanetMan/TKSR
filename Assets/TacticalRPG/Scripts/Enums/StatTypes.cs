using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public enum StatTypes
	{
		LVL, // Level
		EXP, // Experience
		HP,  // Hit Points
		MHP, // Max Hit Points
		MP,  // Magic Points
		MMP, // Max Magic Points
		ATK, // Physical Attack
		DEF, // Physical Defense
		MAT, // Magic Attack
		MDF, // Magic Defense
		HIT, // Hit Ratio
		EVD, // Evade
		SPD, // Speed
		MOV, // Move Range
		RES, // Status Resistance
		JMP, // Jump Height
		CTR, // Counter - for turn order
		Count
	}
}
