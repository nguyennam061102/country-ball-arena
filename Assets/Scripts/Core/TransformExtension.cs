using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    private static Vector3 tempVector3 = Vector3.zero;
	
    public static void SetEulerAngles(this Transform self, float x, float y, float z)
    {
        tempVector3.Set(x, y, z);
        self.eulerAngles = tempVector3;
    }
	
    public static void SetEulerAnglesZAxis(this Transform self, float z)
    {
        self.SetEulerAngles(self.eulerAngles.x, self.eulerAngles.y, z);
    }

    public static void SetLocalScale(this Transform self, float x, float y, float z)
    {
        tempVector3.Set(x, y, z);
        self.localScale = tempVector3;
    }
}
