using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Globalization;
static class Constants
{
public const int SHORT_DELAY=10;
public const int LONG_DELAY=250;
}
public class SpacePointAlgorithm : MonoBehaviour
{
	public enum Levels
	{
		//MainMenu = 0,
		//DataDisplay,
		//FlyingMessenger,
		MotionTracking = 0,
		PointTracking//,
		//MalletGame
	}

//	[StructLayout(LayoutKind.Sequential)]
//	public struct DataPackage
//	{
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//		public float[] mag_raw;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//		public float[] mag_cal;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//		public float[] acc_raw;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//		public float[] gyro_raw;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
//		public float[] SIHI_in;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//		public float[] G_states_in;
//	}
//
//	[StructLayout(LayoutKind.Sequential)]
//	public struct Knobs
//	{
//		public float ATime;
//		public float MTime;
//		public float PTime;
//		public float FrTime;
//		public float eml_gbias_thresh;
//		public float hScale;
//		public float vScale;
//		public float jitterThrhold;
//		public byte AMode;
//		public byte LMode;
//		//[MarshalAs(UnmanagedType.ByValArray,SizeConst = 3)]
//		//public  byte[] SensorFlag;
//		public byte magFlag;
//		public byte accFlag;
//		public byte gyroFlag;
//		public byte resetRoll;
//		public byte resetRef;
//		public byte counter;
//		public byte lossLimit;
//		public byte filterBW;
//		public byte gCounter;
//		public float pointerMode;
//		public float dbfilt;
//	}
//    
//    [StructLayout(LayoutKind.Sequential)]
//    public struct AutoCALKnobs
//    {
//        public byte calFlag;
//        public byte coverageN;
//        public float stableFilter;
//        public float quality;
//    }
//
    [StructLayout(LayoutKind.Sequential)]
	public struct Output
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] position;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] rotation;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public float[] quaternion;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] acc_cal;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
		public float[] state;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
		public float[] SIHI_out;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] gBias;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] Gravity;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] Accelerate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] calStatus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] score;
	}

//	[StructLayout(LayoutKind.Sequential)]
//	public struct Coeff
//	{
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
//		public float[] magTrCoeffs;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//		public float[] magOffCoeffs;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
//		public float[] accTrCoeffs;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//		public float[] accOffCoeffs;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
//		public float[] gyroTrCoeffs;
//		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//		public float[] gyroOffCoeffs;
//	}

    [StructLayout(LayoutKind.Sequential)]
    public struct Statistics
    {
        public int total;
        public int lost;
        public int repeat;
        public float rate;
        public float lostpctge;
        public float repeatpctge;
    }
    static class Constants
    {
        public const int QDATABYTE = 26;
        public const int RAWDATABYTE = 27;
    }

	//public static DataPackage api_input, sensorData, temp_data;
//	public Knobs api_knobs;
	public static Output api_output;
//	public Coeff api_coeff;
//    public AutoCALKnobs autocal_knobs;

    //public int[] mag, acc, gyr, rawmag, q_out, pos_out;
    public float[] q_ay, acc_out;
    public static float[] hpr, hv_pos;
    private static int counter = 0, countdiff = 0;
    private byte[] indata;
    public string datalogMsg, filename, showknobMsg;
    public static string sttMsg;
    public bool log_data;
    public System.IO.StreamWriter file;
    public System.IO.StreamReader infile;
    public static bool hideKnobs, engShow, photoShow, rndShow;
    public static string demo_verstring = "SentralDemo rev D", fw_verstring="";
    public static float[] uncalHPR, caledHPR;
    private float resolution_ratio;
    private Vector3 vec2;
    public byte[] buffer2;
    public static byte data_mode = 0;
    public Statistics stats;
    public Texture pniLogo;

//	List<float> magRawXList;
//	List<float> magRawYList;
//	List<float> magRawZList;
//
//	List<float> magCalXList;
//	List<float> magCalYList;
//	List<float> magCalZList;
//
//	List<float> accRawXList;
//	List<float> accRawYList;
//	List<float> accRawZList;

    //serial port
    private static SerialPort sp;
    private int in_byte = 0;

	static byte[] statusArr; //cal status, transientCompensation flags, gbias
	static int[] rawdata;
	
    public static void OpenComPort(ComPort comport)
    {
        string portname = "COM4";
        int baudrate = 921600;
        portname = comport.GetPortName();
        baudrate = comport.GetBaudRate();

        Debug.Log("Port: " + portname + "Baud: " + baudrate.ToString());

        if (portname.Length > 4)
            portname = "\\\\.\\" + portname;

        sp = new SerialPort(portname
                             , baudrate
                             , Parity.None
                             , 8
                             , StopBits.One);
        try
        {
            // Open the port for communications 
            sp.Open();
            sp.ReadTimeout = 5;
            sp.WriteTimeout = 5;
            sttMsg = "COM Port Opened";
            sp.ReadBufferSize = 4096;

            if (sp.IsOpen)
            {
                sttMsg = sp.PortName + " Opened"; //"Reset COM Port";
                //data_mode = 2; 

            }
            else
                sttMsg = "Fail to Open " + sp.PortName;
        }
        catch (Exception e)
        {
			Debug.Log(e.Message);

        }
    }

    void Start()
    {
        q_ay = new float[4];
        acc_out = new float[3];
        hpr = new float[3];
        hv_pos = new float[2];

        stats = new Statistics();
        ResetStats();

        //buffer1 = new byte[27];
        buffer2 = new byte[52];

        api_output = new Output();
        api_output.position = new float[3] { 0, 0, 0 };
        api_output.rotation = new float[3] { 0, 0, 0 };
        api_output.acc_cal = new float[3] { 0, 0, 0 };
        api_output.quaternion = new float[4] { 0, 0, 0, 1 };
        api_output.state = new float[9];
        api_output.SIHI_out = new float[12]
        {
           1, 0, 0,
		   0, 1, 0,
		   0, 0, 1,
           0, 0, 0
        };
        api_output.gBias = new float[3];
        api_output.Gravity = new float[3] { 0, 0, 0 };
        api_output.Accelerate = new float[3] { 0, 0, 0 };

        sttMsg = "";
        log_data = false;
        datalogMsg = "Press 'L' to Log Data";
        showknobMsg = "Press 'H' to Hide Data\n";
        filename = "datalog.txt"; //dummy here
        hideKnobs = false;
        engShow = false;
		rndShow = false;
		
		//Dial related
        uncalHPR = new float[3];
        caledHPR = new float[3];

        resolution_ratio = Screen.height / 768f;
        vec2 = new Vector3(resolution_ratio, resolution_ratio, 1.0f);

//        magRawXList = new List<float>();
//        magRawYList = new List<float>();
//        magRawZList = new List<float>();
//
//        magCalXList = new List<float>();
//        magCalYList = new List<float>();
//        magCalZList = new List<float>();
//
//        accRawXList = new List<float>();
//        accRawYList = new List<float>();
//        accRawZList = new List<float>();

        //open the statefile, otherwise create one.
        //ReadStateFile(ref api_input.SIHI_in, ref api_input.G_states_in);

		statusArr = new byte[6];
		rawdata = new int [13];
		
        myStyle = new GUIStyle();
        myStyle.fontStyle = FontStyle.Bold;
				
    }
	
	public static void ReadFWVersion()
	{
		byte[] svnVersion = new byte[2];
		string cmdtext = "0"; //Read GP25=0x50, expect return 0 to show Sentral is ready
		string replytxt = "0";
		
		string verstr = "0";
		byte number = 0;
		if (fw_verstring == "")
		{
			ClearComPort(sp);
			

			pause(10);
			
			cmdtext = "{50 72 01}";	
			SendCmdToComPort(cmdtext);
			pause(25);
			ReceiveFromComPort(ref replytxt);
			Debug.Log("Reply: "+replytxt);
			char[] charsToTrim = {'\n', '\r',' '};
			verstr = replytxt.Trim(charsToTrim);
			
			

			replytxt = replytxt.Trim(charsToTrim);
			byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out number);
			svnVersion[0] = number;
			
			
			pause(10);
			 
			cmdtext = "{50 73 01}";	
			SendCmdToComPort(cmdtext);
			pause(25);
			ReceiveFromComPort(ref replytxt);
			Debug.Log("Reply: "+replytxt);
		
			verstr = replytxt.Trim(charsToTrim);
			
			

			replytxt = replytxt.Trim(charsToTrim);
			byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out number);
			svnVersion[1] = number;
			
			int retval = BitConverter.ToInt16(svnVersion, 0);
			
			fw_verstring=retval.ToString();
			
			
				sttMsg = "Read FW Ver Done";
		
		
				
			SendCmdToComPort(":"); //Exit TerminalMode	
		}
	}
	
	void ReadStateFile (ref float[] sihi, ref float[] gbias)
	{
		string statefilename = "state.csv";
		System.IO.StreamReader statefile;
		try {
			statefile = new System.IO.StreamReader (statefilename);
			string textline;
			string[] values = null;
			if (statefile != null) {
				//textline = infile.ReadLine(); //in case empty line
				
				for (int row = 0; row < 4; row++) {
					textline = statefile.ReadLine ();
					//Read a line of data
					values = textline.Split (',');
					if (values.Length > 0 && values[0].Trim ().Length > 0) {
						for (int col = 0; col < 3; col++) {
							sihi[row * 3 + col] = float.Parse (values[col]);
						}
					}
				}
				
				textline = statefile.ReadLine ();
				//Read a line of data
				values = textline.Split (',');
				if (values.Length > 0 && values[0].Trim ().Length > 0) {
					for (int col = 0; col < 3; col++) {
						gbias[col] = float.Parse (values[col]);
					}
				}
			} else {
				sttMsg = "Empty to read: " + statefilename;
			}
			statefile.Close ();
		} catch {
			sttMsg = "Fail to open:" + statefilename;
		}
	}

	void WriteStateFile (float[] sihi, float[] gbias)
	{
		string statefilename = "state.csv";
		System.IO.StreamWriter statefile;
		int i, j;
		try {
			statefile = new System.IO.StreamWriter (statefilename);
			//Mag cal coeffs
			for (i = 0; i < 3; i++) {
				for (j = 0; j < 3; j++) {
					statefile.Write (sihi[3 * i + j]);
					statefile.Write (",");
				}
				statefile.WriteLine ();
			}
			
			for (i = 0; i < 3; i++) {
				statefile.Write (sihi[9 + i]);
				statefile.Write (",");
			}
			statefile.WriteLine ();
			
			for (i = 0; i < 3; i++) {
				statefile.Write (gbias[i]);
				statefile.Write (",");
			}
			statefile.WriteLine ();
			statefile.Close ();
		} catch {
			sttMsg = "Fail to write:" + statefilename;
		}
	}
	
	public void switch_toggle(int type, bool flag_in)
	{
		
		
		
		
		float sensorFlags = GetKnob(57);
		
		byte[] sensorFlagsArray = BitConverter.GetBytes(sensorFlags);
		
		Debug.Log("switch_toggle("+type+","+flag_in+")");
		
		 

		switch (type)
		{
		case 0:
		case 1:
			
			sensorFlagsArray[0] = Convert.ToByte(flag_in);
			
			break;
		case 2:
			
			sensorFlagsArray[1] = Convert.ToByte(flag_in);;
			
			break;
		case 3:
			
			sensorFlagsArray[2] = Convert.ToByte(flag_in);;
			break;
				
			
		default: break;
		}
		
		
		float knob_val = System.BitConverter.ToSingle(sensorFlagsArray, 0);
		
	SetKnob(57,knob_val);
			
	}
	

	public bool read_toggle(int type, out bool status)
	{
		bool flag = false;
		status = true;
		
		
		
		float sensorFlags = GetKnob(57);
		
		byte[] sensorFlagsArray = BitConverter.GetBytes(sensorFlags);
		
		switch (type)
		{
		case 0: //terminal mode
			break;
		
		//Sensor flags
		case 1:
			flag = Convert.ToBoolean(sensorFlagsArray[0]);
			break;
		case 2:
			flag = Convert.ToBoolean(sensorFlagsArray[1]);
			break;
		case 3:
			flag = Convert.ToBoolean(sensorFlagsArray[2]);
			break;
		default: break;
		}
				
		return flag;
	}	
	
	enum RangeType
	{
		RANGE25DOT5 = 0,
		RANGE12DOT7 = 1,
		RANGE255 = 2,
		RANGEBOOLEAN = 3
	
	}
	
	enum ScaleType
	{
		TEN = 10,
		ONE = 1
	
	}
	
//	enum SentralKnobID
//	{
//		ATIME = 1,
//		MTIME,
//		stopActionFilte,
//		magTransient_cntrl_1,
//		magTransient_cntrl_2,
//		transientCompensationFilter,
//		sensorFlags_0,
//		sensorFlags_1,
//		sensorFlags_2,
//		stillnessMode,
//		dynamic_accel_mode,
//		gbias_thresh,
//		stillDelay_0,
//		stillDelay_1,
//		stillDelay_2,
//		noiseMult
//	}
	
	const string 	SENTRALADDR = "50",//0x50
					GP24 = "49",	 //0x49
					GP25 = "4e", 	 //0x50 changed to 0x4e later on
					GP34 = "59",
					GP35 = "5a",
					GP36 = "5b";
	
	const float FLOAT999 = 999;
	const Int16 INT999 = 999;
	
	float SentralConvert(byte inbyte, RangeType type, ScaleType scaler)
	{
		float retval = FLOAT999;
		
		switch (type)
		{
		case RangeType.RANGE25DOT5:
				retval = Convert.ToInt16(inbyte);
			break;
		case RangeType.RANGE12DOT7:
			byte[] bytes = new byte[2];
			bytes[0] = inbyte;
			if ((inbyte & 0x80) == 0x80)
				bytes[1] = 0xff;
			else
				bytes[1] = 0x00;
				
			retval = BitConverter.ToInt16(bytes, 0);
			
			break;
		case RangeType.RANGE255:
		case RangeType.RANGEBOOLEAN:
			retval = Convert.ToInt16(inbyte);
			break;

		default: break;
		}
		
		float scale = 1.0f;
		
		switch (scaler)
		{
		case ScaleType.ONE:
			scale = 1.0f;
			break;
			
		case ScaleType.TEN:
			scale = 10.0f;
			break;
		default:
			break;
		}
		
		retval /= scale;
		
		Debug.Log("SentralConvert: "+retval);
		
		return retval;
	}
	
	//1 based Sentral KnobID as input
	float ParsePayload(string str_in, int knob_id)
	{
		float knob_val = FLOAT999;
		
		byte number = 255;
		//Need System.Globalization
		bool canConvert = byte.TryParse(str_in, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out number);
		Debug.Log(canConvert+", "+number);
		if ((str_in.Length > 0) && canConvert && (number != 255))
		{			
			byte mybyte = Byte.Parse(str_in, System.Globalization.NumberStyles.HexNumber);
			
			RangeType rangetype = RangeType.RANGE25DOT5;
			ScaleType scaletype = ScaleType.TEN;
			
			switch (knob_id)
			{
			case 3: //stop action
			case 10://stillmode
				rangetype = RangeType.RANGE12DOT7;
				scaletype = ScaleType.TEN;
				break;
			case 1:	//aTime
			case 2:	//mTime
			case 4:	//mtrans_ctrl
			case 5:	//mtrans_ctrl
			case 12://gbias_thresh
			case 16://noiseMulti
				rangetype = RangeType.RANGE25DOT5;
				scaletype = ScaleType.TEN;
				break;
			case 6:	//transCom
			case 11://dyn_acc_mode
			case 13://stlldly0
			case 14://stlldly1
			case 15://stlldly2
				rangetype = RangeType.RANGE255;
				scaletype = ScaleType.ONE;
				break;
			case 7://magSensor
			case 8://accSensor
			case 9://gyrSensor
				rangetype = RangeType.RANGEBOOLEAN;
				scaletype = ScaleType.ONE;
				break;
				
			default: break;
			}
					
			knob_val = SentralConvert(mybyte, rangetype, scaletype);
			Debug.Log("mybyte: " + mybyte +", knob_val: " + knob_val +", Reply: "+str_in);
			sttMsg = "";
		}
		else
			sttMsg = "Read Fail";
		
		return knob_val;
	}
	
	//pause in ms
	static void pause(int ms)
	{
		System.Threading.Thread.Sleep(ms);
	}
	
//Here's how I am planning to implement the readback functionality.
//unity writes knob identifier (see ID column in knobs table below)to GP35. 
//unity writes  0x02 to GP34 (Read command)
//Sentral writes knob value to GP26 and writes 0x01 to GP25
//Unity reads GP25 to see if it is 1 and read backs knob value from GP26
//Unity writes 0x00 to GP34
//Sentral clears GP25 & GP26.
	
	//1 based ID as input
	float GetKnob(int knobID) 
	{
		
		
byte[] savedParams = new byte[4];
		
		

		float knob_val = 0;
		 
		ClearComPort(sp);
		string cmdtext = "0"; //Read GP25=0x50, expect return 0 to show Sentral is ready
		char[] charsToTrim = {'\n', '\r', ' '};
		
		byte number = 255;
		//Need System.Globalization
		 
		SendCmdToComPort(":"); //Enter TerminalMode
		pause(25);
		
		//knobID = knobID +128; //set MSB high
		string SentralKnobID = knobID.ToString("x2"); //Sentral Id is 1 based
		cmdtext = "[50 64 "+SentralKnobID;
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
		pause(25);
			
		
		cmdtext = "{50 54 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);
		pause(25);
		string replytxt="";
		ReceiveFromComPort(ref replytxt);
		
		byte anglorithmcontrol=0;
		byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out anglorithmcontrol);
		anglorithmcontrol+=128;
		
		pause(25);
		
		cmdtext = "[50 54 "+anglorithmcontrol.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		pause(25);
		
		cmdtext = "{50 3a 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);
		pause(25);
		string confirmreplytxt="";
				
		ReceiveFromComPort(ref confirmreplytxt);
				
		confirmreplytxt = confirmreplytxt.Trim(charsToTrim);
		
		if(confirmreplytxt.CompareTo(replytxt)==1)
			sttMsg = "Read successful.";
		else
			sttMsg = "Read operation failed.";
		pause(25);
		
		

	
		

		if(knobID!=56){
		
		cmdtext = "{50 3b 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out number);
		savedParams[0] = number;
		
		
		
		pause(25);
		
		cmdtext = "{50 3c 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out number);
		savedParams[1] = number;
		
		knob_val += number<<8;
		pause(25);
		
		
		cmdtext = "{50 3d 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out number);
		
		savedParams[2] = number;
		
		pause(25);
		
		
		cmdtext = "{50 3e 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out number);
		
		savedParams[3] = number;
		
		
		pause(25);
		
		
		
		knob_val = System.BitConverter.ToSingle(savedParams, 0);
		}
		
		else{
			
		cmdtext = "{50 3b 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out number);
		savedParams[0] = number;
			
		knob_val = number;
		
		
		
		pause(25);
			
			
		}
	
	    pause(25);
		int temp = 0;
		cmdtext = "[50 64 "+temp.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
		 
		 
	 
		
		
		pause(25);
		
		anglorithmcontrol &= 0x7f;	
		cmdtext = "[50 54 "+anglorithmcontrol.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		pause(25);
		


		SendCmdToComPort(":"); //Exit TerminalMode

		return knob_val;
	}
	
	
//1)Unity sends '0' to the 8051 to halt packet transmission
//2)Unity sends ':' to enter terminal mode
//3)unity reads GP25, it will be 0x00 if sentral is ready. (see todo in red below)
//4)unity writes knob identifier (see ID column in knobs table below)to GP35.
//5)unity writes knob value to GP36.
//6)unity writes  0x01 to GP34 to commit change.
//Sentral continuously checks the value of GP34 inside the GyroCalculations interrupt function,
//if it is 0x01 it will change the knob indicated by the index number in GP35 to the value in GP36.
//Once Sentral has changed the knob value it will write 0x01 to GP25.
	
//7)unity checks GP25 to know when the knob value has changed, when GP25 changes from 0x00 to 0x01 it will write 0x00 to GP34.
//Sentral will detect this change and clear GP25 to 0x00.
//8)Unity polls/checks GP25 to see if it is zero.
	
	
	//1 based ID as input
	void SetKnob(int knobID, float valuein)
	{		
		
		byte[ ] byteArray = BitConverter.GetBytes( valuein );
		
		
		 
	 
		ClearComPort(sp);
		string cmdtext = "0"; //Read GP25=0x50, expect return 0 to show Sentral is ready
		char[] charsToTrim = {'\n', '\r', ' '};
		
		 
		//Need System.Globalization
		 
		SendCmdToComPort(":"); //Enter TerminalMode
		pause(25);
		int temp;
		
		if(knobID!=56){
		pause(25);
		temp = byteArray[0];
		cmdtext = "[50 60 "+temp.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
		 
		 
	 
		
		
		pause(25);
		temp = byteArray[1];
		cmdtext = "[50 61 "+temp.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
		 
		 
 
		
		
	    pause(25);
		temp = byteArray[2];
		cmdtext = "[50 62 "+temp.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
		 
		 
		 
		
		pause(25);
		temp = byteArray[3];
		cmdtext = "[50 63 "+temp.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
		 
		 
		pause(25);
		//savedParams[3] = number;
			
		}
		else{
			
		pause(25);
		temp = (byte)valuein;
		cmdtext = "[50 60 "+temp.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
			
			
			
		} 
	 
		knobID = knobID +128; //set MSB high
		string SentralKnobID = knobID.ToString("x2"); //Sentral Id is 1 based
		cmdtext = "[50 64 "+SentralKnobID;
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
		pause(25);
			
		
		cmdtext = "{50 54 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);
		pause(25);
		string replytxt="";
		ReceiveFromComPort(ref replytxt);
		
		byte anglorithmcontrol=0;
		byte.TryParse(replytxt, NumberStyles.HexNumber,CultureInfo.InvariantCulture, out anglorithmcontrol);
		anglorithmcontrol+=128;
		pause(25);
		
		cmdtext = "[50 54 "+anglorithmcontrol.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		pause(150);
		
		
	    pause(25);
		temp = 0;
		cmdtext = "[50 64 "+temp.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);
		 
		 
		
	 
		
		
		pause(25);
		
		cmdtext = "{50 3a 01"; //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "}";
		SendCmdToComPort(cmdtext);
		pause(25);
		string confirmreplytxt="";
				
		ReceiveFromComPort(ref confirmreplytxt);
				
		confirmreplytxt = confirmreplytxt.Trim(charsToTrim);
		
		if(confirmreplytxt.CompareTo(replytxt)==1)
			sttMsg = "Write successful.";
		else
			sttMsg = "Write operation failed.";
		pause(25);
		
	    anglorithmcontrol &= 0x7f;	
		cmdtext = "[50 54 "+anglorithmcontrol.ToString("x2"); //}";	
		SendCmdToComPort(cmdtext);
		pause(10);
		cmdtext = "]";
		SendCmdToComPort(cmdtext);		
		replytxt="";
		pause(25);
		ReceiveFromComPort(ref replytxt);
		replytxt = replytxt.Trim(charsToTrim);
		pause(25);
		

		
		//knob_val = System.BitConverter.ToSingle(savedParams, 0);
		
		


		SendCmdToComPort(":"); //Exit TerminalMode
		
		
		
		
		
		
	  
	}

	public void adjust (int knobID, float value_in)
	{
		//string cmdtext = "";
		Int16 data = INT999;
		
		int paramNumber;
		
		
				if (knobID < 6)
			{
				
				 
				
			}
			else if (knobID < 13)
			{
				knobID = knobID + 3;
				 
			}	
		
		switch (knobID)
		{
		case 0: //aTime
			paramNumber = 53; //Sentral Id is 1 based
			
			break;
		case 1: //mTime
			paramNumber = 52; //Sentral Id is 1 based
			break;
		case 2: //stopAction
			paramNumber = 58; //Sentral Id is 1 based
			break;
		case 3: //magTransiantCtrl1
			paramNumber = 60; //Sentral Id is 1 based
			break;
		case 4: //magTransiantCtrl2
			paramNumber = 62; //Sentral Id is 1 based
			break;
			
			case 5: //transientCompensationFilter
			paramNumber = 59; //Sentral Id is 1 based
			break;
			
			case 6: //sensorFlags_0
			paramNumber = 0; //Sentral Id is 1 based
			break;
			
			
			case 7: //sensorFlags_1
			paramNumber = 0; //Sentral Id is 1 based
			break;
			
			case 8: //sensorFlags_2
			paramNumber = 0; //Sentral Id is 1 based
			break;
			
			case 9: //stillnessMode
			paramNumber = 73; //Sentral Id is 1 based
			break;
			
			
			case 10: //dynamic_accel_mode
			paramNumber = 56; //Sentral Id is 1 based
			break;
			
			case 11: //gbias_thresh
			paramNumber = 55; //Sentral Id is 1 based
			break;
			
			case 12: //stillDelay_0
			paramNumber = 66; //Sentral Id is 1 based
			break;
			
			case 13: //stillDelay_1
			paramNumber = 67; //Sentral Id is 1 based
			break;
			
			case 14: //stillDelay_2
			paramNumber = 68; //Sentral Id is 1 based
			break;
			
			case 15: //noiseMult
			paramNumber = 65; //Sentral Id is 1 based
			break;
			
		default:
			paramNumber = 0;
			break;
	}
 
				SetKnob(paramNumber, value_in);
			 

	}
	
	public float read_knob(int knobID)
	{
		float knob_val = FLOAT999;
		Debug.Log("read_knob("+knobID+")");
		string cmdtext;
		
		ClearComPort(sp);
		
			if (knobID < 6)
			{
				knobID = knobID;
				 
				
			}
			else if (knobID < 13)
			{
				knobID = knobID + 3;
				 
			}
		
	//	1	ATIME				real32_T		10			0 to 10		float				
//	2	MTIME				real32_T		10			0 to 10		float
//	3	stopActionFilter 	real32_T		10			0 to 10		float
//	4	magTransient_cntrl_1	real32_T	10			0 to 10		float
//	5	magTransient_cntrl_2	real32_T	10			0 to 10		float
//	6	transientCompensationFilter	real32_T 1			0 or 1			integer
//	7	sensorFlags_0,		boolean_T		1			0 or 1			integer
//	8	sensorFlags_1,		boolean_T		1			0 or 1			integer
//	9	sensorFlags_2,		boolean_T		1			0 or 1			integer
//	10	stillnessMode		real32_T		10			0 or 1			integer
//	11	dynamic_accel_mode	uint8_T			1			0 to 1			integer
//	12	gbias_thresh		real32_T		10			0 to 10			float
//	13	stillDelay_0		real32_T		1			0 to 100		integer
//	14	stillDelay_1		real32_T		1			0 to 25			integer
//	15	stillDelay_2,		real32_T		1			0 to 100		integer
//	16	noiseMult			real32_T		10			0 to 10			float	
	switch (knobID)
		{
		case 0: //aTime
			knob_val = GetKnob(53); //Sentral Id is 1 based
			
			break;
		case 1: //mTime
			knob_val = GetKnob(52); //Sentral Id is 1 based
			break;
		case 2: //stopAction
			knob_val = GetKnob(58); //Sentral Id is 1 based
			break;
		case 3: //magTransiantCtrl1
			knob_val = GetKnob(60); //Sentral Id is 1 based
			break;
		case 4: //magTransiantCtrl2
			knob_val = GetKnob(62); //Sentral Id is 1 based
			break;
			
			case 5: //transientCompensationFilter
			knob_val = GetKnob(59); //Sentral Id is 1 based
			break;
			
			case 6: //sensorFlags_0
			knob_val = GetKnob(0); //Sentral Id is 1 based
			break;
			
			
			case 7: //sensorFlags_1
			knob_val = GetKnob(0); //Sentral Id is 1 based
			break;
			
			case 8: //sensorFlags_2
			knob_val = GetKnob(0); //Sentral Id is 1 based
			break;
			
			case 9: //stillnessMode
			knob_val = GetKnob(73); //Sentral Id is 1 based
			break;
			
			
			case 10: //dynamic_accel_mode
			knob_val = GetKnob(56); //Sentral Id is 1 based
			break;
			
			case 11: //gbias_thresh
			knob_val = GetKnob(55); //Sentral Id is 1 based
			break;
			
			case 12: //stillDelay_0
			knob_val = GetKnob(66); //Sentral Id is 1 based
			break;
			
			case 13: //stillDelay_1
			knob_val = GetKnob(67); //Sentral Id is 1 based
			break;
			
			case 14: //stillDelay_2
			knob_val = GetKnob(68); //Sentral Id is 1 based
			break;
			
			case 15: //noiseMult
			knob_val = GetKnob(65); //Sentral Id is 1 based
			break;
			
		default: break;
	}

	/*	
		int sentral_knob_id = INT999;
		//Convert Unity Knob Sequencial ID to Sentral knob ID
		if (knobID >= 0)
		{
			if (knobID < 6)
			{
				sentral_knob_id = knobID + 1;
				knob_val = GetKnob(sentral_knob_id); //Sentral Id is 1 based
				
			}
			else if (knobID < 13)
			{
				sentral_knob_id = knobID + 4;
				knob_val = GetKnob(sentral_knob_id);
			}
		}
		*/
		if (!rndShow)	
		{
			//StopAction bandage for AL
			if ((knobID == 2) && (knob_val == -1.2f)) //StopAct
			{
				Debug.Log("Knob " + knobID +" StopAction value:"+knob_val);
				knob_val = 0;
			}
		}	
		
	 
		return knob_val;
	}

	public static int [] data_out = {0,0,0,0};
	
	public int read_feature_sensors(byte id)
	{
	
		//int [] feature_data = new int[4];
		string replytxt = "", sentence = "";
		
		int ret_val = 1;
		 
			ClearComPort(sp);
			
		switch (id)
		{
		case 1:
			pause(10);
			string cmdtext = "[50 58 02]";	
			SendCmdToComPort(cmdtext);
			pause(25);
			
			cmdtext = "{50 2A 08}";	
			SendCmdToComPort(cmdtext);
			break;
		case 2:
			pause(10);
			cmdtext = "{50 48 08}";	//no need 8 bytes, just keep here to match case 1
			SendCmdToComPort(cmdtext);
			break;
			
		default: 
			return 0;
		}
		
		//Process receiving UART ASCII data
		pause(50);
		ReceiveFromComPort(ref replytxt);
		Debug.Log("Reply: "+replytxt);
		char[] charsToTrim = {'\n', '\r'};
		
		sentence = replytxt.Trim(charsToTrim);

		// Divide the sentence into words
        string[] Words = sentence.Split(' ');
		
		for (int i = 0; i < Words.Length; i++)
			Debug.Log(Words[i]);
		
        if (Words[0] != "")
        {
        //    Words[index] = Words[index].Substring(1);

            //debugstring = Words[index].Substring(0);
            for (int i = 0; i < (Words.Length / 2); i++)
            {
				//Debug.Log("Words[2*"+i+"] = "+Words[i]+" and Words[2*"+i+"+1] = "+Words[i*2+1]);
				if (Words[i*2] != "")
				{
					data_out[i] = Int16.Parse(Words[i*2], System.Globalization.NumberStyles.HexNumber);
					data_out[i] += (Int16.Parse(Words[i*2+1], System.Globalization.NumberStyles.HexNumber) << 8);
					Debug.Log(data_out[i]);
				}
            }
        }
		else
			ret_val = 0;
						
			
		SendCmdToComPort(":"); //Exit TerminalMode
		
		return ret_val;

	}



	void Update ()
	{
		if (Input.GetKey (KeyCode.LeftControl))
			if (Input.GetKey (KeyCode.LeftShift))
				if (Input.GetKey (KeyCode.LeftAlt))
					if (Input.GetKeyUp (KeyCode.F10)) {
						engShow = !engShow;
						if (photoShow)
							photoShow = false;					
					}
        
        if ((Application.loadedLevel != (int)SpacePointAlgorithm.Levels.MotionTracking) &&
            (Application.loadedLevel != (int)SpacePointAlgorithm.Levels.PointTracking)// &&
		    )
        {
            //hideKnobs = true;
            engShow = false;
        }
		
		if (engShow)
			if (Input.GetKey(KeyCode.F1))
				rndShow = !rndShow;
		
	}
	
	private static float deltaTime = 0;
	
	void FixedUpdate()
	{
		deltaTime = Time.deltaTime;
		if (data_mode == 2)
        {
            try
            {
				in_byte = sp.Read(buffer2, 0, (int)buffer2.Length);

				//if ((in_byte >= 26) && (buffer2[0] == '$') && (buffer2[buffer2.Length -2] == 0x0D) && (buffer2[buffer2.Length -1] == 0x0A))
				if ((in_byte >= 52) && (buffer2[0] == '$') && (buffer2[buffer2.Length -2] == 0x0D) && (buffer2[buffer2.Length -1] == 0x0A))
				{				
                        if (BlueBoxParser.ParseSentralLongData(buffer2, ref statusArr, ref q_ay, ref acc_out, ref hv_pos, ref rawdata))
                        {
                            //BlueBoxParser.Convert2PNIQ(q_ay, ref api_output.quaternion);

                            Q2HPR(q_ay, ref hpr);
						
							for (int i = 0; i < 3; i++)
							{
								api_output.quaternion[i] = q_ay[i];
								caledHPR[i] = hpr[i];
							}
                            api_output.quaternion[3] = q_ay[3];
							
//                            api_output.SIHI_out[1] = api_output.state[3];
//							
//                            api_output.SIHI_out[2] = api_output.state[4];

							//acc_out fields are used for cal result HI
//							api_output.state[5] = acc_out[0];
//							api_output.state[6] = acc_out[1];
//							api_output.state[7] = acc_out[2];
							

							//HVpos[0] sub for Calscore
							api_output.state[8] = hv_pos[0] * 100;
							
							countdiff = statusArr[0] - counter;
							if (countdiff < 0)
								countdiff += 256;
						
							counter = statusArr[0];
							UpdateStats(countdiff);
							
                        }
					
                        if (log_data)
						{
							if (engShow)
								DataLogEng(rawdata,acc_out, q_ay, hpr);
							else
								DataLogAandQ(acc_out, q_ay, hpr, caledHPR, 0); //byteIndex=21
						}

                    }//end of good package
					else
					{
						sp.ReadTo("\r\n");
						//Debug.Log("Bad Data Pkg, Recover here");
					}
            }//end of read SP
            catch (Exception e)
            {
                in_byte = 0;
				if (!e.Message.Contains("timed-out"))
					Debug.Log(e.Message);
				if (e.Message.Contains("port is not open"))
					sttMsg = e.Message;
            }
            finally
            {
            }
        }
    }
	
    void UpdateStats(int countdiff)
    {
        //Update Statistics
        if (countdiff == 0 && stats.total > 0)
            stats.repeat++;
        else if (countdiff > 1)
            stats.lost += (countdiff - 1);

        stats.total += countdiff;
        if (stats.total > 0)
        {
            stats.lostpctge = 100f * (float)stats.lost / (float)stats.total;
            stats.repeatpctge = 100f * (float)stats.repeat / (float)stats.total;
        }
    }

    void ResetStats()
    {
        stats.repeat = 0;
        stats.lost = 0;
        stats.total = 0;
        stats.lostpctge = 0;
        stats.repeatpctge = 0;
    }

    static bool  bgcalflag = false;
    static bool isblueboxcaled = false;
    public static bool GetBackgroundCalStatus()
    {
        if (statusArr[2] >= 3)
        {
            bgcalflag = true;
            blueboxMsg = "BlueBox is calibrated\n";
        }
        else
        {
            bgcalflag = false;
            blueboxMsg = "Please Move the BlueBox\n";
        }

        return bgcalflag;
    }

    static string blueboxMsg = "";
    bool firstbboxreadflag = true;
    public static bool GetBlueBoxCaledFlag()
    {
        return isblueboxcaled;
    }


    void Q2HPR(float[] q, ref float[] ypr)
    {
        ypr[0] = (180 / Mathf.PI) * Mathf.Atan2((2 * q[0] * q[1] + 2 * q[3] * q[2]), (2 * q[3] * q[3] + 2 * q[0] * q[0] - 1));
        ypr[1] = (180 / Mathf.PI) * Mathf.Asin(-(2 * q[0] * q[2] - 2 * q[3] * q[1]));
        ypr[2] = (180 / Mathf.PI) * Mathf.Atan2((2 * q[1] * q[2] + 2 * q[3] * q[0]), (2 * q[3] * q[3] + 2 * q[2] * q[2] - 1));
        if (ypr[0] < 0)
            ypr[0] += 360;
    }

    void SaveToFlash()
    {
        byte[] stringToWrite;
        stringToWrite = new byte[9];
    }
    void MagAccToHPR(float[] mag, float[] acc, ref float[] hpr)
    {
        float tempM, tempA;

        //heading
        tempM = Mathf.Pow(mag[0], 2) + Mathf.Pow(mag[1], 2) + Mathf.Pow(mag[2], 2);
        tempM = Mathf.Sqrt(tempM);

        tempA = Mathf.Pow(acc[0], 2) + Mathf.Pow(acc[1], 2) + Mathf.Pow(acc[2], 2);
        tempA = Mathf.Sqrt(tempA);

        for (int i = 0; i < 3; i++)
        {
            mag[i] = mag[i] / tempM;
            acc[i] = acc[i] / tempA;
        }

        tempM = mag[0] * (Mathf.Pow(acc[1], 2) + Mathf.Pow(acc[2], 2))
                - mag[1] * acc[0] * acc[1]
                - mag[2] * acc[0] * acc[2];

        if (tempM != 0.0)
        {
            hpr[0] = (180 / Mathf.PI) * Mathf.Atan2((mag[2] * acc[1] - mag[1] * acc[2]), tempM);
        }

        if (hpr[0] < 0.0)
        {
            hpr[0] += 360;
        }

        //pitch & roll
        // get pitch and roll angles, convert to an angle in degrees
        tempA = Mathf.Sqrt(Mathf.Pow(acc[1], 2) + Mathf.Pow(acc[2], 2));

        if (tempA == 0.0)
        {
            if (acc[0] > 0.0)
            {
                hpr[1] = -90;
            }
            else
            {
                hpr[1] = 90;
            }
        }
        else
        {
            hpr[1] = Mathf.Atan(-acc[0] / tempA) * (180 / Mathf.PI);
        }

        if (acc[2] == 0.0)
        {
            if (acc[1] > 0.0)
            {
                hpr[2] = 90;
            }
            else
            {
                if (acc[1] < 0.0)
                {
                    hpr[2] = -90;
                }
                else
                {
                    hpr[2] = 0;
                }
            }
        }
        else
        {
            hpr[2] = Mathf.Atan2(acc[1], acc[2]) * (180 / Mathf.PI);
        }
    }

//void DataFilter (float[] indata, byte listType, ref float[] movingAverage)
//{
//    uint depth = 75;
//    		
//    //MagRawList
//    if (listType == 0) {
//        if (magRawXList.Count >= depth) {
//            magRawXList.RemoveAt (0);
//            magRawYList.RemoveAt (0);
//            magRawZList.RemoveAt (0);
//        }
//        magRawXList.Add (indata[0]);
//        magRawYList.Add (indata[1]);
//        magRawZList.Add (indata[2]);
//        movingAverage[0] = getAverage (magRawXList);
//        movingAverage[1] = getAverage (magRawYList);
//        movingAverage[2] = getAverage (magRawZList);
//    } else if (listType == 1) {
//        if (accRawXList.Count >= depth) {
//            accRawXList.RemoveAt (0);
//            accRawYList.RemoveAt (0);
//            accRawZList.RemoveAt (0);
//        }
//        accRawXList.Add (indata[0]);
//        accRawYList.Add (indata[1]);
//        accRawZList.Add (indata[2]);
//        movingAverage[0] = getAverage (accRawXList);
//        movingAverage[1] = getAverage (accRawYList);
//        movingAverage[2] = getAverage (accRawZList);
//    //MagCalList
//    } else if (listType == 2) {
//        if (magCalXList.Count >= depth) {
//            magCalXList.RemoveAt (0);
//            magCalYList.RemoveAt (0);
//            magCalZList.RemoveAt (0);
//        }
//        magCalXList.Add (indata[0]);
//        magCalYList.Add (indata[1]);
//        magCalZList.Add (indata[2]);
//        movingAverage[0] = getAverage (magCalXList);
//        movingAverage[1] = getAverage (magCalYList);
//        movingAverage[2] = getAverage (magCalZList);
//    }
//		
//}

float getAverage (List<float> floatList)
{
    float total = 0;
    foreach (float value in floatList) {
        total += value;
    }
    float average = total / floatList.Count;
    return average;
}

void ApplyMagCoeffs (float[] mag_cal_in, float[] state_in, ref float[] mag_cal_out)
{
    int row, col;
    float[] temp_array = new float[3];
		
//Apply calibration coeffs
        /* mag coeffs */
        for (row = 0; row < 3; row++) {
			temp_array[row] = 0.0f;
			for (col = 0; col < 3; col++) {
				temp_array[row] += state_in[3 * row + col] * (mag_cal_in[col] - state_in[9 + col]);
			}
		}
		for (row = 0; row < 3; row++) {
			mag_cal_out[row] = temp_array[row];
		}
	}
	
    public static bool CheckresetRefFlagFunction()
    {
        if (data_mode == 0)
            return false;
        else
            return true;
    }

    void AddTitleToLog(System.IO.StreamWriter tofile)
    {
        tofile.WriteLine("FW Ver: " + fw_verstring + ";   Demo Ver: " + demo_verstring);
        tofile.WriteLine("MRX,MRY,MRZ,MCX,MCY,MCZ,AX,AY,AZ,GX,GY,GZ,BTs,"
            + "ATime,MTime,PTime,FrTime,GBias,hScl,vScl,JThrsld,AMode,LMode,MOff,AOff,GOff,Rroll,Rref,Cnter,LossL,flterBW,gCnt,"
            + "CalFlg,CovN,StblFltr,Qlty,"
            + "hPos,vPos,Count");
    }

    void AddNTDTitleToLog(System.IO.StreamWriter tofile)
    {
        tofile.WriteLine("API Ver: " + fw_verstring + ";   Demo Ver: " + demo_verstring);
        tofile.WriteLine("MRX,MRY,MRZ,MCX,MCY,MCZ,AX,AY,AZ,GX,GY,GZ,"
            //+ "BTs,"
            //+ "ATime,MTime,PTime,FrTime,GBias,hScl,vScl,JThrsld,AMode,LMode,MOff,AOff,GOff,Rroll,Rref,Cnter,LossL,flterBW,gCnt,"
            //+ "CalFlg,CovN,StblFltr,Qlty,"
            //+ "hPos,vPos,Count"
            );
    }

    void DataLogAandQ(float[] accdata, float[] qdata, float[] hpr, float[] hvPos, byte res)
    {
        int i;


        for (i = 0; i < 4; i++)
        {
            file.Write(qdata[i]);
            file.Write(",");
        }

        for (i = 0; i < 3; i++)
        {
            file.Write(hpr[i]);
            file.Write(",");
        }

        file.WriteLine();
    }

	void DataLogEng(int [] sendata, float[] accdata, float[] qdata, float[] hpr)
	{
		int i;
		
		//"QTime,MX,MY,MZ,MStamp,AX,AY,AZ,AStamp,GX,GY,GZ,GStamp,
		//HI,HI,HI,Qx,Qy,Qz,Qw,H,P,R,Calstatus,CalScore,TranComp,
		//Cnter,Gbias,CC1,CC2
		
		//Sensor Data
		for (i = 0; i < 13; i++) {
			file.Write (sendata[i]);
			file.Write (",");
		}
			
		//HI
        for (i = 0; i < 3; i++)
        {
            file.Write(accdata[i]);
            file.Write(",");
        }
		//Q
        for (i = 0; i < 4; i++)
        {
            file.Write(qdata[i]);
            file.Write(",");
        }
		//HPR
        for (i = 0; i < 3; i++)
        {
            file.Write(hpr[i]);
            file.Write(",");
        }
		
		//Calstatus
        file.Write(statusArr[2]);
        file.Write(",");

        //CalScore
        file.Write(api_output.state[8]);
        file.Write(",");
		
        //TransComp
        file.Write(statusArr[3]);
        file.Write(",");
		
        //Counter
        file.Write(statusArr[0]);
        file.Write(",");
		
        //gBias
        file.Write(statusArr[1]);
        file.Write(",");
		
        //gthr2 //CC1
        file.Write(statusArr[4]);
        file.Write(",");
        //gthr3 //CC2
        file.Write(statusArr[5]);
        file.Write(",");
		
		file.WriteLine ();
	}
	
	void EngLogTitle (System.IO.StreamWriter myfile)
	{
		string log_line1_text = demo_verstring;
		string log_line2_text = "QTime,MX,MY,MZ,Stmp,AX,AY,AZ,Stmp,GX,GY,GZ,Stmp,HI,HI,HI,Qx,Qy,Qz,Qw,H,P,R,CalStatus,CalScore,TransComp,Cnter,Gbias,gthr2,gthr3";
		myfile.WriteLine (log_line1_text);
		myfile.WriteLine (log_line2_text);
	}

	void MarketingLogTitle (System.IO.StreamWriter myfile)
	{
		string log_line1_text = "DefaultP Ver:" + demo_verstring + "\tFW Ver:" + fw_verstring;
		//+ "SMX,SMY,SMZ,SAX,SAY,SAZ,SGX,SGY,SGZ,"
		//+ "PMX,PMY,PMZ,PAX,PAY,PAZ,PGX,PGY,PGZ,"
		//+ "BTs,ATime,MTime,PTime,FrTime,GBias,hScl,vScl,JThrsld,AMode,LMode,MOff,AOff,GOff,Rroll,Rref,Cnter,LossL,flterBW,gCnt,pntrMode,"
		//+ "RX,RY,RZ,"
		string log_line2_text = "MX,MY,MZ,AX,AY,AZ,GX,GY,GZ," + "Qx,Qy,Qz,Qw";
		myfile.WriteLine (log_line1_text);
		myfile.WriteLine (log_line2_text);
		
	}

    void AandQLogTitle(System.IO.StreamWriter myfile)
    {
        string log_line3_text = demo_verstring;
        //string log_line4_text = "Ax,Ay,Az,Qx,Qy,Qz,Qw,H,P,R,hPos,vPos,Res";
        //string log_line4_text = "Ax,Ay,Az,Qx,Qy,Qz,Qw,H,P,R,H-C,P-C,R-C,SI,SI,HI,HI,HI,CalStatus,CalScore";
        string log_line4_text = "Qx,Qy,Qz,Qw,H,P,R";

        myfile.WriteLine(log_line3_text);
        myfile.WriteLine(log_line4_text);
    }

    bool CloseComPort(System.IO.Ports.SerialPort serial)
    {
        if ((serial != null) && (serial.IsOpen))
        {
            serial.DiscardInBuffer();
            serial.Close();
            return true;
        }
        else
            return false;
    }
	
	//Bridge function from other file to call
	public static void ClearSerialPort()
	{	
		ClearComPort(sp);
	}
	
    static bool ClearComPort(System.IO.Ports.SerialPort serial)
    {
		pause(25);
        if ((serial != null) && (serial.IsOpen))
        {
            serial.DiscardInBuffer();
			
			try
			{
				int count = 100;
				while(count-- > 0)
				{
//					string discardstr = serial.ReadLine();
//					Debug.Log("Clear lines inQ: "+discardstr);
					int len = 100;
					byte[] tempbuf = new byte[len];
					int num = serial.Read(tempbuf, 0, len);
					Debug.Log("Clear "+num+" chars inQ");
				}
//				int len = 1000;
//				byte[] tempbuf = new byte[len];
//				int num = serial.Read(tempbuf, 0, 1000);
//				Debug.Log("Clear "+num+" chars inQ");
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			}
			
            return true;
        }
        else
            return false;
    }
	
    public static bool SendCmdToComPort(string cmdtext)
    {
        bool retval;

        if ((sp != null) && (sp.IsOpen))
        {
			sp.DiscardInBuffer();
			
            sp.Write(cmdtext);
            Debug.Log("send: " + cmdtext);
            retval = true;
        }
        else
            retval = false;
        return retval;
    }


	//接收来自于PC端(串口)的数据
    public static bool ReceiveFromComPort(ref string revtext)
    {
        bool retval = false;
		byte[] buffer = new byte[25];
		int numbytes = 0;
        try
        {
            //string buffer = sp.ReadTo("\r");
            //string buffer = sp.ReadLine();
			numbytes = sp.Read(buffer, 0, 25);
            //revtext = String.Copy(buffer);
			revtext = System.Text.Encoding.Default.GetString(buffer);
            Debug.Log("In Bytes: " + buffer.Length + " Receive: " + revtext);
            if (revtext.Contains("fb00"))
                retval = true;
        }
        catch (Exception e)
        {
			Debug.Log(e.Message+", number of bytes: " +numbytes);
            
        }
        finally
        {
        }
        return retval;
    }

    public static void SetDataMode(byte mode)
    {
        data_mode = mode;
        if ((sp != null) && (sp.IsOpen))
		{
			if (mode == 2)
				sp.Write("2");
			else if (mode == 0)
			{
				sp.Write("0");
				ReadFWVersion();
			}
		}
    }

    GUIStyle myStyle;

    void OnGUI()
    {
        //bool removeCoeffTextFocus = true;

        if (Event.current.Equals(Event.KeyboardEvent("H")))
        {
            hideKnobs = !hideKnobs;
            if (hideKnobs)
                showknobMsg = "Press 'H' to Show Data \n";
            else
                showknobMsg = "Press 'H' to Hide Data \n";
        }

        if (Event.current.Equals(Event.KeyboardEvent("L")))
        {

            if (!log_data)
            {
                filename = "log" + DateTime.Now.ToString("MM-dd-yy_HH_mm_ss") + ".csv";
                try
                {
                    file = new System.IO.StreamWriter(filename);
                }
                catch
                {
                    sttMsg = "Fail to open " + filename;
                }

                if (file != null)
                {
                    log_data = true;

                    if (data_mode == 2)
                    {
                        if (engShow)
							EngLogTitle(file);
						else
							AandQLogTitle(file);  
                    }
//                    else
//                        MarketingLogTitle(file);

                    datalogMsg = "LoggingData, Press 'L' to Stop";
                }
                else
                    sttMsg = "Fail to open " + filename;
            }
            else
            {
                if (file != null)
                    file.Close();
                log_data = false;
                datalogMsg = "Press 'L' to Log Data";
                sttMsg = "";
            }
        }
		
		if (Application.loadedLevelName != "SentralDemo")
		{
			if (Event.current.Equals(Event.KeyboardEvent("M")))
			{
				WriteStateFile(api_output.SIHI_out, api_output.gBias);
				if (CloseComPort(sp))
					sttMsg = "Switch Engine...";
				else
					sttMsg = "Close COM failed";
				Application.LoadLevel(0);
			}

		}
		
		//Letter Ouch for O, not Zero "0"
		if (Event.current.Equals(Event.KeyboardEvent("O")))
		{
			Debug.Log("Come Here press O");
			ResetStats();
		}

        if (Event.current.Equals(Event.KeyboardEvent("X")))
        {
            if (file != null)
                file.Close();

            WriteStateFile(api_output.SIHI_out, api_output.gBias);
            if (CloseComPort(sp))
            {
                data_mode = 0;
            }
            Application.Quit();
        }
				
        if (!hideKnobs || engShow)
        {
            string datastr;
            if (data_mode == 1)
            {
                datastr = "Quaternion:\t" + api_output.quaternion[0].ToString("F3") + "\t\t" + api_output.quaternion[1].ToString("F3") + "\t\t" + api_output.quaternion[2].ToString("F3") + "\t\t" + api_output.quaternion[3].ToString("F3")
                   + "\nHPR:\t\t" + api_output.rotation[0].ToString("F3") + "\t\t" + api_output.rotation[1].ToString("F3") + "\t\t" + api_output.rotation[2].ToString("F3")
                   + "\nGravity:\t" + api_output.Gravity[0].ToString("F3") + "\t\t" + api_output.Gravity[1].ToString("F3") + "\t\t" + api_output.Gravity[2].ToString("F3")
                   + "\nLinear Accel:\t" + api_output.Accelerate[0].ToString("F3") + "\t\t" + api_output.Accelerate[1].ToString("F3") + "\t\t" + api_output.Accelerate[2].ToString("F3")
                   //+ "\nMag\t" + sensorData.mag_cal[0].ToString("F0") + "\t" + sensorData.mag_cal[1].ToString("F0") + "\t" + sensorData.mag_cal[2].ToString("F0")
                    //+"\nMag_raw\t" + sensorData.mag_raw[0].ToString("F0") + "\t" + sensorData.mag_raw[1].ToString("F0") + "\t" + sensorData.mag_raw[2].ToString("F0") 
                   //+ "\nAcc\t" + sensorData.acc_raw[0].ToString("F0") + "\t" + sensorData.acc_raw[1].ToString("F0") + "\t" + sensorData.acc_raw[2].ToString("F0")
                    //+ "\nGyr\t" + sensorData.gyro_raw[0].ToString("F0") + "\t" + sensorData.gyro_raw[1].ToString("F0")  + "\t" + sensorData.gyro_raw[2].ToString("F0")
                   //+ "\nGyr\t" + gyr[0].ToString("F0") + "\t" + gyr[1].ToString("F0") + "\t" + gyr[2].ToString("F0")
					;
            }
            else if (data_mode == 2)
            {
                datastr = //"Quternion Directly from BlueBox"
                    "HPR: " + hpr[0].ToString("F1") + "\t\t" + hpr[1].ToString("F1") + "\t\t" + hpr[2].ToString("F1")
                   + "\n    Q: " + q_ay[0].ToString("F3") + "\t" + q_ay[1].ToString("F3") + "\t" + q_ay[2].ToString("F3") + "\t" + q_ay[3].ToString("F3")
                   ;
            }
            else
            {
                datastr = "No Data Output from COM Port";
            }
			
            if (engShow)
            {
                if (data_mode == 1)
                {
                    datastr += "";
                }
                else if (data_mode == 2)
                {
                    datastr += "\nHI:\t" + acc_out[0].ToString("F3") + "\t\t" + acc_out[1].ToString("F3") + "\t\t" + acc_out[2].ToString("F3")
	                   + "\nCalStatus:\t" + statusArr[2].ToString("D2") + "\t\tCalScore:" + api_output.state[8].ToString("F2") 
					   + "\nTransComp:" + statusArr[3].ToString("D2")
	                   +"\nMag:\t" + rawdata[1].ToString("D5") + "\t" + rawdata[2].ToString("D5") + "\t" + rawdata[3].ToString("D5") + "\t" + rawdata[4].ToString("D5")
	                   +"\nAcc:\t" + rawdata[5].ToString("D5") + "\t" + rawdata[6].ToString("D5") + "\t" + rawdata[7].ToString("D5") + "\t" + rawdata[8].ToString("D5")
	                   +"\nGyr:\t" + rawdata[9].ToString("D5") + "\t" + rawdata[10].ToString("D5") + "\t" + rawdata[11].ToString("D5") + "\t" + rawdata[12].ToString("D5")
					   ;
                }

                datastr += ""
                    + "\ndeltaTime:\t" + deltaTime.ToString("F4")// + "\tdT:\t" + dT.ToString("F4") + "\tb1:\t" + b1 + "\tb2:\t" + b2
                    + "\nTotal\t" + stats.total.ToString("D") + "\t" + "Lost\t" + stats.lost.ToString("D") + "\tis " + stats.lostpctge.ToString("F1") + "%\t"
                    + "\nCountDiff:\t" + countdiff.ToString("D") + "\tRepeat\t" + stats.repeat.ToString("D") + "\tis " + stats.repeatpctge.ToString("F1") + "%\t"
                    ;
            }
			
			int screenlength = 120;
			if (engShow)
				screenlength = 170;

            GUI.TextField(new Rect(10, 10, 250, screenlength), datastr);
			
			string instructionstr = "Demo Ver:\t" + demo_verstring;
			if (engShow)
				instructionstr += "\nFW Ver:"+fw_verstring
								+"\nPress 'O' to reset counters"; 
			
			instructionstr += "\nPress 'P' to Set View"
                //+ "\nPress '1' to output RAW"
                //+ "\nPress '2' to output Q"
                //+ "\nPress '0' to stop data output"
                //+ "\nPress 'M' to Point Tracking Engine"
				+ "\n" + datalogMsg
                + "\n" + showknobMsg
                + "\nPress 'X' to Exit\n"
                + sttMsg;
			
            GUI.TextField(new Rect(260, 10, 220, screenlength), instructionstr);
        }
    }

    void OnApplicationQuit()
    {
        WriteStateFile(api_output.SIHI_out, api_output.gBias);
        if ((sp != null) && (sp.IsOpen))
		{
			sp.Write("0");
            CloseComPort(sp);
		}
        Application.Quit();
    }
}
