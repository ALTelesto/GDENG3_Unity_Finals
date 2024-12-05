using UnityEngine;
using UnityEditor;
using System.IO;

// ditched jsonlevelexport and jsonlevelparser because unreal json blueprint utilities suck and is in need of a third-party asset extension
public class LevelFileExporter : EditorWindow
{
    string outputFilePath = "";

    [MenuItem("Tools/Level File Exporter")]
    public static void ShowWindow()
    {
        GetWindow<LevelFileExporter>("Level File Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Level File Exporter", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Output File"))
        {
            outputFilePath = EditorUtility.SaveFilePanel("Save Level File", Application.dataPath, "LevelData", "level");
        }

        GUILayout.Label("output File: " + (string.IsNullOrEmpty(outputFilePath) ? "none" : outputFilePath), EditorStyles.wordWrappedLabel);

        if (!string.IsNullOrEmpty(outputFilePath) && GUILayout.Button("Export Level"))
        {
            exportLevel();
        }
    }

    void exportLevel()
    {
        if (string.IsNullOrEmpty(outputFilePath))
        {
            Debug.LogError("how 2");
            return;
        }

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("Cube") || obj.CompareTag("Plane") || obj.CompareTag("Sphere") || obj.CompareTag("Capsule"))
                {
                    writer.WriteLine(obj.name);
                    writer.WriteLine(obj.GetInstanceID().ToString());

                    int type = getType(obj.tag);
                    writer.WriteLine($"Type: {type}");

                    Vector3 position = obj.transform.position;
                    writer.WriteLine($"Position: {position.x} {position.y} {position.z}");

                    Vector3 rotation = obj.transform.eulerAngles;
                    writer.WriteLine($"Rotation: {Mathf.Deg2Rad * rotation.x} {Mathf.Deg2Rad * rotation.y} {Mathf.Deg2Rad * rotation.z}");

                    Vector3 scale = obj.transform.localScale;
                    writer.WriteLine($"Scale: {scale.x} {scale.y} {scale.z}");

                    int physicsEnabled = obj.GetComponent<Rigidbody>() != null ? 1 : 0;
                    writer.WriteLine($"Physics Enabled: {physicsEnabled}");
                }
            }
        }
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
