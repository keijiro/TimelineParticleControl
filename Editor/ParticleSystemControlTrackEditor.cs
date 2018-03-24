// Timeline Particle Control Example
// https://github.com/keijiro/TimelineParticleControl

using UnityEditor;
using UnityEditor.Timeline;

namespace Klak.Timeline {

// We use a nested editor to enable context reference (it's needed to enable
// exposed references).

[CustomEditor(typeof(ParticleSystemControlTrack)), CanEditMultipleObjects]
class ParticleSystemControlTrackEditor : Editor
{
    Editor _editor;

    void OnEnable()
    {
        // Use the current active director as an editing context.
        _editor = Editor.CreateEditorWithContext(
            targets, TimelineEditor.playableDirector,
            typeof(ParticleSystemControlTrackEditor2)
        );
    }

    void OnDestroy()
    {
        DestroyImmediate(_editor);
    }

    public override void OnInspectorGUI()
    {
        _editor.OnInspectorGUI();
    }
}

class ParticleSystemControlTrackEditor2 : Editor
{
    SerializedProperty _snapTarget;
    SerializedProperty _randomSeed;

    void OnEnable()
    {
        _snapTarget = serializedObject.FindProperty("template.snapTarget");
        _randomSeed = serializedObject.FindProperty("template.randomSeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_snapTarget);
        EditorGUILayout.PropertyField(_randomSeed);
        serializedObject.ApplyModifiedProperties();
    }
}

}
