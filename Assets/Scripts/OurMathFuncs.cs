using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurMathFuncs : MonoBehaviour
{
    public static float Sigmoid(float v)
    {
        return v / (1f + Mathf.Abs(v));
    }
}
