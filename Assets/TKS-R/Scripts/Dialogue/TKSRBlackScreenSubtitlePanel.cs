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
    public class TKSRBlackScreenSubtitlePanel : StandardUISubtitlePanel
    {
        
        [Tooltip("Canvas where all chat bubble instances will be created.")]
        [HideInInspector]
        public Canvas canvas;

        public TKSRBlackScrenDlg screenDlg;
        public TKSRBlackScrenDlg screenDlgPrefab;
        public bool halfBlack = false; // [TKSR] TODO:专用于做对话黑屏衔接(例如在对话中显示半黑屏,然后进入剧情显示另一半黑屏。北平赵云化妆)

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
            
            if (screenDlg == null && screenDlgPrefab != null)
            {
                screenDlg = GameObject.Instantiate(screenDlgPrefab, canvas.transform);
            }
            if (screenDlg == null)
            {
                Debug.LogWarning("Dialogue System: " + name + " is missing a Chat Bubble.", this);
                return;
            }

            screenDlg.parentPanel = this;
            screenDlg.gameObject.SetActive(false);

            // Look for continue button:
            if (continueButton == null)
            {
                continueButton = screenDlg.GetComponentInChildren<UnityEngine.UI.Button>();
            }
        }

        // protected override void SetUIElementsActive(bool value)
        // {
        //     base.SetUIElementsActive(value);
        // }

        public override void ShowSubtitle(Subtitle subtitle)
        {
            //base.ShowSubtitle(subtitle);
            isSpeaking = true;
            screenDlg.BindMessageValue(subtitle.formattedText.text);
            // screenDlg.gameObject.SetActive(true);
            screenDlg.ShowBlack();
        }

        public override void HideSubtitle(Subtitle subtitle)
        {
            //base.HideSubtitle(subtitle);
            HideImmediate();
        }

        public override void HideImmediate()
        {
            //base.HideImmediate();
            if (screenDlg != null)
            {
                // screenDlg.gameObject.SetActive(false);
                screenDlg.HideBlack();
            }
        }

        public override void ClearText()
        {
            base.ClearText();
            screenDlg.MessageComponent.text = string.Empty;
        }
        
        public void ContinueWhenChatFinished()
        {
            base.OnContinue();
        }

        private TeleportMainPlayer m_targetTeleport;
        public void SetTeleportCheckPoint(TeleportMainPlayer teleport)
        {
            m_targetTeleport = teleport;
        }

        public void DoTeleportMainPlayer()
        {
            if (m_targetTeleport)
            {
                var mainPlayer = PlayerCharacter.PlayerInstance;
                
                var pathTargetPos = m_targetTeleport.teleportNode.position;
                Vector2 target = new Vector2(pathTargetPos.x, pathTargetPos.y);
                var current = mainPlayer.Controller2D.Rigidbody2D.position;
                mainPlayer.Controller2D.Rigidbody2D.position = target;
                mainPlayer.SetMoveVector(Vector2.zero);
                mainPlayer.UpdateFacingDirection(m_targetTeleport.finishDirection);
            }

            m_targetTeleport = null;
        }
    }
}
