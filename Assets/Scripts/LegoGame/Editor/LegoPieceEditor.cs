using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LegoPiece))]
public class LegoPieceEditor : Editor
{
    protected SerializedProperty sizeProp;
    protected SerializedProperty pivotProp;
    protected SerializedProperty tilesProp;
    protected SerializedProperty colorProp;
    protected SerializedProperty baseSpriteProp;
    protected SerializedProperty defaultSpriteOrderProp;
    protected SerializedProperty canMoveProp;

    protected const int tileSize = 20; // Size of each grid cell in the inspector
    protected const int padding = 2;   // Padding between grid cells
    protected const int outlineWidth = 1;   // Padding between grid cells

    protected void OnEnable()
    {
        // Link the serialized properties with the fields
        sizeProp = serializedObject.FindProperty("size");
        pivotProp = serializedObject.FindProperty("pivot");
        tilesProp = serializedObject.FindProperty("tiles");
        colorProp = serializedObject.FindProperty("color");
        baseSpriteProp = serializedObject.FindProperty("baseSprite");
        defaultSpriteOrderProp = serializedObject.FindProperty("defaultSpriteOrder");
        canMoveProp = serializedObject.FindProperty("canMove");
    }

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
                Undo.RecordObject(legoPiece, "Rebuild LegoPiece");

                legoPiece.Rebuild();
                EditorUtility.SetDirty(legoPiece);
            }
        }
    }

    protected void DrawGrid()
    {
        // Get the size and pivot from the SerializedProperties
        Vector2Int size = sizeProp.vector2IntValue;
        Vector2Int pivot = pivotProp.vector2IntValue;

        // Ensure the tiles array is consistent with the size
        if (size.x * size.y != tilesProp.arraySize)
        {
            tilesProp.arraySize = size.x * size.y;
        }

        GUILayout.Space(10);

        // Calculate the total width of the grid
        float gridWidth = size.x * (tileSize + padding);

        // Get the width of the inspector window
        float inspectorWidth = EditorGUIUtility.currentViewWidth;

        // Calculate how much horizontal padding is needed to center the grid
        float horizontalPadding = Mathf.Max((inspectorWidth - gridWidth) / 2, 0);

        // Create a grid layout in the custom editor
        for (int y = 0; y < size.y; y++)
        {
            GUILayout.BeginHorizontal();

            // Add left padding to center the grid row
            GUILayout.Space(horizontalPadding);

            for (int x = 0; x < size.x; x++)
            {
                int index = x + y * size.x;
                SerializedProperty tileProp = tilesProp.GetArrayElementAtIndex(index);
                bool isPivot = (pivot.x == x && pivot.y == y);

                // Draw each grid cell
                DrawGridCell(x, y, tileProp, isPivot);
            }

            GUILayout.EndHorizontal();
        }
    }

    protected void DrawGridCell(int x, int y, SerializedProperty tileProp, bool isPivot)
    {
        Event currentEvent = Event.current;
        Rect rect = GUILayoutUtility.GetRect(tileSize, tileSize, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

        // Handle left-click for toggling tile state and right-click for setting pivot
        if (rect.Contains(currentEvent.mousePosition) && currentEvent.type == EventType.MouseDown)
        {
            if (currentEvent.button == 0) // Left-click: toggle tile
            {
                tileProp.boolValue = !tileProp.boolValue;
                currentEvent.Use();
                serializedObject.ApplyModifiedProperties();
            }
            else if (currentEvent.button == 1) // Right-click: set pivot
            {
                pivotProp.vector2IntValue = new Vector2Int(x, y);
                currentEvent.Use();
                serializedObject.ApplyModifiedProperties();
            }
        }

        // If it's a pivot, color it differently
        Color tileColor = Color.green;
        if (tileProp.boolValue)
        {
            tileColor = new Color(0.1f, 0.6f, 0.1f, 1.0f);
        }
        else
        {
            tileColor = GetInspectorBackgroundColor();
        }

        // Draw the cell outline
        EditorGUI.DrawRect(rect, Color.black);

        // Draw the cell inside
        Rect insideRect = rect;
        insideRect.x += outlineWidth; insideRect.y += outlineWidth;
        insideRect.width -= outlineWidth * 2; insideRect.height -= outlineWidth * 2;
        EditorGUI.DrawRect(insideRect, tileColor);

        // Draw the pivot
        if (isPivot)
        {
            rect.x += rect.width * 0.4f;
            rect.y += rect.height* 0.4f;
            rect.width *= 0.2f;
            rect.height *= 0.2f;

            EditorGUI.DrawRect(rect, Color.yellow);
        }
    }

    protected Color GetInspectorBackgroundColor()
    {
        if (EditorGUIUtility.isProSkin)
        {
            // Dark theme: use a dark background color
            return new Color(0.219f, 0.219f, 0.219f); // Typical dark background color
        }
        else
        {
            // Light theme: use a light background color
            return new Color(0.761f, 0.761f, 0.761f); // Typical light background color
        }
    }
}
