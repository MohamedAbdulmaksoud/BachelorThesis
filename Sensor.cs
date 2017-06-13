using UnityEngine;


public class Sensor : MonoBehaviour
{

    //ID of Sensor; must be an integer and mustn't be repeated !
    public int ID;
    // The state of the sensor; 1=Object 0=None
    public bool state;
   
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
