// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;


namespace TKSR
{
    /// <summary>
    /// Subclass of StandardUISubtitlePanel that uses Lively Chat Bubbles.
    /// Since it's a subclass of StandardUISubtitlePanel, you can assign it to a
    /// Dialogue Actor's Custom Subtitle Panel field.
    /// </summary>
    public class TKSRSelectorSubtitlePanel : StandardUISubtitlePanel
    {
        private Canvas canvas;

        private TKSRSelectorDlg selectorDlg;
        public TKSRSelectorDlg selectorDlgPrefab;
        
        protected TimelineScenarioItem fromTimeline;
        
        public bool isSpeaking { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            isSpeaking = false;
            if (canvas == null)
            {
                canvas = DialogueManager.instance.GetComponentInChildren<Canvas>();
                if (canvas == null) canvas = FindObjectOfType<Canvas>();
            }
            if (canvas == null)
            {
                Debug.LogWarning("Dialogue System: " + name + " is missing a Canvas assignment.", this);
                return;
            }
            
            if (selectorDlg == null && selectorDlgPrefab != null)
            {
                selectorDlg = GameObject.Instantiate(selectorDlgPrefab, canvas.transform);
            }
            if (selectorDlg == null)
            {
                Debug.LogWarning("Dialogue System: " + name + " is missing a Chat Bubble.", this);
                return;
            }

            selectorDlg.parentPanel = this;

            HideResponses();
        }

        public void ShowResponses(Response[] arrayOfResponses, TimelineScenarioItem parentTimeline)
        {
            selectorDlg.gameObject.SetActive(true);
            // 不在TKSRSelectorSubtitlePanel上注册此消息的原因是,TKSRSelectorSubTitlePanel会在完成一次对话中自动Disable,导致无法接受此消息.
            selectorDlg.BindResponsesValue(arrayOfResponses, parentTimeline.transform);

            fromTimeline = parentTimeline;
        }
        
        public void HideResponses()
        {
            selectorDlg.ClearResponseButtons();
            selectorDlg.gameObject.SetActive(false);
        }
        
        // 不在TKSRSelectorSubtitlePanel上注册此消息的原因是,TKSRSelectorSubTitlePanel会在完成一次对话中自动Disable,导致无法接受此消息.
        // public void OnClick(object data)
        // {
        //     var resp = data as Response;
        //     Debug.Log($"[TKSR] OnClick Response conversationID = {resp.destinationEntry.conversationID}, EntryId = {resp.destinationEntry.id}");
        //     
        //     HideResponses();
        //
        //     if (fromTimeline != null)
        //     {
        //         fromTimeline.SwitchNextActionBySelectorResponse(resp);
        //     }
        // }
    }
}
