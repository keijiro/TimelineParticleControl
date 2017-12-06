using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class ParticleSystemControlMixer : PlayableBehaviour
{
    #region Editable properties

    public bool checkDeterminism = true;

    #endregion

    #region Private variables and methods

    bool _needRestart;
    bool _warned;

    static string GetTransformFullPath(Transform tr)
    {
        var path = tr.name;

        while (tr.parent != null)
        {
            tr = tr.parent;
            path = tr.name + "/" + path;
        }

        return path;
    }

    void CheckDeterminism(ParticleSystem ps)
    {
        if (!_warned && ps.useAutoRandomSeed)
        {
            Debug.LogWarning(
                "Auto random seed is enabled in " +
                "'" + GetTransformFullPath(ps.transform) + "'. " +
                "Turn it off to get deterministic behavior in the timeline.");
            _warned = true;
        }
    }

    static void ResetParticleSystem(ParticleSystem ps, float time)
    {
        const float maxSimTime = 2.0f / 3;

        if (time < maxSimTime)
        {
            // The target time is small enough: Use the default simulation
            // function (restart and simulate for the given time).
            ps.Simulate(time);
        }
        else
        {
            // The target time is larger than the threshold: The default
            // simulation can be heavy in this case, so use fast-forward
            // (simulation with just a single step) then simulate for a small
            // period of time.
            ps.Simulate(time - maxSimTime, true, true, false);
            ps.Simulate(maxSimTime, true, false, true);
        }
    }

    #endregion

    #region PlayableBehaviour overrides

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var ps = playerData as ParticleSystem;
        if (ps == null) return;

        // Validate the settings.
        CheckDeterminism(ps);

        // Do nothing if the game object is not active. Will do full restart
        // when activated next time.
        if (!ps.gameObject.activeInHierarchy)
        {
            _needRestart = true;
            return;
        }

        // We use the root node time because the track playable/mixer playable
        // only gives time = 0.
        var time = (float)playable.GetGraph().GetRootPlayable(0).GetTime();

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

        var em = ps.emission;
        em.rateOverTimeMultiplier = totalOverTime;
        em.rateOverDistanceMultiplier = totalOverDist;

        // Time control

        if (Application.isPlaying)
        {
            // Play mode time control: Only resets the simulation when a large
            // gap between the particle time and the playhead was found.
            var maxDelta = Mathf.Max(1.0f / 30, Time.smoothDeltaTime * 2);
            if (Mathf.Abs(time - ps.time) > maxDelta)
            {
                ResetParticleSystem(ps, time);
                ps.Play();
            }
        }
        else
        {
            // Do full restart on activation.
            if (_needRestart)
            {
                ps.Play();
                _needRestart = false;
            }

            // Edit mode time control
            var minDelta = 1.0f / 240;
            var smallDelta = Mathf.Max(0.1f, Time.fixedDeltaTime * 2);
            var largeDelta = 0.2f;

            if (time < ps.time || time > ps.time + largeDelta)
            {
                // Backward seek or big leap
                // Reset the simulation with the current playhead position.
                ResetParticleSystem(ps, time);
            }
            else if (time > ps.time + smallDelta)
            {
                // Fast-forward seek
                // Simulate without restarting but with fixed steps.
                ps.Simulate(time - ps.time, true, false, true);
            }
            else if (time > ps.time + minDelta)
            {
                // Edit mode playback
                // Simulate without restarting nor fixed step.
                ps.Simulate(time - ps.time, true, false, false);
            }
        }
    }

    #endregion
}
