// Timeline Particle Control Example
// https://github.com/keijiro/TimelineParticleControl

using UnityEngine;
using UnityEngine.Playables;

namespace Klak.Timeline {

// Playable clip class for particle system control

[System.Serializable]
public class ParticleSystemControlPlayable : PlayableBehaviour
{
    public float rateOverTime = 10;
    public float rateOverDistance = 0;
}

}
