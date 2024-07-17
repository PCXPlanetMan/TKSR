using System.Collections;
using System.Collections.Generic;
using TKSR;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TKSRPlayables
{
    [TrackColor(0f, 0.4866645f, 1f)]
    [TrackClipType(typeof(AnimMoveToTargetClip))]
    [TrackBindingType(typeof(ICharacterSpriteRender))]
    public class AnimMoveToTargetTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var c in GetClips())
            {
                //Clips are renamed after the actionType of the clip itself
                AnimMoveToTargetClip clip = (AnimMoveToTargetClip)c.asset;
                c.displayName = clip.faceType.ToString();
            }

            return ScriptPlayable<AnimMoveToTargetMixerBehaviour>.Create(graph, inputCount);
        }
    }
}