using UnityEngine;

public static class VectorsExtensions
{
    public static Vector3 WithAxis(this Vector3 vector3, Axis axis, float value)
    {
        return new Vector3(
            x: axis == Axis.X ? value : vector3.x,
            y: axis == Axis.Y ? value : vector3.y,
            z: axis == Axis.Z ? value : vector3.z
            );
    }
}

public enum Axis
{
    X, Y, Z
}