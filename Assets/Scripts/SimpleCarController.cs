using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    [Header("Input Objects")]
    private InputManager IM;
    public GameManager GM;
    public List<AxleInfo> axleInfos;
    private Vector3 initialPos;
    private Rigidbody rigidbody;
    private GameObject COM;

    [Header("Frictions")]
    public float frictionSmoothing=0.2f;
    public float[] SideExtremumSlip = new float[4];

    [Header("Engine Power and Torque")]
    public bool roadStatus = false;
    public AnimationCurve engineTorque;
    public bool AutomaticTransmission;
    public int max_gear_rpm = 5000;
    public int min_gear_rpm = 3500;
    public float totalPower;
    private float wheelrpm;
    public float engineRPM;
    public float[] gears;
    public int gearNum = 0;
    public float smoothTime = 0.1f;
    public float KPH;

    [Header("Other Variables")]
    public float[] slip = new float[4];
    public float downforceValue = 50;
    public float radius=6;
    public float brake = 60000f;
    public float handbrake = 5000000;
    public bool fourWheel;



    public void Start()
    {

        initialPos = this.transform.position;
        getObject();
        checkWheelDrive();
        COM = this.transform.Find("CenterOfMass").gameObject;
        this.rigidbody.centerOfMass = COM.transform.localPosition;
    }


    public void FixedUpdate()
    {
        getFriction();
        addDownForce();
        gearShift();
        roadCheck();
        checkWheelDrive();
        calculateEnginePower();
        Movement();
        Debug.Log("CarCONTROL: " + IM.vertical + " " + IM.horizontal);
        // Speed in KPH of the car
        KPH = rigidbody.velocity.magnitude * 3.6f;
    }

    /// <summary>
    /// Manages the overall movement of the agent and 
    /// animates the wheels
    /// </summary>
    public void Movement()
    {
        if (IM.reset)
            ResetCar();

        driftCar();

        // Divide the torque in 4 if fourwheel drive, else in 2
        float motor = (fourWheel) ? totalPower / 4 : totalPower / 2;
        foreach (AxleInfo axleInfo in axleInfos)
        {

            moveVehicle(axleInfo, motor);
            steerVehicle(axleInfo, IM.horizontal);


            if (IM.handbrake && axleInfo.rear)
            {
                axleInfo.rightWheel.brakeTorque = axleInfo.leftWheel.brakeTorque = handbrake;

            }
            else
            {
                axleInfo.rightWheel.brakeTorque = axleInfo.leftWheel.brakeTorque = 0f;

            }

            animateWheels(axleInfo.leftWheel, axleInfo.leftWheelmesh, 180);
            animateWheels(axleInfo.rightWheel, axleInfo.rightWheelmesh);
        }
    }


    /// <summary>
    /// Move the Wheels on the axle with the torque
    /// </summary>
    /// <param name="axleInfo"> The axle containing the two wheels (left, right)</param>
    /// <param name="torque"> The torque being applied to these wheels </param>
    private void moveVehicle(AxleInfo axleInfo, float torque)
    {
        if (axleInfo.motor)
        {
            axleInfo.leftWheel.motorTorque = torque;
            axleInfo.rightWheel.motorTorque = torque;
        }
    }


    /// <summary>
    /// Steer the wheels on the axle with the steering torque
    /// calculated from the ackerman formula for steering
    /// </summary>
    /// <param name="axleInfo"> The axle containing the two wheels (left, right)</param>
    /// <param name="steering"> The torque being applied to turn these wheels</param>
    private void steerVehicle(AxleInfo axleInfo, float steering)
    {
        // ackerman steering formula

        if (axleInfo.steering)
        {
            if (steering > 0)
            {
                axleInfo.leftWheel.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.66f / (radius - (1.647f / 2))) * steering;
                axleInfo.rightWheel.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.66f / (radius + (1.647f / 2))) * steering;
            }
            else if (steering < 0)
            {
                axleInfo.leftWheel.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.66f / (radius - (1.647f / 2))) * steering;
                axleInfo.rightWheel.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.66f / (radius + (1.647f / 2))) * steering;
            }
            else
            {
                axleInfo.leftWheel.steerAngle = 0;
                axleInfo.rightWheel.steerAngle = 0;
            }

        }
    }

    /// <summary>
    /// Increase the side slip during handbrake to allow for drifting
    /// </summary>
    public void driftCar()
    {
        WheelHit hit1, hit2;
        WheelCollider left, right;


        for (int i = 0; i < 2; i++)
        {

            left = axleInfos[i].leftWheel;
            right = axleInfos[i].rightWheel;
            float velocity = this.rigidbody.velocity.magnitude;
            left.GetGroundHit(out hit1); right.GetGroundHit(out hit2);


            WheelFrictionCurve sidewaysFriction = left.forwardFriction;
            sidewaysFriction.asymptoteSlip = (IM.handbrake) ? 1f : Mathf.Lerp(sidewaysFriction.asymptoteSlip, .11f, frictionSmoothing);
            left.sidewaysFriction = sidewaysFriction;


            //-------------------------------------------------------------------------------------------------------



            sidewaysFriction = right.sidewaysFriction;
            sidewaysFriction.asymptoteSlip = (IM.handbrake) ? 1f : Mathf.Lerp(sidewaysFriction.asymptoteSlip, .11f, frictionSmoothing);
            right.sidewaysFriction = sidewaysFriction;


            WheelFrictionCurve sidwaysFriction = left.sidewaysFriction;
            sidwaysFriction.extremumSlip = (IM.handbrake) ? 1.5f : Mathf.Lerp(sidwaysFriction.extremumSlip, .5f, frictionSmoothing);
            left.sidewaysFriction = sidwaysFriction;


            //-------------------------------------------------------------------------------------------------------



            sidwaysFriction = right.sidewaysFriction;
            sidwaysFriction.extremumSlip = (IM.handbrake) ? 1.5f : Mathf.Lerp(sidwaysFriction.extremumSlip, .5f, frictionSmoothing);
            right.sidewaysFriction = sidwaysFriction;

        }


    }

    /// <summary>
    /// Adds downforce to the car proportional to the velocity
    /// </summary>
    private void addDownForce()
    {

        rigidbody.AddForce(-transform.up * downforceValue * rigidbody.velocity.magnitude);

    }

    /// <summary>
    /// Checks if the vehicle is 4 wheel drive or 
    /// 2 wheel drive 
    /// </summary>
    private void checkWheelDrive()
    {
        fourWheel = axleInfos[0].motor && axleInfos[1].motor;   
    }


    /// <summary>
    /// Calculate the power to be output based on the gear, RPM and the 
    /// RPM - Power curve
    /// </summary>
    private void calculateEnginePower()
    {
        wheelRPM();
        totalPower = engineTorque.Evaluate(engineRPM) * gears[gearNum] * IM.vertical;

        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelrpm) * 3.6f * (gears[gearNum])), ref velocity, smoothTime);
    }


    /// <summary>
    /// Animates the Wheels to rotate along with the wheel colliders
    /// </summary>
    /// <param name="wc"></param>
    /// <param name="wm"></param>
    /// <param name="offset"></param>
    void animateWheels(WheelCollider wc, GameObject wm, float offset=0)
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        
        wc.GetWorldPose(out wheelPosition, out wheelRotation);
        wm.transform.position = wheelPosition;
        wm.transform.rotation = wheelRotation;
        
    }

    /// <summary>
    /// Shift the gear either manually or automatically
    /// </summary>
    private void gearShift()
    {
        if (!isGrounded())
            return;

        if (!AutomaticTransmission)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                gearNum++;
                GM.changeGear();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                gearNum--;
                GM.changeGear();
            }
        }

        else
        {
            if (max_gear_rpm < engineRPM && gearNum < gears.Length - 1)
            {
                gearNum++;
                GM.changeGear();
            }
            if (min_gear_rpm > engineRPM && gearNum > 0)
            {
                gearNum--;
                GM.changeGear();
            }
        }
    }

    /// <summary>
    /// Reset the car to the inital position once 
    /// reset input recieved
    /// </summary>
    public void ResetCar()
    {

        this.transform.eulerAngles = Vector3.zero;
        this.rigidbody.velocity = Vector3.zero;
        this.rigidbody.angularVelocity = Vector3.zero;
        this.transform.position = initialPos;
        IM.reset = false;
    }


    private void getObject()
    {
        IM = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();
    }


    /// <summary>
    /// Get the wheel slip and friction values 
    /// </summary>
    private void getFriction()
    {

        WheelHit wheelhit;
        axleInfos[0].rightWheel.GetGroundHit(out wheelhit);
        slip[0] = wheelhit.sidewaysSlip;

        axleInfos[0].leftWheel.GetGroundHit(out wheelhit);
        slip[1] = wheelhit.sidewaysSlip;

        axleInfos[1].rightWheel.GetGroundHit(out wheelhit);
        slip[2] = wheelhit.sidewaysSlip;

        axleInfos[1].leftWheel.GetGroundHit(out wheelhit);
        slip[3] = wheelhit.sidewaysSlip;

        SideExtremumSlip = slip;
    }

    /// <summary>
    /// Get the RPM of the wheels and update the gears
    /// </summary>
    private void wheelRPM()
    {
        float sum = 0;
        int R = 0;
        for(int i =0; i < 2; i++)
        {
            sum += axleInfos[i].rightWheel.rpm + axleInfos[i].leftWheel.rpm;
            R += 2;
        }
        wheelrpm = (R != 0) ? sum / R : 0;
    }

    /// <summary>
    /// Check if the wheels are touching the ground
    /// </summary>
    /// <returns>true if either wheel touches the ground</returns>
    private bool isGrounded()
    {
        return axleInfos[0].leftWheel.isGrounded &&
            axleInfos[0].rightWheel.isGrounded &&
            axleInfos[1].leftWheel.isGrounded &&
            axleInfos[1].rightWheel.isGrounded ;
    }


    /// <summary>
    /// Check whether the car is on the road or not
    /// </summary>
    private void roadCheck()
    {
        roadStatus = false;
        WheelHit hit;
        foreach(AxleInfo axle in axleInfos)
        {
            if(axle.leftWheel.GetGroundHit(out hit))
                roadStatus = roadStatus || hit.collider.CompareTag("Road");

            if(axle.rightWheel.GetGroundHit(out hit))
                roadStatus = roadStatus || hit.collider.CompareTag("Road");
        }

        
    }
}


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;

    public GameObject leftWheelmesh;
    public GameObject rightWheelmesh;

    public bool rear;
    public bool motor;
    public bool steering;
}