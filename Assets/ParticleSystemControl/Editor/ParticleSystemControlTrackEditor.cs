// Timeline Particle Control Example
// https://github.com/keijiro/TimelineParticleControl

using UnityEditor;
using UnityEditor.Timeline;

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
    SerializedProperty _checkDeterminism;

    void OnEnable()
    {
        _snapTarget = serializedObject.FindProperty("template.snapTarget");
        _checkDeterminism = serializedObject.FindProperty("template.checkDeterminism");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_snapTarget);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_checkDeterminism);
        serializedObject.ApplyModifiedProperties();
    }
}
