using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalRPG {	
	public class Ability : MonoBehaviour 
	{
		public const string CanPerformCheck = "Ability.CanPerformCheck";
		public const string FailedNotification = "Ability.FailedNotification";
		public const string DidPerformNotification = "Ability.DidPerformNotification";
	
		public bool CanPerform ()
		{
			BaseException exc = new BaseException(true);
			this.PostNotification(CanPerformCheck, exc);
			return exc.toggle;
		}
	
		public void Perform (List<Tile> targets)
		{
			if (!CanPerform())
			{
				this.PostNotification(FailedNotification);
				return;
			}
	
			for (int i = 0; i < targets.Count; ++i)
				Perform(targets[i]);
	
			this.PostNotification(DidPerformNotification);
		}
	
		public bool IsTarget (Tile tile)
		{
			Transform obj = transform;
			for (int i = 0; i < obj.childCount; ++i)
			{
				AbilityEffectTarget targeter = obj.GetChild(i).GetComponent<AbilityEffectTarget>();
				if (targeter.IsTarget(tile))
					return true;
			}
			return false;
		}
	
		void Perform (Tile target)
		{
			for (int i = 0; i < transform.childCount; ++i)
			{
				Transform child = transform.GetChild(i);
				BaseAbilityEffect effect = child.GetComponent<BaseAbilityEffect>();
				effect.Apply(target);
			}
		}
	}
}
