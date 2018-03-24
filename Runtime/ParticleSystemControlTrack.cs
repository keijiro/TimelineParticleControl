// Timeline Particle Control Example
// https://github.com/keijiro/TimelineParticleControl

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Klak.Timeline {

// Track asset class for particle system control

[TrackColor(0.4f, 0.7f, 0.6f)]
[TrackClipType(typeof(ParticleSystemControlClip))]
[TrackBindingType(typeof(ParticleSystem))]
public class ParticleSystemControlTrack : TrackAsset
{
    public ParticleSystemControlMixer template = new ParticleSystemControlMixer();

    public void OnEnable()
    {
        if (template.randomSeed == 0xffffffff)
            template.randomSeed = (uint)Random.Range(0, 0x7fffffff);
    }

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // Retrieve the reference to the track-bound particle system via the
        // director.
        var director = go.GetComponent<PlayableDirector>();
        var ps = director.GetGenericBinding(this) as ParticleSystem;

        // Create a track mixer playable and give the reference to the particle
        // system (it has to be initialized before OnGraphStart).
        var playable = ScriptPlayable<ParticleSystemControlMixer>.Create(graph, template, inputCount);
        playable.GetBehaviour().particleSystem = ps;

        return playable;
    }

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
        //
        // In this track, the following properties will be modified.
        //
        // - transform.position
        // - transform.rotation
        // - particleSystem.useAutoRandomSeed
        // - particleSystem.main.duration
        // - particleSystem.emission.rateOverTimeMultiplier
        // - particleSystem.emission.rateOverDistanceMultiplier
        //
        // Note that the serialized property names are a bit defferent from
        // their property name.
        //

        var ps = director.GetGenericBinding(this) as ParticleSystem;
        if (ps == null) return;

        var go = ps.gameObject;

        driver.AddFromName<Transform>(go, "m_LocalPosition");
        driver.AddFromName<Transform>(go, "m_LocalRotation");

        driver.AddFromName<ParticleSystem>(go, "lengthInSec");
        driver.AddFromName<ParticleSystem>(go, "autoRandomSeed");
        driver.AddFromName<ParticleSystem>(go, "randomSeed");

        driver.AddFromName<ParticleSystem>(go, "EmissionModule.rateOverTime.scalar");
        driver.AddFromName<ParticleSystem>(go, "EmissionModule.rateOverDistance.scalar");
    }
}

}
