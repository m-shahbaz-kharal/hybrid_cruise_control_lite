using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentBehaviour : MonoBehaviour
{
    public enum ExperimentPhaseEnumeration
    {
        Learning, Inference
    }
    public static ExperimentPhaseEnumeration ExperimentPhase = ExperimentPhaseEnumeration.Learning;

    public static bool DebugMode = true;
}
