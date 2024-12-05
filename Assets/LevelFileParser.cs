using UnityEngine;
using UnityEditor;
using System.IO;

// ditched jsonlevelexport and jsonlevelparser because unreal json blueprint utilities suck and is in need of a third-party asset extension
public class LevelFileParser : EditorWindow
{
    string levelFilePath = "";

    [MenuItem("Tools/Level File Parser")]
    public static void ShowWindow()
    {
        GetWindow<LevelFileParser>("Level File Parser");
    }

    void OnGUI()
    {
        GUILayout.Label("Level File Parser", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Level File"))
        {
            levelFilePath = EditorUtility.OpenFilePanel("Select Level File", Application.dataPath, "level");
        }

        GUILayout.Label("selected File: " + (string.IsNullOrEmpty(levelFilePath) ? "none" : levelFilePath), EditorStyles.wordWrappedLabel);

        if (!string.IsNullOrEmpty(levelFilePath) && GUILayout.Button("Parse Level"))
        {
            parseLevelFile();
        }
    }

    void parseLevelFile()
    {
        if (string.IsNullOrEmpty(levelFilePath))
        {
            Debug.LogError("bruh");
            return;
        }

        string[] lines = File.ReadAllLines(levelFilePath);
        GameObject currentPrimitive = null;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            if (!trimmedLine.Contains(":"))
            {
                currentPrimitive = null;
                continue;
            }

            string[] parts = trimmedLine.Split(':');
            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key)
            {
                case "Type":
                    int type = int.Parse(value);
                    switch (type)
                    {
                        case 1:
                            currentPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            currentPrimitive.tag = "Cube";
                            break;
                        case 2:
                            currentPrimitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
                            currentPrimitive.tag = "Plane";
                            break;
                        case 3:
                            currentPrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            currentPrimitive.tag = "Sphere";
                            break;
                        case 4:
                            currentPrimitive = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                            currentPrimitive.tag = "Capsule";
                            break;
                        default:
                            Debug.LogWarning("Unknown Type: " + type);
                            break;
                    }
                    break;
                case "Position":
                    if (currentPrimitive != null)
                    {
                        string[] positionValues = value.Split(' ');
                        Vector3 position = new Vector3(
                            float.Parse(positionValues[0]),
                            float.Parse(positionValues[1]),
                            float.Parse(positionValues[2])
                        );
                        currentPrimitive.transform.position = position;
                    }
                    break;
                case "Rotation":
                    if (currentPrimitive != null)
                    {
                        string[] rotationValues = value.Split(' ');
                        Vector3 rotation = new Vector3(
                            Mathf.Rad2Deg * float.Parse(rotationValues[0]),
                            Mathf.Rad2Deg * float.Parse(rotationValues[1]),
                            Mathf.Rad2Deg * float.Parse(rotationValues[2])
                        );
                        currentPrimitive.transform.eulerAngles = rotation;
                    }
                    break;
                case "Scale":
                    if (currentPrimitive != null)
                    {
                        string[] scaleValues = value.Split(' ');
                        Vector3 scale = new Vector3(
                            float.Parse(scaleValues[0]),
                            float.Parse(scaleValues[1]),
                            float.Parse(scaleValues[2])
                        );
                        currentPrimitive.transform.localScale = scale;
                    }
                    break;
                case "Physics Enabled":
                    if (currentPrimitive != null)
                    {
                        bool physicsEnabled = int.Parse(value) != 0;
                        if (physicsEnabled)
                        {
                            currentPrimitive.AddComponent<Rigidbody>();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        Debug.Log("level parsing dones");
    }
}