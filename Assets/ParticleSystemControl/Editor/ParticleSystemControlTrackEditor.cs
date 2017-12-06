// Timeline Particle Control Example
// https://github.com/keijiro/TimelineParticleControl

using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomEditor(typeof(ParticleSystemControlTrack)), CanEditMultipleObjects]
class ParticleSystemControlTrackEditor : Editor
{
    SerializedProperty _checkDeterminism;

    void OnEnable()
    {
        _checkDeterminism = serializedObject.FindProperty("template.checkDeterminism");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_checkDeterminism);

        serializedObject.ApplyModifiedProperties();
    }
}
