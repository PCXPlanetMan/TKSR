using Cinemachine.Editor;
using UnityEditor;
using UnityEngine;

namespace TKSR
{
    [CustomEditor(typeof(SceneTransitionDestination))]
    public class TransitionDestinationEditor : Editor
    {
        private SerializedProperty m_DestinationTagProp;
        private SerializedProperty m_TransitioningGameObjectProp;
        private SerializedProperty m_OnReachDestinationProp;
        private SerializedProperty m_DefaultDeploymentProp;
        
        void OnEnable ()
        {
            m_DestinationTagProp = serializedObject.FindProperty("destinationTag");
            m_TransitioningGameObjectProp = serializedObject.FindProperty("transitioningGameObject");
            m_OnReachDestinationProp = serializedObject.FindProperty("OnReachDestination");
            m_DefaultDeploymentProp = serializedObject.FindProperty("defaultDeployment");
        }

        public override void OnInspectorGUI ()
        {
            serializedObject.Update ();

            EditorGUILayout.PropertyField(m_DestinationTagProp);
            EditorGUILayout.PropertyField(m_TransitioningGameObjectProp);
            EditorGUILayout.PropertyField(m_DefaultDeploymentProp);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_OnReachDestinationProp);
            
            serializedObject.ApplyModifiedProperties ();
        }
    }
}