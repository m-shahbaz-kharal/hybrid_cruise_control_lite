using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.UI;

public class CruiseControllerBehaviour : Agent
{
    public IntersectionBehaviour Intersection;
    public Transform ZoneStart, ZoneEnd;
    public Text CruiseValueUI;
    public static float ZoneLength = 10f;

    [Header("RL Parameters")]
    [HideInInspector]
    public int ZoneID = -1; // only for dictionary purposes
    private static int ZoneIDIndexer = -1;
    private float intersection_distance, intersection_length;
    [SerializeField]
    public RoadBehaviour.LaneType Lane = RoadBehaviour.LaneType.None;
    private bool[] lane_hot_id = null;
    public static SortedDictionary<int, int> VehicleCountHisto = new SortedDictionary<int, int>();
    public static SortedDictionary<int, float> CruiseValueHisto = new SortedDictionary<int, float>();

    public override void Initialize()
    {
        ZoneID = ++ZoneIDIndexer;

        Vector3 intersection_distance_vector = (transform.position - Intersection.transform.position);
        intersection_distance = intersection_distance_vector.x;
        if (intersection_distance_vector.y > intersection_distance) intersection_distance = intersection_distance_vector.y;
        if (intersection_distance_vector.z > intersection_distance) intersection_distance = intersection_distance_vector.z;
        intersection_length = Intersection.transform.localScale.z;
        lane_hot_id = RoadBehaviour.GetLaneID(Lane);
        VehicleCountHisto.Add(ZoneID, 0);
        CruiseValueHisto.Add(ZoneID, -1f);
    }

    public override void OnEpisodeBegin()
    {
        VehicleCountHisto[ZoneID] = 0;
        CruiseValueHisto[ZoneID] = -1f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // distance from intersection and intersection length
        sensor.AddObservation(intersection_distance);
        sensor.AddObservation(intersection_length);

        // lane hot id
        sensor.AddObservation(lane_hot_id[0]);
        sensor.AddObservation(lane_hot_id[1]);
        sensor.AddObservation(lane_hot_id[2]);
        sensor.AddObservation(lane_hot_id[3]);        

        // histogram of vehicle counts
        foreach(int value in VehicleCountHisto.Values) sensor.AddObservation(value);

        // histogram of cruise values
        foreach (float value in CruiseValueHisto.Values) sensor.AddObservation(value);
    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        CruiseValueHisto[ZoneID] = Mathf.Clamp(actions.ContinuousActions[0], 0f, 1f);

        // saftey reward
        AddReward(SpawnBehaviour.VehicleUnDestroyedCount/SpawnBehaviour.VehicleDensity);

        // flow-rate reward
        AddReward(SpawnBehaviour.VehicleIntersectionPassedCount/SpawnBehaviour.VehicleDensity);
    }



    public override void Heuristic(in ActionBuffers actions)
    {
        var continuous_actions = actions.ContinuousActions;
        continuous_actions[0] = Random.value;
    }

    void FixedUpdate()
    {
        if (CruiseValueUI != null) CruiseValueUI.text = string.Format("Cruise Value = {0:0.00}", CruiseValueHisto[ZoneID]);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("traffic"))
        {
            VehicleCountHisto[ZoneID]++;
            collider.GetComponent<VehicleBehaviour>().CruiseController = this;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.tag.Equals("traffic") && collider.GetComponent<VehicleBehaviour>().CruiseController == null)
        {
            VehicleCountHisto[ZoneID]++;
            collider.GetComponent<VehicleBehaviour>().CruiseController = this;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag.Equals("traffic"))
        {
            VehicleCountHisto[ZoneID]--;
            if (collider.GetComponent<VehicleBehaviour>().CruiseController.Equals(this)) collider.GetComponent<VehicleBehaviour>().CruiseController = null;
        }
    }
}
