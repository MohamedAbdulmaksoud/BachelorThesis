using UnityEngine;
using System.Collections;

public class CoveyorController : MonoBehaviour
{
	public float speed;

	//acts as a electric switch for the conveyor belt
	//private bool power;

	//Current state of the system
	private bool state;



	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*if (Input.GetKeyDown ("space")) {
			power = !power;
			Debug.Log ("Power State " + power.ToString ());
		}*/

		state = gameObject.GetComponent<Actuator>().getState();
	
	}

	void OnCollisionStay (Collision collision)
	{
		if (collision.gameObject.CompareTag ("Box")) {			

			collision.rigidbody.MovePosition(collision.transform.position + setDirection(state) *Time.deltaTime*speed);
			}
		}


	Vector3 setDirection (bool state)
	{
		if (state)
			return Vector3.forward;
		else
			return Vector3.back;
	}
}
