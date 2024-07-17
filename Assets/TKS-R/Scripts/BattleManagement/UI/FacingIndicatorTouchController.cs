using System;
using UnityEngine;

namespace TKSR
{
    public class FacingIndicatorTouchController : MonoBehaviour
    {
        public int Index;
        
        private FacingIndicator m_parentFacingIndicator;

        void Awake()
        {
            m_parentFacingIndicator = this.GetComponentInParent<FacingIndicator>();
        }

        private void OnEnable()
        {
            // GetComponent<TapGesture>().Tapped += TappedHandler;
        }

        private void OnDisable()
        {
            // GetComponent<TapGesture>().Tapped -= TappedHandler;
        }

        private void TappedHandler(object sender, EventArgs eventArgs)
        {
            // var gesture = sender as TapGesture;
            // var hit = gesture.GetScreenPositionHitData();
            // var cellPos = m_parentGrid.WorldToCell(hit.Point);
            // Debug.Log($"[TKSR] Tap at transform position = {hit.Point} and Cell Tile Position = {cellPos}");
            m_parentFacingIndicator.OnClickArrow(Index);
        }
    }
}