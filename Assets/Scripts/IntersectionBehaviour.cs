using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionBehaviour : MonoBehaviour
{
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag.Equals("traffic"))
        {
            SpawnBehaviour.VehicleIntersectionPassedCount++;
        }
    }
}
