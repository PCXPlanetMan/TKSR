using UnityEditor;
using UnityEngine;

namespace TKSR
{
    [CustomEditor(typeof(QuestsChecker))]
    public class QuestsCheckerEditor : Editor
    {
        private SerializedProperty m_ListQuestStateItemsProp;
        private SerializedProperty m_ListQuestEntryStateItemsProp;
        private SerializedProperty m_QuestDestinationTagProp;
        
        void OnEnable ()
        {
            m_ListQuestStateItemsProp = serializedObject.FindProperty("listQuestStateItems");
            m_ListQuestEntryStateItemsProp = serializedObject.FindProperty("listQuestEntryStateItems");
            m_QuestDestinationTagProp = serializedObject.FindProperty("questDestinationTag");
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();
            
            // EditorGUILayout.PropertyField(m_DialogueDatabaseProp);
            // if (m_DialogueDatabaseProp.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_ListQuestStateItemsProp);
                EditorGUILayout.PropertyField(m_ListQuestEntryStateItemsProp);
                EditorGUILayout.PropertyField(m_QuestDestinationTagProp);
            }
            
            serializedObject.ApplyModifiedProperties ();
        }
    }
}