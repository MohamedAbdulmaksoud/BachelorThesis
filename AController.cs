using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.Linq;
using System.Text;

public class AController : MonoBehaviour
{
    private int portNumber = 2222;
    private IPAddress multicastaddress = IPAddress.Parse("224.0.0.3");
    private IPEndPoint remoteep;
    UdpClient reciever;
    Thread recieverThread;
    bool alive; // flag to terminate thread cleanly
    byte[] data; // current datagram recieved
    byte[] rdata; // last datagram recieved

    public GameObject[] actuators;

    private void Awake()
    {
        alive = true;
        reciever = new UdpClient(portNumber);
        remoteep = new IPEndPoint(IPAddress.Any, portNumber);


    }
    // Use this for initialization
    void Start()
    {
        //Sorts the Sensors of array in ascending order according to ID.
        actuators = GameObject.FindGameObjectsWithTag("Actuator").OrderBy(Actuator => Actuator.GetComponent<Actuator>().ID).ToArray();

        reciever.JoinMulticastGroup(multicastaddress);
        recieverThread = new Thread(new ThreadStart(ReceiveData));
        recieverThread.IsBackground = true;
        recieverThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (data != null)
        {
            if (rdata == null)
                rdata = new byte[data.Length];

            if (dataChange(rdata, data))
            {
                rdata = data;
                for (int i = 0; i < data.Length; i++)
                {
                    actuators[i].GetComponent<Actuator>().setState(ToBoolean(data, i));
                }

                foreach (GameObject actuator in actuators)
                {
                    Debug.Log(actuator.name + " : " + actuator.GetComponent<Actuator>().getState().ToString());
                }
            }
        }

    }
    public void ReceiveData()
    {
        while (alive)
        {
            data = reciever.Receive(ref remoteep);
            Thread.Sleep(200);
        }
    }
    //Terminating the UDP connection upon exit from game.
    void OnApplicationQuit()
    {
        reciever.Close();
        alive = false;
    }
    // Process the data received and changing it to boolean variable
    public bool ToBoolean(byte[] data, int i) { return Convert.ToBoolean((data[i] & 0x1)); }
    //Compare two byte arrays
    static bool dataChange(byte[] a1, byte[] a2)
    {
        if (a1.Length != a2.Length)
            return true;

        for (int i = 0; i < a1.Length; i++)
            if (a1[i] != a2[i])
                return true;

        return false;
    }

}

