using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBehaviour : MonoBehaviour
{
    public int VehicleDensity = 50;
    public float VehicleSafeSpawnDistance = 20f;
    private float vehicle_body_length = -1f;
    public int ZoneDensityPerLane = 5;
    public Transform SpawnZR, SpawnZL, SpawnXR, SpawnXL;
    public GameObject VehiclePrefab, ZonePrefab;

    public Text VehicleCountUI, GlobalRewardUI;
    void Awake()
    {
        vehicle_body_length = VehiclePrefab.transform.GetChild(0).transform.localScale.z;
    }

    void FixedUpdate()
    {
        if (VehicleCountUI != null) VehicleCountUI.text = "Vehicle Count = " + VehicleBehaviour.VehicleCount.ToString();
    }

    private GameObject SpawnVehicle(Transform spawn_point, float initial_velocity, float offset)
    {
        GameObject car = Instantiate(VehiclePrefab, spawn_point);
        car.SetActive(false);
        car.transform.localPosition = Vector3.zero;
        car.transform.localRotation = Quaternion.identity;
        car.transform.position += car.transform.forward * offset;
        VehicleBehaviour vb = car.GetComponent<VehicleBehaviour>();
        vb.HumanDecidedVelocity = initial_velocity;
        vb.MaxZ = (ZoneDensityPerLane * CruiseControllerBehaviour.ZoneLength) + RoadBehaviour.LaneWidth + vehicle_body_length;
        car.SetActive(true);

        return car;
    }

    private CruiseControllerBehaviour SpawnZone(Transform spawn_point, float offset)
    {
        GameObject zone = Instantiate(ZonePrefab, spawn_point);
        zone.transform.localPosition = Vector3.zero;
        zone.transform.localRotation = Quaternion.identity;
        zone.transform.position += zone.transform.forward * offset;

        return zone.GetComponent<CruiseControllerBehaviour>();
    }

    public void SpawnLearnStepVehicles()
    {
        float x_split = Random.Range(0.25f,0.75f);
        float z_split = 1f - x_split;
        int x_split_car_count = Mathf.RoundToInt(x_split * VehicleDensity);
        int z_split_car_count = Mathf.RoundToInt(z_split * VehicleDensity);
        float last_x_spawn_distance = 0f, last_z_spawn_distance = 0f;

        int count = x_split_car_count - z_split_car_count;
        if(count != 0) count = count > 0 ? x_split_car_count : z_split_car_count;
        int x_car_count = 0, z_car_count = 0;

        float base_offset = -RoadBehaviour.LaneWidth - (ZoneDensityPerLane * CruiseControllerBehaviour.ZoneLength) - vehicle_body_length;
        for (int i = 0; i < count; i++)
        {
            if (x_car_count < x_split_car_count)
            {
                last_x_spawn_distance += Random.Range(i == 0 ? 0f : vehicle_body_length * 2f, VehicleSafeSpawnDistance);
                SpawnVehicle(SpawnXR, Random.Range(10f, RoadBehaviour.SpeedLimit), base_offset - last_x_spawn_distance);
                x_car_count++;
            }

            if (z_car_count < z_split_car_count)
            {
                last_z_spawn_distance += Random.Range(i == 0 ? 0f : vehicle_body_length * 2f, VehicleSafeSpawnDistance);
                SpawnVehicle(SpawnZR, Random.Range(10f, RoadBehaviour.SpeedLimit), base_offset - last_z_spawn_distance);
                z_car_count++;
            }
        }
    }

    public void DestroyVehicles()
    {
        VehicleBehaviour[] all = FindObjectsOfType<VehicleBehaviour>();
        foreach (VehicleBehaviour vehicle in all)
        {
            Destroy(vehicle.gameObject);
        }
    }

    public void SpawnLearnStepZones()
    {
        for (int i = 0; i < ZoneDensityPerLane; i++)
        {
            float f_offset = RoadBehaviour.LaneWidth + i * CruiseControllerBehaviour.ZoneLength;
            float r_offset = -(f_offset + CruiseControllerBehaviour.ZoneLength);
            CruiseControllerBehaviour zrf = SpawnZone(SpawnZR, f_offset);
            CruiseControllerBehaviour zrr = SpawnZone(SpawnZR, r_offset);
            //CruiseControllerBehaviour zlf = SpawnZone(ZoneSpawnZL, f_offset);
            //CruiseControllerBehaviour zlr = SpawnZone(ZoneSpawnZL, r_offset);
            CruiseControllerBehaviour xrf = SpawnZone(SpawnXR, f_offset);
            CruiseControllerBehaviour xrr = SpawnZone(SpawnXR, r_offset);
            //CruiseControllerBehaviour xlf = SpawnZone(ZoneSpawnXL, f_offset);
            //CruiseControllerBehaviour xlr = SpawnZone(ZoneSpawnXL, r_offset);

            zrf.Lane = RoadBehaviour.LaneType.ZRLane;
            zrf.GlobalRewardUI = GlobalRewardUI;
            zrr.Lane = RoadBehaviour.LaneType.ZRLane;
            zrr.GlobalRewardUI = GlobalRewardUI;
            //zlf.Lane = RoadBehaviour.LaneType.ZLLane;
            //zlf.GlobalRewardUI = GlobalRewardUI;
            //zlr.Lane = RoadBehaviour.LaneType.ZLLane;
            //zlr.GlobalRewardUI = GlobalRewardUI;
            xrf.Lane = RoadBehaviour.LaneType.XRLane;
            xrf.GlobalRewardUI = GlobalRewardUI;
            xrr.Lane = RoadBehaviour.LaneType.XRLane;
            xrr.GlobalRewardUI = GlobalRewardUI;
            //xlf.Lane = RoadBehaviour.LaneType.XLLane;
            //xlf.GlobalRewardUI = GlobalRewardUI;
            //xlr.Lane = RoadBehaviour.LaneType.XLLane;
            //xlr.GlobalRewardUI = GlobalRewardUI;
        }
    }
}
