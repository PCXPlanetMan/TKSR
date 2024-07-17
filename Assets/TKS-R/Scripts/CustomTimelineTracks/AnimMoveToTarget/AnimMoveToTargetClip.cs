using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TKSRPlayables
{
    public class AnimMoveToTargetClip : PlayableAsset, ITimelineClipAsset
    {
        [HideInInspector] public AnimMoveToTargetBehaviour template = new AnimMoveToTargetBehaviour();

        public FaceParam.FaceType faceType;
        public Vector2 targetPosition;
        public ExposedReference<Transform> targetTransform;
        public float animSpeed = 1f;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AnimMoveToTargetBehaviour>.Create(graph, template);
            AnimMoveToTargetBehaviour clone = playable.GetBehaviour();
            clone.faceType = faceType;
            clone.targetPosition = targetPosition;
            clone.targetTransform = targetTransform.Resolve(graph.GetResolver());
            clone.animSpeed = animSpeed;
            return playable;

        }
    }
}