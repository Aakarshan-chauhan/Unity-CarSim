using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{

    public GameObject Player;
    public GameObject camTarget;
    public float speed;

    // Start is called before the first frame update
    public void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        camTarget = Player.transform.Find("CameraConstraint").gameObject;
    }

    public void FixedUpdate()
    {
        Follow();
    }

    public void Follow()
    {
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, camTarget.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(Player.gameObject.transform.position + Vector3.up * 2);
    }
}
