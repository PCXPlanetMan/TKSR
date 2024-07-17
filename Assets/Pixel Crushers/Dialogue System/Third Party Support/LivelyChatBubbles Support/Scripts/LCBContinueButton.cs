// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using LivelyChatBubbles;

namespace PixelCrushers.DialogueSystem.LivelyChatBubblesSupport
{

    /// <summary>
    /// Continue button fast forward functionality for LivelyChatBubbleSubtitlePanels.
    /// Add to the subtitle panel's Continue Button and configure OnClick() to call OnFastForward().
    /// </summary>
    public class LCBContinueButton : MonoBehaviour
    {
        public ChatBubble chatBubble;

        private void Awake()
        {
            if (chatBubble == null) chatBubble = GetComponentInParent<ChatBubble>();
        }

        public void OnFastForward()
        {
            if (chatBubble == null) Debug.LogError(name + " doesn't have a parent ChatBubble.", this);
            var chatProcesser = chatBubble.GetComponent<ChatOutputProcesser>();
            if (chatBubble.MessageComponent.text != chatProcesser.Value)
            {
                chatProcesser.StopAllCoroutines();
                chatBubble.MessageComponent.text = chatProcesser.Value;
            }
            else
            {
                (DialogueManager.dialogueUI as StandardDialogueUI).OnContinueConversation();
            }
        }
    }
}
