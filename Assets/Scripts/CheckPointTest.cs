using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTest : MonoBehaviour
{
    public GameObject CheckPointManager;
    public List<GameObject> checkpoints;
    public GameObject Car;

    public float distance = 0f;

    public void Start()
    {
        getCheckpoints();
    }
    public void Update()
    {
        TrackLength();
        
    }
    public void getCheckpoints()
    {
        GameObject temp;
        for (int i =0;i < CheckPointManager.transform.childCount; i++)
        {
            temp = CheckPointManager.transform.GetChild(i).gameObject;
            checkpoints.Add(temp);
        }
    }
    public void TrackLength()
    {
        if (checkpoints.Count > 0)
        {
            Vector3 lastPoint = Car.transform.position;
            Vector3 currentPoint = checkpoints[0].transform.position;
            distance = Vector3.Distance(lastPoint, currentPoint);
            lastPoint = currentPoint;
            foreach (GameObject c in checkpoints)
            {
                currentPoint = c.transform.position;
                distance += Vector3.Distance(lastPoint, currentPoint);
                lastPoint = currentPoint;
            }
        }
        else
            distance = 0;

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (checkpoints.Count > 0)
            if (collision.CompareTag("Checkpoint"))
            {
                if (collision.gameObject == checkpoints[0])
                {
                    checkpoints.RemoveAt(0);
                }
            }
    }

    public void reset()
    {
        checkpoints.Clear();
        Start();
    }
}
