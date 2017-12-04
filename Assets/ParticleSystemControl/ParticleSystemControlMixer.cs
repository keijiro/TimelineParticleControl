using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
class ParticleSystemControlMixer : PlayableBehaviour
{
    #region Private state

    ParticleSystem[] _targetCache;
    float _lastUpdateTime;

    #endregion

    const float kDefaultDeltaTime = 1.0f / 60;
    const float kMaxDeltaTime = 1.0f / 15;

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

        foreach (var ps in _targetCache)
        {
            if (time < ps.time || time - ps.time > kMaxDeltaTime)
            {
                ps.time = time;
            }
            else
            {
                if (!Application.isPlaying && time - ps.time >= kDefaultDeltaTime/2)
                {
                    ps.Simulate(time - ps.time, true, false, false);
                }
            }
        }
    }

    #endregion

    float GetEffectTime(Playable playable)
    {
        var inputCount = playable.GetInputCount();
        for (var i = 0; i < inputCount; i++)
            if (playable.GetInputWeight(i) > 0)
                return (float)playable.GetInput(i).GetTime();
        return -1;
    }
}
