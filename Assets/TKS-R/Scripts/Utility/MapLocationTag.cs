using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TKSR
{
    public class MapLocationTag : MonoBehaviour
    {
        public string locTagName;
        
#if UNITY_EDITOR
        private GUIStyle tagStyle = new GUIStyle();
        
        private void OnDrawGizmos()
        {
            tagStyle.normal.background = null;
            tagStyle.normal.textColor = Color.cyan;
            tagStyle.fontStyle = FontStyle.BoldAndItalic;
            tagStyle.fontSize = 40;
            UnityEditor.Handles.Label(this.transform.position + new Vector3(-0.5f, 0.5f, 0), locTagName, tagStyle);
        }
#endif
    }
}