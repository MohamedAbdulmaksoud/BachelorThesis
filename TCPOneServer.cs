using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Linq;
using System.IO;

//for more info: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example

public class TCPOneServer : MonoBehaviour
{
    public static ManualResetEvent allDone = new ManualResetEvent(false);
    //public string _IPAddress; to be used with Unity UI
    private static IPAddress ip;
    // set the TcpListener on port 13000
    const int port = 13000;
    TcpListener server = new TcpListener(IPAddress.Any, port);
    //initialize array of actuators/sensors
    public GameObject[] actuators;
    public GameObject[] sensors;
    // data recieved
    byte[] data;
    // data to be sent. A single byte for each boolean value.
    byte[] sdata;
    //flag to terminate connection upon Unity exit.
    bool alive;   
    private void Awake()
    {
        alive = true;
        // Start listening for client requests
        server.Start();
    }
    // Use this for initialization
    void Start()
    {
        //Sorts the Actuators/Sensors of array in ascending order according to ID
        actuators = GameObject.FindGameObjectsWithTag("Actuator").OrderBy(Actuator => Actuator.GetComponent<Actuator>().ID).ToArray();
        sensors = GameObject.FindGameObjectsWithTag("Sensor").OrderBy(sensor => sensor.GetComponent<Sensor>().ID).ToArray();
        // Sets length of data to be received to be exactly equal to number of Actuators 
        data = new byte[256];
        sdata = new byte[256];
        // Start listening for client requests
        server.Start();
        Thread connect = new Thread(Read);
        connect.IsBackground = true;
        connect.Start();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (data != null)
        {
            // Update Actuators' states
            for (int i = 0; i < actuators.Length; i++)
            {
                actuators[i].GetComponent<Actuator>().setState(ToBoolean(data, actuators[i].GetComponent<Actuator>().ID));

            }

            //Updates sensors' states to be ready for sending
            foreach (GameObject sensor in sensors)
            {
                //sdata[sensor.GetComponent<Sensor>().ID] = To4DIACBoolean(sensor.GetComponent<Sensor>().getState());
                sdata[sensor.GetComponent<Sensor>().ID] = Convert.ToByte(sensor.GetComponent<Sensor>().getState());
            }

            //This approach is for checking on receiving data whether IDs given to the actuators differs from those given in 4DIAC.
            //It works by sending an array of bytes where default value is 2 since 0/1 equals false/true respectively.
            //It searches every Actuator ID and checks corresponding in data received index in array.
            /*for (int i = 0; i < actuators.Length; i++)
            {
                if (data[actuators[i].GetComponent<Actuator>().ID] == 2)
                {
                    Debug.Log("The Actuator:" + " " + actuators[i].name + " has a different ID than parameter given in 4DIAC");
                }
                else
                {
                    actuators[i].GetComponent<Actuator>().setState(ToBoolean(data, actuators[i].GetComponent<Actuator>().ID));
                }
            }*/
        }
    }
    void Read()
    {
        //check whether Unity is still active
        if (alive)
        {
            Thread.Sleep(200);
            // Set the event to nonsignaled state.
            allDone.Reset();
            Debug.Log("Waiting for Client connection... ");
            // Start an asynchronous socket to listen for connections.  
            server.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), server);
            // Wait until a connection is made before continuing.  
            allDone.WaitOne(); 
        }

    }

    private void AcceptCallback(IAsyncResult ar)
    {
       
        // Signal the main thread to continue.  
        allDone.Set();
        // Get the server that handles the client request.  
        TcpListener server = (TcpListener)ar.AsyncState;
        TcpClient client = server.EndAcceptTcpClient(ar);
        Debug.Log("Client Connected! :)");
        NetworkStream stream = client.GetStream();
        //Begins an asynchronous read. 
        stream.BeginRead(data, 0, data.Length, new AsyncCallback(ReadCallback), stream);
    }

    private void ReadCallback(IAsyncResult ar)
    {
        Thread.Sleep(200);
        // Retrieve the server's networkstream
        NetworkStream stream = (NetworkStream)ar.AsyncState;
        // Read actuator data from the client. 
        stream.ReadTimeout = 100;
        stream.EndRead(ar);
        stream.WriteTimeout = 1000;
        // Send sensor data to the stream.
        if (stream.CanWrite)
        {
            try
            {
                stream.Write(sdata, 0, sdata.Length);

            }
            catch(IOException e)
            {
                stream.Close();
                Debug.Log("Client has disconnected ... :(");
                Read();
            }
        }
          
        //Loop again as long as Unity is active
        if (alive)
            stream.BeginRead(data, 0, data.Length, new AsyncCallback(ReadCallback), stream);
        else stream.Close();
    }
    // Process the data received and changing it to boolean variable
    public bool ToBoolean(byte[] data, int i) { return Convert.ToBoolean((data[i] & 0x1)); }
    byte To4DIACBoolean(bool state)
    {
        if (state)
            return 0x41;
        else
            return 0x40;
    }
   
    void OnApplicationQuit()
    {
        
        alive = false;       
        server.Stop();
    }
   
}
