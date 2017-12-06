Timeline Particle System Control Example
========================================

This is an example that shows how to control a particle system from a timeline
with using a custom track class.

![gif](https://i.imgur.com/7DuEknn.gif)

Controlling Particle System is Hard
-----------------------------------

Controlling a particle system from a timeline is hard without full particle
baking (that's not an available option for real-time use). It needs a
compromise to implement, but capacities to compromise vary from case to case.
That's why it's recommended to implement a custom control track class based on
each case.

The standard [Control Track] class that is contained in the Timeline class
library provides basic functionalities to control a particle system, but it has
some drawbacks.

- It only provides on/off switch. An Animation Track is needed to control
  particle parameters.
- It overrides the random seed number with the same value even if there are
  multiple particle systems under the given transform.
- The Inherit Velocity module doesn't work with the Control Track.

The custom track class (`ParticleSystemControlTrack`) contained in this example
provides the following functionalities.

- Playhead scrubbing support (not perfect; reset on rewinding).
- Emission rate control. It's controllable from inline animation curves and
  ease-in/out curves.
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
