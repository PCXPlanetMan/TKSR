using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TKSR
{
    [CustomEditor(typeof(TimelineScenarioItem))]
    public class TimelineScenarioItemEditor : Editor
    {
        private SerializedProperty m_DebugScenarioDescProp;
        SerializedProperty m_FinishTransitionTypeProp;
        SerializedProperty m_ToNewSceneNameProp;
        SerializedProperty m_TransitionDestinationTagProp;
        private SerializedProperty m_DestinationPointProp;
        SerializedProperty m_ToBattleSceneNameProp;
        private SerializedProperty m_FinishMainPlayerDirectionProp;
        private SerializedProperty m_AudioBGMProp;
        private SerializedProperty m_AttachedCollidersProp;
        private SerializedProperty m_SelectorSwitchesProp;
        private SerializedProperty m_AtachedEffectsProp;
        private SerializedProperty m_AttachedPathNodesProp;

        
        void OnEnable ()
        {
            m_DebugScenarioDescProp = serializedObject.FindProperty("debugScenarioDesc");

            m_FinishTransitionTypeProp = serializedObject.FindProperty("finishTransitionType");
            m_ToNewSceneNameProp = serializedObject.FindProperty("toNewSceneName");
            m_TransitionDestinationTagProp = serializedObject.FindProperty("transitionDestinationTag");
            m_DestinationPointProp = serializedObject.FindProperty ("destinationPoint");
            m_ToBattleSceneNameProp = serializedObject.FindProperty("toBattleSceneName");
            m_FinishMainPlayerDirectionProp = serializedObject.FindProperty("finishedMainPlayerDirection");
            m_AttachedCollidersProp = serializedObject.FindProperty("attachedColliders");
            
            m_SelectorSwitchesProp = serializedObject.FindProperty("selectorSwitches");
            
            m_AtachedEffectsProp = serializedObject.FindProperty("attachedEffects");
            m_AttachedPathNodesProp = serializedObject.FindProperty("attachedPathNodes");
            
            m_AudioBGMProp = serializedObject.FindProperty("audioBGM");
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            #if UNITY_EDITOR && TKSR_DEV
            EditorGUILayout.PropertyField(m_DebugScenarioDescProp);
            #endif
        
            EditorGUILayout.PropertyField(m_FinishTransitionTypeProp);
            EditorGUI.indentLevel++;
            if ((TransitionPoint.TransitionType)m_FinishTransitionTypeProp.enumValueIndex == TransitionPoint.TransitionType.SameScene)
            {
                EditorGUILayout.PropertyField (m_DestinationPointProp);
            }
            else if ((TransitionPoint.TransitionType)m_FinishTransitionTypeProp.enumValueIndex ==
                     TransitionPoint.TransitionType.BattleScene)
            {
                EditorGUILayout.PropertyField (m_ToBattleSceneNameProp);
            }
            else if ((TransitionPoint.TransitionType)m_FinishTransitionTypeProp.enumValueIndex ==
                     TransitionPoint.TransitionType.DifferentZone)
            {
                EditorGUILayout.PropertyField (m_ToNewSceneNameProp);
                EditorGUILayout.PropertyField (m_TransitionDestinationTagProp);
            }
            else 
            {
                EditorGUILayout.PropertyField (m_FinishMainPlayerDirectionProp);
            }
            
            EditorGUILayout.PropertyField(m_SelectorSwitchesProp);
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(m_AttachedCollidersProp);
            
            EditorGUILayout.PropertyField(m_AudioBGMProp);
            EditorGUILayout.PropertyField(m_AtachedEffectsProp);
            EditorGUILayout.PropertyField(m_AttachedPathNodesProp);
            
            
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}