using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

namespace TKSR
{
    /// <summary>
    /// 在跳转点检测Quest以及Entry状态,来判断剧情走向
    /// </summary>
    public class ScenarioQuests : QuestsChecker
    {
        [Tooltip("Play Timeline if the Quest is satisfied.")]
        public TimelineScenarioItem timeline;
        [Tooltip("Do Deployment if the Quest is satisfied.")]
        public GameObject deployment;
        [Tooltip("Do something when this Quest is satisfied.")]
        public UnityEvent OnReachQuest; // 在Quest满足条件运行时,额外做一些事情,例如设置相机的Follower
        [Tooltip("If no timeline and deployment existed. This will make NPC do a Conversation.")]
        public DialogueActor dlgActor;
    }
}