using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class ScenarioLauncher : QuestsSetter
    {
        protected override void Awake()
        {
            // 用于点击NPC对话的时候直接触发当前入口的Quests
            // 故需要重载Awake()函数
        }
        
        public void DestroyLauncher()
        {
            Destroy(this);
        }
    }
}