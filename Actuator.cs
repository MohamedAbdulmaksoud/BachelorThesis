using UnityEngine;

public class Actuator : MonoBehaviour
{
    // ID of an actuator. MUST NOT be repeated.
    public int ID;
	//state representing the current output of the actuator.
	private bool state;
	void Start ()
	{        
	}
	void Update ()
	{
	}
	public void setState(bool state){
		this.state = state;
	}
    public bool getState()
    {
        return state;
    }
}