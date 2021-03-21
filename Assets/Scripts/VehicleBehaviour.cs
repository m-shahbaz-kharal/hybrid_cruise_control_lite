using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleBehaviour : MonoBehaviour
{
    private Rigidbody body;
    public Transform BodyFront;
    public float SensorRange = 40; // talking in meters so ...
    public float SafeDistance = 30; // 31.29 meters is safe distance for vehicles running at 56.327 km/h.
    [HideInInspector]
    public float HumanDecidedVelocity = -1f;
    public float RandomVelocityChangeInterval = 5f;
    private float velocity_change_tick = -1f, velocity_change_tock = -1f;
    public Text VelocityUI;

    [HideInInspector]
    public CruiseControllerBehaviour CruiseController;

    [HideInInspector] public float DrivableRoadLength = -1f;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        velocity_change_tick = Time.time;

        SpawnBehaviour.VehicleUnDestroyedCount++;

        if (DrivableRoadLength == -1) Debug.LogError("DrivableRoadLength value not set!");
    }

    void FixedUpdate()
    {

        RaycastHit hit;
        if (Physics.Raycast(BodyFront.position, BodyFront.forward, out hit, SensorRange, LayerMask.GetMask("traffic")) && hit.distance < SafeDistance)
        {
            if (ExperimentBehaviour.DebugMode) Debug.DrawLine(BodyFront.position, hit.point, Color.red, 0.1f);
            if(hit.transform.forward.Equals(body.transform.forward)) body.velocity = hit.rigidbody.velocity;
            else if(CruiseController == null) body.velocity = BodyFront.forward * HumanDecidedVelocity;
            else body.velocity = BodyFront.forward * CruiseControllerBehaviour.CruiseValueHisto[CruiseController.ZoneID] * RoadBehaviour.SpeedLimit;
        }
        else
        {
            if (CruiseController == null) body.velocity = BodyFront.forward * HumanDecidedVelocity;
            else body.velocity = BodyFront.forward * CruiseControllerBehaviour.CruiseValueHisto[CruiseController.ZoneID] * RoadBehaviour.SpeedLimit;
        }

        velocity_change_tock = Time.time;
        if(velocity_change_tock - velocity_change_tick >= RandomVelocityChangeInterval)
        {
            velocity_change_tick = velocity_change_tock;
            HumanDecidedVelocity = Random.Range(0.5f, 1f) * RoadBehaviour.SpeedLimit;
        }

        if (VelocityUI != null) VelocityUI.text = string.Format("V = {0:0.00}", body.velocity.magnitude);

        // destroy car when out of range
        // if (transform.localPosition.z > DrivableRoadLength) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("traffic"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            SpawnBehaviour.VehicleUnDestroyedCount--;
            SpawnBehaviour.VehicleDestroyedCount++;
        }
    }
}
