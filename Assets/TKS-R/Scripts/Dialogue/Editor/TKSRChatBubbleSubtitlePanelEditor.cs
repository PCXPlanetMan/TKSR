// Copyright (c) Pixel Crushers. All rights reserved.

using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.LivelyChatBubblesSupport;
using UnityEngine;
using UnityEditor;

namespace TKSR
{
    [CustomEditor(typeof(TKSRChatBubbleSubtitlePanel))]
    public class TKSRChatBubbleSubtitlePanelEditor : StandardUISubtitlePanelEditor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.canvas)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.chatBubble)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.chatBubblePrefab)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.chatAnchor)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.chatOutputProfile)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.continueButton)));

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.onOpen)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.onClose)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.onFocus)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.onUnfocus)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRChatBubbleSubtitlePanel.onBackButtonDown)));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
