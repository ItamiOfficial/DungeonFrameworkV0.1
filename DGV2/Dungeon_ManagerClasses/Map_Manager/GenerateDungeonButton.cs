
using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[CustomEditor(typeof(Dungeon_MapManager))]
[CanEditMultipleObjects]
class ManagerEditor : Editor
{


    private void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
        
        if (GUILayout.Button("Generate Map"))
        {
            Debug.Log("Map is getting Generated");
            Dungeon_Manager manager = Dungeon_Manager.Instance;

            Stopwatch st = new Stopwatch();
            st.Start();
            manager.GenerateNextFloor();
            st.Stop();
            Debug.Log(string.Format("Map Generation took {0} ms to complete", st.ElapsedMilliseconds));
        }
    }
}
