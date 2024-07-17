using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class AttackState : BattleState
    {
        public override void Enter()
        {
            base.Enter();
            StartCoroutine(DoShowAttack());
        }
        
        /// <summary>
        /// 必须要要延迟进行AbilityTargetState的切换,否则状态机的Transition不能保证状态的正确切换
        /// </summary>
        /// <returns></returns>
        IEnumerator DoShowAttack()
        {
            yield return new WaitForEndOfFrame();
            Attack();
        }
        
        void Attack ()
        {
            // [TKSR] 角色对象节点下有且唯一只有一个代表普通Attack的Ability组件
            turn.ability = turn.actor.GetComponentInChildren<Ability>();
            owner.ChangeState<AbilityTargetState>();
        }
    }
}
