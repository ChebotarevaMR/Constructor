using Newtonsoft.Json;
using UnityEngine;
public class Item
{
    public Vector Position { get; private set; }
    public Vector Rotation { get; private set; }
    public Vector Scale { get; private set; }
    public string SpriteName { get; private set; }
    
    [JsonConstructor]
    public Item(Vector position, Vector rotation, Vector scale, string spriteName)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
        SpriteName = spriteName;
    }

    public Item(Vector3 position, Quaternion rotation, Vector3 scale, string spriteName)
    {
        Position = new Vector(position.x, position.y, position.z);
        var eulerAngles = rotation.eulerAngles;
        Rotation = new Vector(eulerAngles.x, eulerAngles.y, eulerAngles.z);
        Scale = new Vector(scale.x, scale.y, scale.z);
        SpriteName = spriteName;
    }
}

public class Vector
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Z { get; private set; }

    [JsonConstructor]
    public Vector(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}
