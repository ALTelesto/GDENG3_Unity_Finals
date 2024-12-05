using UnityEngine;
using UnityEditor;
using System.IO;

// deprecated because unreal json blueprint utilities sucks
public class JSONLevelParser : EditorWindow
{
    string jsonFilePath = "";

    [MenuItem("Tools/JSON Level Parser")]
    public static void ShowWindow()
    {
        GetWindow<JSONLevelParser>("JSON Level Parser");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON Level Parser", EditorStyles.boldLabel);

        if (GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", Application.dataPath, "json");
        }

        GUILayout.Label("selected File: " + (string.IsNullOrEmpty(jsonFilePath) ? "none" : jsonFilePath), EditorStyles.wordWrappedLabel);

        if (!string.IsNullOrEmpty(jsonFilePath) && GUILayout.Button("Parse JSON"))
        {
            parse();
        }
    }

    void parse()
    {
        if (string.IsNullOrEmpty(jsonFilePath))
        {
            Debug.LogError("how");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        JsonLevel levelData = JsonUtility.FromJson<JsonLevel>(jsonContent);

        foreach (JsonObject obj in levelData.Objects)
        {
            GameObject primitive = null;
            switch (obj.Type)
            {
                case 1:
                    primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    primitive.tag = "Cube";
                    break;
                case 2:
                    primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    primitive.tag = "Plane";
                    break;
                case 3:
                    primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    primitive.tag = "Sphere";
                    break;
                case 4:
                    primitive = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                    primitive.tag = "Capsule";
                    break;
                default:
                    Debug.Log("untagged object ignored");
                    continue;
            }

            if (primitive != null)
            {
                primitive.transform.position = new Vector3(obj.Position[0], obj.Position[1], obj.Position[2]);
                primitive.transform.eulerAngles = new Vector3(
                    Mathf.Rad2Deg * obj.Rotation[0],    //bruh radians
                    Mathf.Rad2Deg * obj.Rotation[1],
                    Mathf.Rad2Deg * obj.Rotation[2]
                );
                primitive.transform.localScale = new Vector3(obj.Scale[0], obj.Scale[1], obj.Scale[2]);

                if (obj.PhysicsEnabled)
                {
                    primitive.AddComponent<Rigidbody>();
                }
            }
        }
        Debug.Log("primitives created successfully");
    }
}