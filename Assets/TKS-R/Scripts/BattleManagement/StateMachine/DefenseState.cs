using UnityEngine;
using System.Collections;
using TacticalRPG;

namespace TKSR
{
	public class DefenseState : BattleState
	{
		public override void Enter()
		{
			base.Enter();
			StartCoroutine(DoShowDefenseEffect());
		}

		IEnumerator DoShowDefenseEffect()
		{
			Debug.Log("[TKSR] Begin show Defense Effect.");
			yield return new WaitForSeconds(1f);
			Debug.Log("[TSKR] End show Defense Effect.");
			owner.ChangeState<EndFacingState>();
		}
	}
}