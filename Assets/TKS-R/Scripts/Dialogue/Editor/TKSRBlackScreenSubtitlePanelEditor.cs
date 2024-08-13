// Copyright (c) Pixel Crushers. All rights reserved.

using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.LivelyChatBubblesSupport;
using UnityEngine;
using UnityEditor;

namespace TKSR
{
    [CustomEditor(typeof(TKSRBlackScreenSubtitlePanel))]
    public class TKSRBlackScreenSubtitlePanelEditor : StandardUISubtitlePanelEditor
    {
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRBlackScreenSubtitlePanel.canvas)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRBlackScreenSubtitlePanel.screenDlg)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRBlackScreenSubtitlePanel.screenDlgPrefab)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(TKSRBlackScreenSubtitlePanel.halfBlack)));


            serializedObject.ApplyModifiedProperties();
        }
    }
}
