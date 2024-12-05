using UnityEngine;
using UnityEditor;
using System.IO;

// deprecated because unreal json blueprint utilities sucks
public class JSONLevelExporter : EditorWindow
{
    string outputFilePath = "";

    [MenuItem("Tools/JSON Level Exporter")]
    public static void ShowWindow()
    {
        GetWindow<JSONLevelExporter>("JSON Level Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON Level Exporter", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Output File"))
        {
            outputFilePath = EditorUtility.SaveFilePanel("Save JSON File", Application.dataPath, "LevelData", "json");
        }

        GUILayout.Label("output File: " + (string.IsNullOrEmpty(outputFilePath) ? "none" : outputFilePath), EditorStyles.wordWrappedLabel);

        if (!string.IsNullOrEmpty(outputFilePath) && GUILayout.Button("Export JSON"))
        {
            exportJson();
        }
    }

    void exportJson()
    {
        if (string.IsNullOrEmpty(outputFilePath))
        {
            Debug.LogError("how 2");
            return;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        JsonLevel jsonLevel = new JsonLevel();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Cube") || obj.CompareTag("Plane") || obj.CompareTag("Sphere") || obj.CompareTag("Capsule"))
            {
                JsonObject jsonObject = new JsonObject
                {
                    Name = obj.name,
                    UniqueID = obj.GetInstanceID().ToString(),
                    Type = getType(obj.tag),
                    Position = new float[] { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z },
                    Rotation = new float[] {
                        Mathf.Deg2Rad * obj.transform.eulerAngles.x,
                        Mathf.Deg2Rad * obj.transform.eulerAngles.y,
                        Mathf.Deg2Rad * obj.transform.eulerAngles.z
                    },
                    Scale = new float[] { obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z },
                    PhysicsEnabled = obj.GetComponent<Rigidbody>() != null
                };
                jsonLevel.Objects.Add(jsonObject);
            }
        }

        string json = JsonUtility.ToJson(jsonLevel, true);
        File.WriteAllText(outputFilePath, json);
        Debug.Log($"exported to {outputFilePath}");
    }

    int getType(string tag)
    {
        int type = 0;
        switch(tag){
            case "Cube":
                type = 1;
                break;
            case "Plane":
                type = 2;
                break;
            case "Sphere":
                type = 3;
                break;
            case "Capsule":
                type = 4;
                break;
            default:
                break;
        }
        return type;
    }
}