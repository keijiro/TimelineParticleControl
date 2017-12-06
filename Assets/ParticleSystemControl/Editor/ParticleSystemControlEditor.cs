using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[CustomEditor(typeof(ParticleSystemControl)), CanEditMultipleObjects]
class ParticleSystemControlEditor : Editor
{
    SerializedProperty _rateOverTime;
    SerializedProperty _rateOverDistance;

    static class Styles 
    {
        public static readonly GUIContent time = new GUIContent("Over Time");
        public static readonly GUIContent distance = new GUIContent("Over Distance");
    }

    void OnEnable()
    {
        _rateOverTime = serializedObject.FindProperty("template.rateOverTime");
        _rateOverDistance = serializedObject.FindProperty("template.rateOverDistance");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Particle Emission Rates");
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(_rateOverTime, Styles.time);
        EditorGUILayout.PropertyField(_rateOverDistance, Styles.distance);
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
