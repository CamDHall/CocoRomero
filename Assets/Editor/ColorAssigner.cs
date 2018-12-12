using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ColorAssigner : EditorWindow {
    Color safeColor = Color.white;
    Color moderateColor = Color.white;
    Color highColor = Color.white;

    int highHeight;
    int moderateHeight;
    int safeHeight;

    [MenuItem("Editor Tools/Color Assigner")]
    public static void ShowWindow()
    {
        GetWindow<ColorAssigner>("Color Assigner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign Color", EditorStyles.boldLabel);

        safeColor = EditorGUILayout.ColorField("Safe Color", safeColor);
        moderateColor = EditorGUILayout.ColorField("Moderate Color", moderateColor);
        highColor = EditorGUILayout.ColorField("High Color", highColor);

        safeHeight = EditorGUILayout.IntField("Safe Height", safeHeight);
        moderateHeight = EditorGUILayout.IntField("Moderate Height", moderateHeight);
        highHeight = EditorGUILayout.IntField("High Height", highHeight);

        if (GUILayout.Button("Assign Colors"))
        {
            AssignColors();
        }
    }

    void AssignColors()
    {
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");

        foreach (GameObject room in rooms)
        {
            List<SpriteRenderer> sprites = room.GetComponentsInChildren<SpriteRenderer>().ToList();

            foreach (SpriteRenderer sprite in sprites)
            {
                if (sprite.tag == "Piece") continue;

                float highestPoint = 0;

                if (sprite.tag == "Platform")
                {
                    highestPoint = sprite.bounds.size.y
                        + sprite.transform.parent.localPosition.y;
                }
                else
                {
                    highestPoint = sprite.bounds.size.y
                    + sprite.transform.localPosition.y;
                }
                if (highestPoint > highHeight)
                {
                    sprite.color = highColor;
                }
                else if (highestPoint > safeHeight)
                {
                    sprite.color = moderateColor;
                }
                else
                {
                    sprite.color = safeColor;
                }

                if (sprite.tag == "Platform")
                {
                    Platform platform = sprite.transform.parent.GetComponent<Platform>();
                    platform.safeColor = safeColor;
                    platform.moderateColor = moderateColor;
                    platform.highColor = highColor;
                    platform.safeHeight = safeHeight;
                    platform.moderateHeight = moderateHeight;
                    platform.highHeight = highHeight;
                }
            }
        }
    }
}
