// Timeline Particle Control Example
// https://github.com/keijiro/TimelineParticleControl

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Klak.Timeline {

// Playable track class for particle system control

[System.Serializable]
public class ParticleSystemControlMixer : PlayableBehaviour
{
    #region Editable properties

    public ExposedReference<Transform> snapTarget;
    public uint randomSeed = 0xffffffff;

    #endregion

    #region Runtime properties

    public ParticleSystem particleSystem { get; set; }

    #endregion

    #region Private variables and methods

    Transform _snapTarget;
    bool _needRestart;

    void PrepareParticleSystem(Playable playable)
    {
        // Disable automatic random seed to get deterministic results.
        if (particleSystem.useAutoRandomSeed)
            particleSystem.useAutoRandomSeed = false;

        // Override the random seed number.
        if (particleSystem.randomSeed != randomSeed)
            particleSystem.randomSeed = randomSeed;

        // Retrieve the total duration of the track.
        var rootPlayable = playable.GetGraph().GetRootPlayable(0);
        var duration = (float)rootPlayable.GetDuration();

        // Particle system duration should be longer than the track duration.
        var main = particleSystem.main;
        if (main.duration < duration) main.duration = duration;
    }

    void ResetSimulation(float time)
    {
        const float maxSimTime = 2.0f / 3;

        if (time < maxSimTime)
        {
            // The target time is small enough: Use the default simulation
            // function (restart and simulate for the given time).
            particleSystem.Simulate(time);
        }
        else
        {
            // The target time is larger than the threshold: The default
            // simulation can be heavy in this case, so use fast-forward
            // (simulation with just a single step) then simulate for a small
            // period of time.
            particleSystem.Simulate(time - maxSimTime, true, true, false);
            particleSystem.Simulate(maxSimTime, true, false, true);
        }
    }

    #endregion

    #region PlayableBehaviour overrides

    public override void OnGraphStart(Playable playable)
    {
        if (particleSystem == null) return;

        if (Application.isPlaying)
        {
            // Play mode: Prepare particle system only on graph start.
            particleSystem.Stop();
            PrepareParticleSystem(playable);
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (particleSystem == null) return;

        // Do nothing if the target game object is not active.
        // Will do full restart when being activated next time.
        if (!particleSystem.gameObject.activeInHierarchy)
        {
            _needRestart = true;
            return;
        }

        // Retrieve the track time (playhead position) from the root playable.
        var rootPlayable = playable.GetGraph().GetRootPlayable(0);
        var time = (float)rootPlayable.GetTime();

        // Edit mode: Re-prepare the particle system every frame.
        if (!Application.isPlaying && !particleSystem.isPlaying)
            PrepareParticleSystem(playable);

        // Resolve the snapping target reference.
        // Play mode: Resolve once and keep the reference.
        // Edit mode: Re-resolve every frame.
        if (_snapTarget == null || !Application.isPlaying)
        {
            _snapTarget = snapTarget.Resolve(playable.GetGraph().GetResolver());
            if (_snapTarget == null) _snapTarget = particleSystem.transform;
        }

        // Transform snapping
        if (_snapTarget != particleSystem.transform)
        {
            particleSystem.transform.position = _snapTarget.position;
            particleSystem.transform.rotation = _snapTarget.rotation;
        }

        // Emission rates control
        var totalOverTime = 0.0f;
        var totalOverDist = 0.0f;

        var clipCount = playable.GetInputCount();
        for (var i = 0; i < clipCount; i++)
        {
            var clip = ((ScriptPlayable<ParticleSystemControlPlayable>)playable.GetInput(i)).GetBehaviour();
            var w = playable.GetInputWeight(i);
            totalOverTime += clip.rateOverTime * w;
            totalOverDist += clip.rateOverDistance * w;
        }

        var em = particleSystem.emission;
        em.rateOverTimeMultiplier = totalOverTime;
        em.rateOverDistanceMultiplier = totalOverDist;

        // Time control
        if (Application.isPlaying)
        {
            // Play mode time control: Only resets the simulation when a large
            // gap between the time variables was found.
            var maxDelta = Mathf.Max(1.0f / 30, Time.smoothDeltaTime * 2);

            if (Mathf.Abs(time - particleSystem.time) > maxDelta)
            {
                ResetSimulation(time);
                particleSystem.Play();
            }
        }
        else
        {
            // Edit mode time control
            var minDelta = 1.0f / 240;
            var smallDelta = Mathf.Max(0.1f, Time.fixedDeltaTime * 2);
            var largeDelta = 0.2f;

            // Do full restart on reactivation.
            if (_needRestart)
            {
                particleSystem.Play();
                _needRestart = false;
            }

            if (time < particleSystem.time ||
                time > particleSystem.time + largeDelta)
            {
                // Backward seek or big leap
                // Reset the simulation with the current playhead position.
                ResetSimulation(time);
            }
            else if (time > particleSystem.time + smallDelta)
            {
                // Fast-forward seek
                // Simulate without restarting but with fixed steps.
                particleSystem.Simulate(time - particleSystem.time, true, false, true);
            }
            else if (time > particleSystem.time + minDelta)
            {
                // Edit mode playback
                // Simulate without restarting nor fixed step.
                particleSystem.Simulate(time - particleSystem.time, true, false, false);
            }
            else
            {
                // Delta time is too small; Do nothing.
            }
        }
    }

    #endregion
}

}
