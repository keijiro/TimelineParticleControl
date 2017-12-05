using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class ParticleSystemControlMixer : PlayableBehaviour
{
    public bool checkDeterminism = true;

    #region Private variables and methods

    ParticleSystem[] _targetCache;
    bool _warned;

    float GetEffectTime(Playable playable)
    {
        var inputCount = playable.GetInputCount();
        for (var i = 0; i < inputCount; i++)
            if (playable.GetInputWeight(i) > 0)
                return (float)playable.GetInput(i).GetTime();
        return -1;
    }

    void CheckDeterminism(ParticleSystem ps)
    {
        if (ps.useAutoRandomSeed)
            Debug.LogWarning(
                "Auto random seed is enabled in " +
                "'" + GetTransformFullPath(ps.transform) + "'. " +
                "Turn it off to get deterministic behavior in the timeline.");
    }

    string GetTransformFullPath(Transform tr)
    {
        var path = tr.name;

        while (tr.parent != null)
        {
            tr = tr.parent;
            path = tr.name + "/" + path;
        }

        return path;
    }

    #endregion

    #region PlayableBehaviour overrides

    public override void OnGraphStart(Playable playable)
    {
        _targetCache = null;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // Retrieve the target root transform from the track-given data.
        var root = playerData as Transform;
        if (root == null) return;

        // Scan the track to determine the current effect time.
        var time = GetEffectTime(playable);
        if (time < 0) return;

        // Update the cache that contains the target renderer list.
        if (_targetCache == null || _targetCache.Length == 0)
            _targetCache = root.GetComponentsInChildren<ParticleSystem>();

        // Do nothing if there is no particle system under the target.
        if (_targetCache.Length == 0) return;

        // Is it in edit mode?
        var editMode = !Application.isPlaying;

        // Auto-calculate the target frame rate.
        var sdt = Time.smoothDeltaTime;
        if (editMode || sdt < 1.0f / 200) sdt = 1.0f / 60;
        var minDeltaTime = sdt / 2;
        var maxDeltaTime = sdt * 6;

        foreach (var ps in _targetCache)
        {
            if (time < ps.time - minDeltaTime)
            {
                // Backward seek: Use the seek-with-restart method.
                ps.Simulate(time);

                // In play mode, we have to call Play after Simulate.
                if (!editMode) ps.Play();
            }
            else if (time - ps.time > maxDeltaTime)
            {
                // Fast-forward seek happened.
                // Simulate without restarting but with fixed step.
                ps.Simulate(time - ps.time, true, false, true);
                if (!editMode) ps.Play();
            }
            else if (editMode && time - ps.time >= minDeltaTime)
            {
                // Edit mode playback.
                // Simulate without restarting nor fixed step.
                ps.Simulate(time - ps.time, true, false, false);
            }

            if (!_warned && checkDeterminism) CheckDeterminism(ps);
        }

        // Suppress further warning.
        _warned = true;
    }

    #endregion
}
