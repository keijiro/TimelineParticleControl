using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.4f, 0.4f, 0.4f)]
[TrackClipType(typeof(ParticleSystemControl))]
[TrackBindingType(typeof(Transform))]
public class ParticleSystemControlTrack : TrackAsset
{
    public ParticleSystemControlMixer template = new ParticleSystemControlMixer();

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<ParticleSystemControlMixer>.Create(graph, template, inputCount);
    }
}
