using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EpisodeManager : MonoBehaviour
{
    public float EpisodeTimeSeconds = 100f;
    private CruiseControllerBehaviour[] cruise_controllers;
    private SpawnBehaviour spawn;

    public Text EpisodeTimeSecondsUI;
    private float tick, tock;

    void Awake()
    {
        spawn = FindObjectOfType<SpawnBehaviour>();
    }

    void Start()
    {
        tick = Time.time;
        spawn.SpawnLearnStepZones();
        spawn.SpawnLearnStepVehicles();
    }

    void FixedUpdate()
    {
        tock = Time.time;
        if(tock - tick >= EpisodeTimeSeconds || VehicleBehaviour.VehicleCount == 0)
        {
            if(cruise_controllers == null) cruise_controllers = FindObjectsOfType<CruiseControllerBehaviour>();
            tick = tock;
            foreach(CruiseControllerBehaviour controller in cruise_controllers)
            {
                controller.EndEpisode();
            }
            CruiseControllerBehaviour.GlobalReward = 0f;
            spawn.DestroyVehicles();
            VehicleBehaviour.VehicleCount = 0;
            spawn.SpawnLearnStepVehicles();
        }

        if (EpisodeTimeSecondsUI != null) EpisodeTimeSecondsUI.text = "Episode Time (Sec) = " + string.Format("{0:0.00}", tock - tick) + " / " + EpisodeTimeSeconds;
    }
}
