using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

    public GameObject needle;
    public GameObject car;
    private SimpleCarController carScript;

    private float startpos = 120;
    private float endpos = -120;
    private float relativepos;

    public Text kph;
    public Text gears;
    public Text OnRoad;
    public Text[] WheelSlip = new Text[4];
    // Start is called before the first frame update
    void Start()
    {
        carScript = car.GetComponent<SimpleCarController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        kph.text = carScript.KPH.ToString("0") + " km/h";
        updateNeedle();
        roadCheck();
        frictionCheck();
    }
    public void frictionCheck()
    {
        float[] slip = carScript.SideExtremumSlip;
        for (int i = 0; i < 4; i++)
        {
            if (slip[i] > 1)
            {
                WheelSlip[i].color = Color.red;
            }
            else
                WheelSlip[i].color = Color.green;


            WheelSlip[i].text = "Wheel" + i +" Slip: " + slip[i].ToString();
        }
    }
    private void updateNeedle() {
        relativepos = startpos - endpos;
        float temp = carScript.engineRPM / 8000;
        //temp = Mathf.Clamp(temp, 0, 1.1f);

        needle.transform.eulerAngles = new Vector3(0, 0, startpos - temp * relativepos);
    }

    public void changeGear()
    {
        gears.text = "Gear: " + carScript.gearNum.ToString();
    }

    public void roadCheck()
    {
        if (carScript.roadStatus)
        {
            OnRoad.color = Color.green;
            OnRoad.text = "On the Road";
        }
        else
        {
            OnRoad.color = Color.red;
            OnRoad.text = "Off the road";
        }
    }
}
