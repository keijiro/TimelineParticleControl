using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.4f, 0.4f, 0.4f)]
[TrackClipType(typeof(ParticleSystemControl))]
[TrackBindingType(typeof(Transform))]
public class ParticleSystemControlTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<ParticleSystemControlMixer>.Create(graph, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        var transform = director.GetGenericBinding(this) as Transform;
        if (transform == null) return;

        foreach (var ps in transform.GetComponentsInChildren<ParticleSystem>())
        {
            driver.AddFromName<ParticleSystem>(ps.gameObject, "randomSeed");
            driver.AddFromName<ParticleSystem>(ps.gameObject, "autoRandomSeed");
        }
    }
}
