using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class ParticleSystemControl : PlayableAsset, ITimelineClipAsset
{
    public ParticleSystemControlPlayable template = new ParticleSystemControlPlayable();

    public ClipCaps clipCaps { get { return ClipCaps.ClipIn; } }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        return ScriptPlayable<ParticleSystemControlPlayable>.Create(graph, template);
    }
}
