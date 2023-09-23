using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionUtil
{
    public static Side CalculateCollisionSideSphere(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            // Get the collision normal from the sphere's perspective.
            Vector3 sphereCollisionNormal = -collision.contacts[0].normal;

            // Determine which axis (x, y, or z) the collision normal is predominantly aligned with.
            Axis dominantAxis = GetDominantAxis(sphereCollisionNormal);

            // Determine and log the side of the collision based on the dominant axis and its direction.
            Side side = CollisionSide(dominantAxis, sphereCollisionNormal);
            Debug.Log(side.ToString());
            return side;
        }

        return Side.None;
    }


    private enum Axis
    {
        X, Y, Z
    }

    public enum Side
    {
        Top,
        Bottom,
        Left,
        Right,
        Forward,
        Backward,
        None
    }

    private static Axis GetDominantAxis(Vector3 vector)
    {
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y) && Mathf.Abs(vector.x) > Mathf.Abs(vector.z))
            return Axis.X;

        if (Mathf.Abs(vector.y) > Mathf.Abs(vector.x) && Mathf.Abs(vector.y) > Mathf.Abs(vector.z))
            return Axis.Y;

        return Axis.Z;
    }

    private static Side CollisionSide(Axis dominantAxis, Vector3 normal)
    {
        switch (dominantAxis)
        {
            case Axis.X:
                if (normal.x > 0)
                    return Side.Right;
                else
                    return Side.Left;

            case Axis.Y:
                if (normal.y > 0)
                    return Side.Top;
                else
                    return Side.Bottom;

            case Axis.Z:
                if (normal.z > 0)
                    return Side.Forward;
                else
                    return Side.Backward;
            default: 
                return Side.None;
        }
    }
}
