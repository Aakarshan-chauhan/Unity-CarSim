using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject cam1, cam2;
    // Start is called before the first frame update
    void Start()
    {
        cam1.active = true;
        cam2.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            cam1.active = !cam1.active;
            cam2.active = !cam2.active;
        }
    }
}
