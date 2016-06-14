using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ProfileManager))]
public class ProfileManagerEditor : Editor {
    ProfileManager m_profileManager;

    void OnEnable()
    {
        m_profileManager = (ProfileManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        if (GUILayout.Button("Save Default"))
        {
            m_profileManager.SaveDefaultProfile();
        }
        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        if (GUILayout.Button("Reset"))
        {
            m_profileManager.ResetProfile();
        }
    }
}
