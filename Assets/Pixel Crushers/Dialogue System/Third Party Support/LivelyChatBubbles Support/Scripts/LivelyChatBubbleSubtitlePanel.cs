// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using LivelyChatBubbles;

namespace PixelCrushers.DialogueSystem.LivelyChatBubblesSupport
{

    /// <summary>
    /// Subclass of StandardUISubtitlePanel that uses Lively Chat Bubbles.
    /// Since it's a subclass of StandardUISubtitlePanel, you can assign it to a
    /// Dialogue Actor's Custom Subtitle Panel field.
    /// </summary>
    public class LivelyChatBubbleSubtitlePanel : StandardUISubtitlePanel
    {
        [Tooltip("Canvas where all chat bubble instances will be created.")]
        public Canvas canvas;

        public ChatBubble chatBubble;
        public ChatBubble chatBubblePrefab;

        [Tooltip("Chat anchor to attach all instantiated bubbles.")]
        public ChatAnchor chatAnchor;

        [Tooltip("Audio information to play while speaking.")]
        public AudioSource audioSource;

        [Tooltip("Chat output information used when spoken such as characters per second, etc.")]
        public ChatOutputProfile chatOutputProfile;

        protected ChatOutputProcesser m_chatProcesser;
        protected bool m_addedContinueButtonListener = false;

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
            if (chatBubble == null && chatBubblePrefab != null)
            {
                chatBubble = GameObject.Instantiate(chatBubblePrefab, new Vector3(-100000, -10000, 0), Quaternion.identity, canvas.transform);
            }
            if (chatBubble == null)
            {
                Debug.LogWarning("Dialogue System: " + name + " is missing a Chat Bubble.", this);
                return;
            }
            if (chatAnchor == null)
            {
                Debug.LogWarning("Dialogue System: " + name + " is missing a Chat Anchor.", this);
                return;
            }
            chatAnchor.BindAttachedBubble(chatBubble);
            m_chatProcesser = chatBubble.gameObject.AddComponent<ChatOutputProcesser>();
            m_chatProcesser.enabled = false;
            m_chatProcesser.Profile = chatOutputProfile;
            m_chatProcesser.AudioSource = audioSource;
            m_chatProcesser.enabled = true;
            chatBubble.gameObject.SetActive(false);

            // Look for continue button:
            if (continueButton == null)
            {
                continueButton = chatBubble.GetComponentInChildren<UnityEngine.UI.Button>();
            }
        }

        protected override void SetUIElementsActive(bool value)
        {
            //base.SetUIElementsActive(value);
        }

        public override void ShowSubtitle(Subtitle subtitle)
        {
            //base.ShowSubtitle(subtitle);
            chatBubble.BindNameValue(subtitle.speakerInfo.Name);
            isSpeaking = true;
            m_chatProcesser.Value = subtitle.formattedText.text;
            m_chatProcesser.enabled = true;
            if (chatBubble is ChatBubbleWithPortrait)
            {
                var chatBubbleWithPortrait = chatBubble as ChatBubbleWithPortrait;
                if (chatBubbleWithPortrait.portraitImage != null)
                {
                    var portraitSprite = subtitle.GetSpeakerPortrait();
                    chatBubbleWithPortrait.portraitImage.sprite = portraitSprite;
                    chatBubbleWithPortrait.portraitImage.gameObject.SetActive(portraitSprite != null);
                }
            }
            chatBubble.gameObject.SetActive(true);
        }

        public override void HideSubtitle(Subtitle subtitle)
        {
            //base.HideSubtitle(subtitle);
            HideImmediate();
        }

        public override void HideImmediate()
        {
            //base.HideImmediate();
            if (m_chatProcesser != null)  m_chatProcesser.enabled = false;
            if (chatBubble != null) chatBubble.gameObject.SetActive(false);
        }

        public override void ClearText()
        {
            base.ClearText();
            chatBubble.MessageComponent.text = string.Empty;
        }
    }
}
