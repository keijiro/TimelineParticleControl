// Timeline Particle Control Example
// https://github.com/keijiro/TimelineParticleControl

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// Track asset class for particle system control

[TrackColor(0.4f, 0.7f, 0.6f)]
[TrackClipType(typeof(ParticleSystemControl))]
[TrackBindingType(typeof(ParticleSystem))]
public class ParticleSystemControlTrack : TrackAsset
{
    public ParticleSystemControlMixer template = new ParticleSystemControlMixer();

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<ParticleSystemControlMixer>.Create(graph, template, inputCount);
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        var ps = director.GetGenericBinding(this) as ParticleSystem;
        if (ps == null) return;

        driver.AddFromName<Transform>(ps.gameObject, "m_LocalPosition");
        driver.AddFromName<Transform>(ps.gameObject, "m_LocalRotation");

        driver.AddFromName<ParticleSystem>(ps.gameObject, "EmissionModule.rateOverTime.scalar");
        driver.AddFromName<ParticleSystem>(ps.gameObject, "EmissionModule.rateOverDistance.scalar");
    }
}
