using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LegoPlayfield))]
public class LegoPlayfieldEditor : LegoPieceEditor
{
    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // Draw the default inspector fields
        EditorGUILayout.PropertyField(canMoveProp);
        EditorGUILayout.PropertyField(sizeProp);
        EditorGUILayout.PropertyField(pivotProp);
        EditorGUILayout.PropertyField(colorProp);
        EditorGUILayout.PropertyField(baseSpriteProp);
        EditorGUILayout.PropertyField(defaultSpriteOrderProp);

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();

        DrawGrid();

        serializedObject.ApplyModifiedProperties();

        // Add a button to the inspector
        if (GUILayout.Button("Update"))
        {
            // Code to execute when the button is clicked
            LegoPiece legoPiece = target as LegoPiece;
            if (legoPiece != null)
            {
                Undo.RecordObject(legoPiece, "Rebuild Playfield");

                legoPiece.Rebuild();
                EditorUtility.SetDirty(legoPiece);
            }
        }
    }
}
