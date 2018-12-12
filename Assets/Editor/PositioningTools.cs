using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class PositioningTools : EditorWindow
{
    [MenuItem("Editor Tools/Positioning Tool")]
    public static void ShowWindow()
    {
        GetWindow<PositioningTools>("Position Tool");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Center Elements"))
        {
            CenterElements();
        }
    }

    void CenterElements()
    {
        if (Selection.activeGameObject == null || Selection.activeGameObject.tag != "Room")
        {
            Debug.Log("Please select a room first");
            return;
        }

        List<Transform> children = Selection.activeGameObject.transform.GetComponentsInChildren<Transform>().ToList();
        Transform floor = children[1];

        children.RemoveRange(0, 2);

        float centerOfChildren = 0;

        foreach(Transform child in children)
        {
            centerOfChildren += child.localPosition.x;
        }

        centerOfChildren /= children.Count;

        float diff = floor.transform.localPosition.x - centerOfChildren;

        foreach(Transform child in children)
        {
            child.localPosition = new Vector3(child.localPosition.x + diff, 
                child.localPosition.y, child.localPosition.z);
        }
    }
}
