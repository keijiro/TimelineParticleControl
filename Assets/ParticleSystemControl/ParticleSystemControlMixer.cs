using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
class ParticleSystemControlMixer : PlayableBehaviour
{
    #region Private variables and methods

    ParticleSystem[] _targetCache;

    const float kMinDeltaTime = 1.0f / 120;
    const float kMaxDeltaTime = 1.0f / 15;

    float GetEffectTime(Playable playable)
    {
        var inputCount = playable.GetInputCount();
        for (var i = 0; i < inputCount; i++)
            if (playable.GetInputWeight(i) > 0)
                return (float)playable.GetInput(i).GetTime();
        return -1;
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
        {
            _targetCache = root.GetComponentsInChildren<ParticleSystem>();
        }

        // Is it in edit mode?
        var editMode = !Application.isPlaying;

        foreach (var ps in _targetCache)
        {
            if (time < ps.time)
            {
                // Backward seek happened.
                if (editMode)
                {
                    // Edit mode: Just modify the effect time.
                    ps.time = time;
                }
                else
                {
                    // Play mode: Reset and fast-forward.
                    ps.Simulate(time);
                    ps.Play();
                }
            }
            else if (time - ps.time > kMaxDeltaTime)
            {
                // Fast-forward seek happened.
                // Simulate without restarting but with fixed step.
                ps.Simulate(time - ps.time, true, false, true);
                if (!editMode) ps.Play();
            }
            else if (editMode && time - ps.time >= kMinDeltaTime)
            {
                // Edit mode playback.
                // Simulate without restarting nor fixed step.
                ps.Simulate(time - ps.time, true, false, false);
            }
        }
    }

    #endregion
}
