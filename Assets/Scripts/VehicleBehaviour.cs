using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleBehaviour : MonoBehaviour
{
    public Transform BodyFront;
    public float SensorRange = 40; // talking in meters so ...
    public float BrakingEfficiency = 10f;
    public float SafeDistance = 30; // 31.29 meters is safe distance for vehicles running at 56.327 km/h.
    public float SpeedingEfficiency = 10f;
    [HideInInspector]
    public float HumanDecidedVelocity = -1f;
    public float RandomVelocityChangeInterval = 5f;
    private float velocity_change_tick = -1f, velocity_change_tock = -1f;
    private Rigidbody body;
    public Text VelocityUI;
    [HideInInspector]
    public CruiseControllerBehaviour CruiseController;

    private float position_delta_wrt_zone = 0f;
    [HideInInspector] public float MaxZ = -1f;
    private Vector3 registered_velocity = Vector3.zero;

    public static int VehicleCount = 0;

    void Awake()
    {
        VehicleCount++;
    }

    void Start()
    {
        body = GetComponent<Rigidbody>();
        velocity_change_tick = Time.time;

        if (MaxZ == -1) Debug.LogError("Vehicle's max Z value not set!");
    }

    void FixedUpdate()
    {
        if (CruiseController != null)
        {
            position_delta_wrt_zone = (BodyFront.position - CruiseController.ZoneStart.position).magnitude / CruiseControllerBehaviour.ZoneLength;
            if (body.velocity.magnitude < (CruiseControllerBehaviour.CruiseValueHisto[CruiseController.ZoneID] * RoadBehaviour.SpeedLimit) - 0.1f || body.velocity.magnitude > (CruiseControllerBehaviour.CruiseValueHisto[CruiseController.ZoneID] * RoadBehaviour.SpeedLimit) + 0.1f)
            {
                body.velocity = registered_velocity + BodyFront.forward * ((CruiseControllerBehaviour.CruiseValueHisto[CruiseController.ZoneID] * RoadBehaviour.SpeedLimit) - registered_velocity.magnitude) * position_delta_wrt_zone;
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(BodyFront.position, BodyFront.forward, out hit, SensorRange, LayerMask.GetMask("traffic")) && hit.distance < SafeDistance)
        {
            Debug.DrawLine(BodyFront.position, hit.point, Color.red, 0.1f);
            body.velocity += BodyFront.forward * (hit.rigidbody.velocity.magnitude - body.velocity.magnitude) * BrakingEfficiency * Time.deltaTime;
        }
        else
        {
            if (CruiseController == null) body.velocity += BodyFront.forward * (HumanDecidedVelocity - body.velocity.magnitude) * SpeedingEfficiency * Time.deltaTime;
        }

        velocity_change_tock = Time.time;
        if(velocity_change_tock - velocity_change_tick >= RandomVelocityChangeInterval)
        {
            velocity_change_tick = velocity_change_tock;
            HumanDecidedVelocity = Random.Range(0.5f, 1f) * RoadBehaviour.SpeedLimit;
        }

        if (VelocityUI != null) VelocityUI.text = string.Format("V = {0:0.00}", body.velocity.magnitude);

        // destroy car when out of range
        if (transform.localPosition.z > MaxZ) Destroy(gameObject);
    }

    void OnDestroy()
    {
        VehicleCount--;
    }

    public void RegisterDeltaVelocity()
    {
        registered_velocity = body.velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("traffic"))
        {
            CruiseControllerBehaviour.GlobalReward -= 10f;
            Destroy(collision.gameObject);
            Destroy(this);
        }
    }
}
