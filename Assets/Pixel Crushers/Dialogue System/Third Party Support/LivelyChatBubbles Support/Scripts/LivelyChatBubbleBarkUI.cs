// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using LivelyChatBubbles;

namespace PixelCrushers.DialogueSystem.LivelyChatBubblesSupport
{

    /// <summary>
    /// Bark UI using Lively Chat Bubbles.
    /// </summary>
    public class LivelyChatBubbleBarkUI : AbstractBarkUI
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

        [Tooltip("The duration in seconds to show the bark text before fading it out. If zero, use the Dialogue Manager's Bark Settings.")]
        public float duration = 4f;

        protected ChatOutputProcesser m_chatProcesser;
        protected int numSequencesActive = 0;
        protected float timeLeft = 0;

        public override bool isPlaying
        {
            get
            {
                return (chatBubble != null) && chatBubble.gameObject.activeInHierarchy && (timeLeft > 0);
            }
        }

        public override void Bark(Subtitle subtitle)
        {
            chatBubble.BindNameValue(subtitle.speakerInfo.Name);
            m_chatProcesser.Value = subtitle.formattedText.text;
            m_chatProcesser.enabled = true;
            chatBubble.gameObject.SetActive(true);
            if (chatBubble.MessageComponent != null) chatBubble.MessageComponent.text = string.Empty;
            timeLeft = Mathf.Approximately(0, duration) ? DialogueManager.GetBarkDuration(subtitle.formattedText.text) : duration;
        }

        public override void Hide()
        {
            timeLeft = 0;
            m_chatProcesser.enabled = false;
            chatBubble.gameObject.SetActive(false);
        }

        protected virtual void Awake()
        {
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
        }

        protected void Update()
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0) Hide();
            }
        }

    }
}
