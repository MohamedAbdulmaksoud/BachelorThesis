using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class SController : MonoBehaviour {

    private int portNumber = 8888;
    private IPAddress multicastaddress = IPAddress.Parse("224.0.0.3");
    UdpClient sender;
    IPEndPoint remoteep;
    // data to be sent. A single byte for each boolean value.
    byte[] sdata;   
    public GameObject[] sensors;     
    private void Awake()
    {
        sender = new UdpClient();        
        sender.ExclusiveAddressUse = false;
        remoteep = new IPEndPoint(multicastaddress, portNumber);
        sensors = GameObject.FindGameObjectsWithTag("Sensor");       
    }

    // Use this for initialization
    void Start () {
        //Sorts the Sensors of array in ascending order according to ID.
        getSensors();    
        // Sets length of data to be sent to be exactly equal to number of Sensors.
        sdata = new byte[sensors.Length];
        sender.JoinMulticastGroup(multicastaddress);
    }
    //Sorts the Sensors of array in ascending order according to ID.
    void getSensors()
    {
        sensors = GameObject.FindGameObjectsWithTag("Sensor").OrderBy(sensor => sensor.GetComponent<Sensor>().ID).ToArray();
    }

    void Send()
    {              
            foreach (GameObject sensor in sensors)
            {                
                sdata[sensor.GetComponent<Sensor>().ID] = To4DIACBoolean(sensor.GetComponent<Sensor>().getState());
            }

            sender.Send(sdata, sdata.Length, remoteep);
            Thread.Sleep(30);
        
    }
    // terminates the socket upon exit form games.
    void OnApplicationQuit()
    {        
        sender.Close();
    }
    byte To4DIACBoolean(bool state)
    {
        if (state)
            return 0x41;
        else
            return 0x40;
    }
    

    // Update is called once per frame
    void Update () {
        Send();
       }
}
