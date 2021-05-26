using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float vertical;
    public float horizontal;
    public bool reset;
    public bool handbrake;
    public bool brake;

    [HideInInspector]
    public bool heuristic = false;
    private Dictionary<int, float[]> actions = new Dictionary<int, float[]>();

    public void Start()
    {
        initActionDict();
    }
    public void FixedUpdate()
    {

        brake = (vertical < 0f) ? true : false;
    }
    
    /// <summary>
    /// Using the actions dictionary and the action index,
    /// get the values of the vertical , horizontal and handbrake actions
    /// </summary>
    /// <param name="actionNum"></param>
    public void getAction(int actionNum)
    {
        Debug.Log("Input Manager: " + actionNum);
        
        vertical = actions[actionNum][0];
        horizontal = actions[actionNum][1];
        handbrake = (actions[actionNum][2] == 1) ? true : false;
    }

    /// <summary>
    /// Get the Inputs from the user keyboard
    /// </summary>
    public void keyboardInput()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        handbrake = Input.GetKey(KeyCode.LeftShift);
        reset = Input.GetKey(KeyCode.Space);
        
    }

    /// <summary>
    /// Initialize a dictionary that maps the action indices
    /// to their action vaulues. Vertical, horizontal and handbrake
    /// </summary>
    public void initActionDict()
    {
        // vertical, horizontal, handbrake
        actions[0] = new float[] { 1, 0, 0 };        
        // forward

        actions[1] = new float[] { 1, 1, 0 };        
        // forward right
 
        actions[2] = new float[] { 0, 1, 0 };       
        // right
  
        actions[3] = new float[] { -1, 1, 0 };      
        // back right

        actions[4] = new float[] { -1, 0, 0 };        
        // back

        actions[5] = new float[] { -1, -1, 0 };        
        // back left
 
        actions[6] = new float[] { 0, -1, 0 };       
        // left
 
        actions[7] = new float[] { 1, -1, 0 };       
        // forward left

        actions[8] = new float[] { 0, 0, 1 };
        // handbrake

        actions[9] = new float[] { 0, 0, 0 };
        // Nothing

    }
}
