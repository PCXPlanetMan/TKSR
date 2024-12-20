using UnityEngine;
using System.Collections;

namespace TacticalRPG {	
	public class TransformLocalEulerTweener : Vector3Tweener 
	{
		protected override void OnUpdate ()
		{
			base.OnUpdate ();
			transform.localEulerAngles = currentTweenValue;
		}
	}
}
