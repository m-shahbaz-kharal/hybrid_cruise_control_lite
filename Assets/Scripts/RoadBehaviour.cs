using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBehaviour : MonoBehaviour
{
    /* General Note
     * Vehicles are assumed to follow speed limit.
    */
    public static float LaneLength = 1000f, LaneWidth = 3.6576f;
    public static float SpeedLimit = 60f;

    public enum LaneType
    {
        None, XLLane, XRLane, ZLLane, ZRLane
    }

    public static bool[] GetLaneID(LaneType lane)
    {
        bool[] result;
        switch (lane)
        {
            case LaneType.XLLane:
                result = new bool[] { true, false, false, false };
                break;
            case LaneType.XRLane:
                result = new bool[] { false, true, false, false };
                break;
            case LaneType.ZLLane:
                result = new bool[] { false, false, true, false };
                break;
            case LaneType.ZRLane:
                result = new bool[] { false, false, false, true };
                break;
            default:
                result = new bool[] { false, false, false, false };
                break;
        }
        return result;
    }
}
