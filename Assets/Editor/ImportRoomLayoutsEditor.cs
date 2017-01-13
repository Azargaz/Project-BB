using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ImportRoomLayouts))]
class ImportRoomLayoutsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ImportRoomLayouts myScript = (ImportRoomLayouts)target;
        if (GUILayout.Button("Import Rooms"))
        {
            Undo.RecordObject(myScript, "Importing rooms.");
            myScript.ImportRooms();
        }
    }
}
