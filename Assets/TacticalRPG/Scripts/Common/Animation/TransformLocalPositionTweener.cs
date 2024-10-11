using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class TransformLocalPositionTweener : Vector3Tweener 
	{
		protected override void OnUpdate ()
		{
			base.OnUpdate ();
			transform.localPosition = currentTweenValue;
		}
	}
}
