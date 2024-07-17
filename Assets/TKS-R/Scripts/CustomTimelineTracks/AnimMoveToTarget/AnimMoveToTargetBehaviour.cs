using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace TKSRPlayables
{
    [Serializable]
    public class AnimMoveToTargetBehaviour : PlayableBehaviour
    {
        public FaceParam.FaceType faceType;
        public Transform targetTransform;
        public Vector2 targetPosition;
        public float animSpeed = 1f;

        [HideInInspector] public bool commandExecuted = false; //the user shouldn't author this, the Mixer does
    }
}