// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem.LivelyChatBubblesSupport
{
    [CustomEditor(typeof(LivelyChatBubbleSubtitlePanel))]
    public class LivelyChatBubbleSubtitlePanelEditor : StandardUISubtitlePanelEditor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.canvas)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.chatBubble)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.chatBubblePrefab)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.chatAnchor)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.audioSource)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.chatOutputProfile)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.continueButton)));

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.onOpen)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.onClose)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.onFocus)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.onUnfocus)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.onBackButtonDown)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(LivelyChatBubbleSubtitlePanel.onOpen)));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
