Timeline Particle System Control Example
========================================

This is an example that shows how to control particle systems from a custom 
track class.

![gif](https://i.imgur.com/rJz6eWk.gif)
![gif](https://i.imgur.com/G6LnNFn.gif)

Comparison with Control Track
-----------------------------

The standard [Control Track] provides basic functionalities to control a
particle system within a timeline track. Although it's enough for most cases,
it has some small limitations.

- It only provides on/off switch. An extra Animation Track is needed to control
  particle parameters.
- The [Inherit Velocity] module doesn't work with the Control Track.
- It overrides the random seed number with the same value even if there are
  multiple particle systems under the hierarchy.
- It uses the [fixed delta time] as a simulation interval. This is not ideal to
  get smooth animation.

The custom track class (`ParticleSystemControlTrack`) contained in this example
provides the following functionalities.

- Playhead scrubbing support (not perfect; reset on rewinding).
- Emission rate control. It's controllable from both inline animation curves
  and ease-in/out curves.
- Inherit Velocity module support.
- Transform snapping support.
- Random number override just happens with a single particle system.
- Animation with system delta time.

[Control Track]: https://docs.unity3d.com/Packages/com.unity.timeline@1.7/api/UnityEngine.Timeline.ControlTrack.html
[Inherit Velocity]: https://docs.unity3d.com/Manual/PartSysInheritVelocity.html
[fixed delta time]: https://docs.unity3d.com/ScriptReference/Time-fixedDeltaTime.html

License
-------

Copyright (c) 2017 Unity Technologies

This repository is to be treated as an example content of Unity; you can use
the code freely in your projects. Also see the [FAQ] about example contents.

[FAQ]: https://unity3d.com/unity/faq#faq-37863
