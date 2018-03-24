// Timeline Particle Control Example
// https://github.com/keijiro/TimelineParticleControl

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Klak.Timeline {

// Clip asset class for particle system control

[System.Serializable]
public class ParticleSystemControlClip : PlayableAsset, ITimelineClipAsset
{
    public ParticleSystemControlPlayable template = new ParticleSystemControlPlayable();

    public ClipCaps clipCaps { get { return ClipCaps.Blending; } }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        return ScriptPlayable<ParticleSystemControlPlayable>.Create(graph, template);
    }
}

}
