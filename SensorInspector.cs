using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sensor))]
public class SensorInspector : Editor
{

    public override void OnInspectorGUI()
    {
        Sensor sensorScript = (Sensor)target;
        sensorScript.ID = EditorGUILayout.IntField("ID", sensorScript.ID);
        EditorGUILayout.LabelField("State", sensorScript.getState().ToString());
    }

}

