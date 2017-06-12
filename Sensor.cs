using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;


public class Sensor : MonoBehaviour
{

    //ID of Sensor; must be an integer and mustn't be repeated !
    public int ID;
    // The state of the sensor; 1=Object 0=None
    public bool state;
    //flag to terminate thread upon exit from game
    


    private void Awake()
    {
        state = false;
    }

    // Use this for initialization
    void Start ()
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
	//Functions to simulate a sensor
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag("Box"))
			state = true;
	}
	void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag("Box"))
			state = false;
	}
    public bool getState()
    {
        return state;
    }


}
