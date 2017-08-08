using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class ComPort  {

    private string portName;
    private int baudRate;
    //private SerialPort s_port = null;

    public void SetPortName(string name)
    {
        portName = name;
    }

    public void SetBaudRate(int rate)
    {
        baudRate = rate;
    }

    public string GetPortName()
    {
        return portName;
    }

    public int GetBaudRate()
    {
        return baudRate;
    }

    //public bool CreatePort(ComPort cp)
    //{
    //    s_port = new SerialPort(cp.portName
    //                  , cp.baudRate
    //                  , Parity.None
    //                  , 8
    //                  , StopBits.One);

    //    if (s_port != null)
    //        return true;
    //    else
    //        return false;
    //}

    //public SerialPort GetComPort()
    //{
    //    return s_port;
    //}

    //public SerialPort OpenComPort(string portname, int baud)
    //{
    //    SerialPort comport = null;
    //    comport = new SerialPort(portname
    //                  , baud
    //                  , Parity.None
    //                  , 8
    //                  , StopBits.One);

    //    try
    //    {
    //        // Open the port for communications 
    //        comport.Open();
    //        comport.ReadTimeout = 5;
    //        comport.WriteTimeout = 5;
    //        Debug.Log("COM Port Opened");
    //        comport.ReadBufferSize = 4096;

    //        if (comport.IsOpen)
    //        {
    //            comport.Write("0");
    //            Debug.Log("COM Port Opened"); //"Reset COM Port";
    //            //data_mode = 0;
    //        }
    //        else
    //            Debug.Log("Fail to Open COM Port");
    //    }
    //    catch
    //    {
    //        Debug.Log("Open COM Port Failed");
    //    }
    //    finally
    //    {
    //        Debug.Log("comport finally");
    //    }

    //    return comport;
    //}
}
