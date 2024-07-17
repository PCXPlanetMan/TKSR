using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class ScenarioSetter : QuestsSetter
    {
        public GameObject targetNPC;

        protected override void Awake()
        {
            // 如果ScenarioSetter与PatrolPathDrawer同时存在,可以预先在PatrolPathDrawer的Awake中卸载旧的ScenarioLauncher
        }

        void Start()
        {
            if (targetNPC != null)
            {
                var launcher = targetNPC.AddComponent<ScenarioLauncher>();

                launcher.listQuestStateItemsCondition = base.listQuestStateItemsCondition;
                launcher.listQuestEntryStateItemsCondition = base.listQuestEntryStateItemsCondition;
                launcher.listQuestStateItemsResult = base.listQuestStateItemsResult;
                launcher.listQuestEntryStateItemsResult = base.listQuestEntryStateItemsResult;
            }
        }
    }
}