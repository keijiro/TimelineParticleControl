Timeline Particle System Control Example
========================================

This is an example that shows how to control a particle system from a timeline
track with using a custom track class.

![gif](https://i.imgur.com/rJz6eWk.gif)
![gif](https://i.imgur.com/G6LnNFn.gif)

Controlling Particle System is Hard
-----------------------------------

Controlling a particle system from a timeline is hard without full particle
baking (that's not an available option for real-time use). It needs a
compromise to implement, but capacities to compromise vary from case to case.
That's why it's recommended to implement a custom control track class for each
case.

The standard [Control Track] class that is contained in the Timeline class
library provides basic functionalities to control a particle system within a
timeline track, but it has some drawbacks.

- It only provides on/off switch. An extra Animation Track is needed to control
  particle parameters.
- The Inherit Velocity module doesn't work with the Control Track.
- It overrides the random seed number with the same value even if there are
  multiple particle systems under the given transform.

The custom track class (`ParticleSystemControlTrack`) contained in this example
provides the following functionalities.

- Playhead scrubbing support (not perfect; reset on rewinding).
- Emission rate control. It's controllable from both inline animation curves
  and ease-in/out curves.
- Inherit Velocity module support.
- No random seed override. Only warns when non-deterministic setting is found.

This is just based on my personal preference and far from perfect design, but
enough for improve productivity in my projects.

[Control Track]: https://docs.unity3d.com/ScriptReference/Timeline.ControlTrack.html

License
-------

Copyright (c) 2017 Unity Technologies

This repository is to be treated as an example content of Unity; you can use
the code freely in your projects. Also see the [FAQ] about example contents.

[FAQ]: https://unity3d.com/unity/faq#faq-37863
