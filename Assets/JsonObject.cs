using System;

[Serializable]
public class JsonObject
{
    public string Name;
    public string UniqueID;
    public int Type;
    public float[] Position = new float[3];
    public float[] Rotation = new float[3];
    public float[] Scale = new float[3];
    public bool PhysicsEnabled;
}