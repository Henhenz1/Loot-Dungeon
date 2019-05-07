using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public class Utility
    {
        public static bool near(Vector2 p1, Vector2 p2, float tolerance)
        {
            return (p1 - p2).magnitude < tolerance;
        }

        public static Vector2 findOrientation(Vector2 loc, Vector2 target)
        {
            Vector2 dir = target - loc;
            float angle = Vector2.SignedAngle(Vector2.right, dir);
            if (-180 < angle && angle <= -135)
            {
                //Debug.Log("Orientaion is left");
                return Vector2.left;
            } else if (-135 < angle && angle <= -45)
            {
                //Debug.Log("Orientaion is down");
                return Vector2.down;
            } else if (-45 < angle && angle <= 45)
            {
                //Debug.Log("Orientaion is right");
                return Vector2.right;
            } else if (45 < angle && angle <= 135)
            {
                //Debug.Log("Orientaion is up");
                return Vector2.up;
            } else // if (135 < angle && angle < 180)
            {
                //Debug.Log("Orientaion is left");
                return Vector2.left;
            }
        }

        public static void instantiateInRandomSpace(Vector2 from, Vector2 to, GameObject what)
        {
            throw new System.NotImplementedException();
        }

        public static List<Vector2> FourFace()
        {
            List<Vector2> result = new List<Vector2>();

            result.Add(Vector2.right);
            result.Add(Vector2.up);
            result.Add(Vector2.left);
            result.Add(Vector2.down);

            return result;
        }
    }
}
