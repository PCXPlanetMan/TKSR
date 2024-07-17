using DigitalRubyShared;
using UnityEngine;

namespace TKSR
{
    public class TilemapTouchController : MonoBehaviour
    {
        private Grid m_parentGrid;
        private TapGestureRecognizer oneFingerTap;
        private TapGestureRecognizer twoFingerTap;
        private LongPressGestureRecognizer longPressGesture;

        void Awake()
        {
            m_parentGrid = this.GetComponentInParent<Grid>();

            // CreateDoubleTapGesture();
            CreateFingersTapGesture();
            // CreateSwipeGesture();
            CreateLongPressGesture();
        }
        
        private void OnEnable()
        {
            FingersScript.Instance.AddGesture(oneFingerTap);
            FingersScript.Instance.AddGesture(twoFingerTap);
            // FingersScript.Instance.AddGesture(doubleTapGesture);
            // FingersScript.Instance.AddGesture(swipeGesture);
            FingersScript.Instance.AddGesture(longPressGesture);
        }
        
        private void OnDisable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(oneFingerTap);
                FingersScript.Instance.RemoveGesture(twoFingerTap);
                // FingersScript.Instance.RemoveGesture(doubleTapGesture);
                // FingersScript.Instance.RemoveGesture(swipeGesture);
                FingersScript.Instance.RemoveGesture(longPressGesture);
            }
        }
        
        private void CreateFingersTapGesture()
        {
            oneFingerTap = new TapGestureRecognizer();
            oneFingerTap.StateUpdated += TapCallback;
            twoFingerTap = new TapGestureRecognizer();
            twoFingerTap.MinimumNumberOfTouchesToTrack = twoFingerTap.MaximumNumberOfTouchesToTrack = 2;
            twoFingerTap.StateUpdated += TapCallback;
            
            oneFingerTap.RequireGestureRecognizerToFail = twoFingerTap;
            FingersScript.Instance.ShowTouches = true;
        }
        
        private void TapCallback(DigitalRubyShared.GestureRecognizer tapGesture)
        {
            if (tapGesture.State == GestureRecognizerState.Ended)
            {
                int touchCount = (tapGesture as TapGestureRecognizer).TapTouches.Count;
                if (touchCount == 1)
                {
                    Vector3 vecScreen = new Vector3(tapGesture.FocusX, tapGesture.FocusY, 0);
                    string msg = string.Format("Single tap at {0},{1}", tapGesture.FocusX, tapGesture.FocusY);
                    Debug.Log(msg);
                    var hitPoint = Camera.main.ScreenToWorldPoint(vecScreen);
                    var cellPos = m_parentGrid.WorldToCell(hitPoint);
            
                    var lastPos = BattleController.Instance.pos;
                    Debug.Log($"[TKSR] Tap at transform position = {hitPoint} and Cell Tile Position = {cellPos}, lastPos={lastPos}");
                    var diffPos = new Point(cellPos.x, cellPos.y) - lastPos;
                    InputController.Instance.SimOnMoveEvent(diffPos);
                }
                else if (touchCount == 2)
                {
                    Debug.Log($"[TKSR] Double Touch to simulate Right Mouse Click.");
                    InputController.Instance.SimOnFireEvent(1);
                }
            }
        }
        
        private void CreateLongPressGesture()
        {
            longPressGesture = new LongPressGestureRecognizer();
            longPressGesture.MaximumNumberOfTouchesToTrack = 1;
            longPressGesture.StateUpdated += LongPressGestureCallback;
            
        }
        
        private void LongPressGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Began)
            {
                Debug.LogFormat("Long press began: {0}, {1}", gesture.FocusX, gesture.FocusY);
                // BeginDrag(gesture.FocusX, gesture.FocusY);
            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {
                Debug.LogFormat("Long press moved: {0}, {1}", gesture.FocusX, gesture.FocusY);
                // DragTo(gesture.FocusX, gesture.FocusY);
            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                Debug.LogFormat("Long press end: {0}, {1}, delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);
                // EndDrag(longPressGesture.VelocityX, longPressGesture.VelocityY);
                InputController.Instance.SimOnFireEvent();
            }
        }
        
        // private void CreateDoubleTapGesture()
        // {
        //     doubleTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        //     doubleTapGesture.StateUpdated += DoubleTapGesture_StateUpdated;
        // }
        //
        // private void DoubleTapGesture_StateUpdated(DigitalRubyShared.GestureRecognizer gesture)
        // {
        //     //Debug.LogFormat("Double tap state: {0}", gesture.State);
        //     if (gesture.State == GestureRecognizerState.Ended)
        //     {
        //         string msg = string.Format("Double tap at {0},{1}", gesture.FocusX, gesture.FocusY);
        //         Debug.Log(msg);
        //         InputController.Instance.SimOnFireEvent();
        //     }
        // }
        
        // private void CreateSwipeGesture()
        // {
        //     swipeGesture = new SwipeGestureRecognizer();
        //     swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
        //     swipeGesture.StateUpdated += SwipeGestureCallback;
        //     swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
        // }
        //
        // private void SwipeGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
        // {
        //     if (gesture.State == GestureRecognizerState.Ended)
        //     {
        //         SwipeGestureRecognizer swipe = gesture as SwipeGestureRecognizer;
        //         float angle = Mathf.Atan2(swipe.DistanceY, swipe.DistanceX) * Mathf.Rad2Deg;
        //         Debug.LogFormat("Swipe dir: {0}, angle = {1}", swipe.EndDirection, angle);
        //         Debug.LogFormat("Swiped from {0},{1} to {2},{3}; velocity: {4}, {5}", gesture.StartFocusX, gesture.StartFocusY, gesture.FocusX, gesture.FocusY, swipeGesture.VelocityX, swipeGesture.VelocityY);
        //
        //         if (swipe.EndDirection == SwipeGestureRecognizerDirection.Right || swipe.EndDirection == SwipeGestureRecognizerDirection.Left)
        //         {
        //             InputController.Instance.SimOnFireEvent(1);
        //         }
        //     }
        // }
    }
}