using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace TKSR
{
    /// <summary>
    /// 在地图出入口添加Quest检测,主要用于在某些剧情中关闭进入其他场景的可能性
    /// 如果满足条件,则关闭此入口
    /// </summary>
    public class TransitionPointQuests : QuestsChecker
    {
        private Collider2D m_collider2D;
        void Awake()
        {
            RunQuestToDisableCollider();
        }

        public void RunQuestToDisableCollider()
        {
            if (m_collider2D == null)
            {
                m_collider2D = this.gameObject.GetComponent<Collider2D>();
            }

            if (m_collider2D == null)
            {
                Debug.LogError("[TKSR] No Collider 2D Exist.");
                return;
            }

            bool initColliderEnabled = m_collider2D.enabled;

            if (base.AreConditionsSatisfied() && m_collider2D != null)
            {
                m_collider2D.enabled = initColliderEnabled ? false : true;
                return;
            }

            if (m_collider2D != null)
            {
                m_collider2D.enabled = initColliderEnabled ? true : false;
            }
        }
    }
}