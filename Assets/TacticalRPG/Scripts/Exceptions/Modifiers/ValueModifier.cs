using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public abstract class ValueModifier : Modifier
	{
		public ValueModifier (int sortOrder) : base (sortOrder) {}
		public abstract float Modify (float fromValue, float toValue);
	}
}
