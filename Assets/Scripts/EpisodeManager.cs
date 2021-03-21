using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EpisodeManager : MonoBehaviour
{
    public SpawnBehaviour Spawner;
    private CruiseControllerBehaviour[] cruise_controllers;

    public float EpisodeTimeSeconds = 30f;

    public Text EpisodeTimeSecondsUI;
    private float tick, tock;

    void Start()
    {
        tick = Time.time;
        Spawner.SpawnLearnStepZones();
        StartCoroutine(Spawner.SpawnLearnStepVehicles());
    }

    void FixedUpdate()
    {
        tock = Time.time;
        if(tock - tick >= EpisodeTimeSeconds)
        {
            tick = tock;
            
            if (cruise_controllers == null) cruise_controllers = FindObjectsOfType<CruiseControllerBehaviour>();
            foreach (CruiseControllerBehaviour controller in cruise_controllers) controller.EndEpisode();
            
            Spawner.DestroyVehicles();
            SpawnBehaviour.VehicleUnDestroyedCount = 0;
            SpawnBehaviour.VehicleDestroyedCount = 0;
            SpawnBehaviour.VehicleIntersectionPassedCount = 0;
            StartCoroutine(Spawner.SpawnLearnStepVehicles());
        }

        if (EpisodeTimeSecondsUI != null) EpisodeTimeSecondsUI.text = "Episode Time (Sec) = " + string.Format("{0:0.00}", tock - tick) + " / " + EpisodeTimeSeconds;
    }
}
