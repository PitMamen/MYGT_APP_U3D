using UnityEngine;
using System;
using System.Collections;

public class BlueBoxParser : MonoBehaviour {

    #region parseData
    // Divides a sentence into individual words
    public static string[] GetWords(string sentence)
    {
        //strip off the begining text
        //sentence = sentence.Substring(0, sentence.IndexOf("*"));
        //now split it up
        return sentence.Split(',');   //分割
    }

    // Divides a sentence into individual words
    public string[] GetBytes(string sentence)
    {
        //strip off the final * + checksum
        sentence = sentence.Substring(0, sentence.IndexOf("\r"));
        //now split it up
        return sentence.Split(' ');
    }
	
	// Interprets a "Sentral data" package
    public static bool ParseSentralData(byte[] inBytes, ref byte[] status, ref float[] rmatrix, ref float[] q_out, ref float[] acc_out, ref float[] hv_pos)
    {		
		int byteIndex = 3;
		int[] temp = new int[3];
		
		//Force it to pass for now
		//status[0] = 300;
		
		for (int i = 0; i < 3; i++) {
			//dataIndex = byteIndex;
			temp [i] = inBytes[byteIndex] + (inBytes[byteIndex + 1] << 8);
			acc_out[i] = 6f * (temp[i] / 32768f - 1f);
			//Debug.Log(acc_out[i]);
			byteIndex += 2;
		}
		
		byteIndex = 9;
		for (int i = 0; i < 4; i++) {
			q_out[i] = (int)inBytes[byteIndex] + ((int)inBytes[byteIndex + 1] << 8);
			q_out[i] = (float)((float)q_out[i] - 32768f) / 32768f;
			
			byteIndex += 2;
		}
		
		byteIndex = 17;
		for (int i = 0; i < 2; i++) {
			hv_pos[i] = (int)inBytes[byteIndex] + ((int)inBytes[byteIndex + 1] << 8);
			hv_pos[i] = (float)((float)hv_pos[i] - 32768f) / 32768f;
			
			byteIndex += 2;
		}
		byteIndex = 20;
		status[0] = inBytes[byteIndex];   //calStatus
		status[1] = inBytes[byteIndex+1]; //transientCompensation
		
		byteIndex = 21;
//		int b1 = inBytes[byteIndex] & 0x1;
//		int b2 = (inBytes[byteIndex] >> 1) & 0x1;
//		
//		//Counter					
//		int counter = inBytes[byteIndex] >> 4;
		
		
        return true;
    }
	
// Sentral Long pkg data format
//	Byte	Description
//	1	$ character. 
//	2	Frame ID (0-255)   incrementing unsigned char
//	3	Packet type value 0x02
//	4	mcalSIHI[9] LSB (float scaled to a 16 bit unsigned int)
//	5	mcalSIHI[9] MSB
//	6	mcalSIHI[10] LSB
//	7	mcalSIHI[10] MSB
//	8	mcalSIHI[11] LSB
//	9	mcalSIHI[11] MSB
//	10	QX LSB (float scaled to a 16 bit unsigned int)
//	11	QX MSB
//	12	QY LSB
//	13	QY MSB
//	14	QZ LSB
//	15	QZ MSB
//	16	QW LSB
//	17	QW MSB
//	18	mcalScore LSB (float scaled to a 16 bit unsigned int)
//	19	mcalScore MSB
//	20	Value 0x00, placeholder for gbias_mode
//	21	mCalStatus(unsigned char)
//	22	transient compensation (unsigned char)
//	23	Value 0x00 dummy checksum bytes
//	24	Value 0x00 dummy checksum bytes
//	25	QTime LSB (Unsigned 16 bit int)
//	26	QTime MSB
//	27-28	MX  (signed 16 bit int)
//	29-30	MY
//	31-32	MZ
//	33-34	MTIme
//	35-36	AX
//	37-38	AY
//	39-40	AZ
//	41-42	ATime
//	43-44	GX
//	45-46	GY
//	47-48	GZ
//	49-50	GTime
//	51	Carriage return character
//	52	Newline character. (End of packet)

	// Interprets a "Sentral data" package
    public static bool ParseSentralLongData(byte[] inBytes, ref byte[] status, ref float[] q_out, ref float[] acc_out, ref float[] hv_pos, ref int[] SentralRaw)
    {		
		int byteIndex = 1;
		int[] temp = new int[3];
		
		//Counter					
		status[0] = Convert.ToByte(inBytes[byteIndex]);
		
		//SIHI used to be Acc
		byteIndex = 3;
		for (int i = 0; i < 3; i++) {
			temp [i] = inBytes[byteIndex] + (inBytes[byteIndex + 1] << 8);
			acc_out[i] = 6f * (temp[i] / 32768f - 1f);
			//Debug.Log(acc_out[i]);
			byteIndex += 2;
		}
		
		//Qs
		byteIndex = 9;
		for (int i = 0; i < 4; i++) {
			q_out[i] = (int)inBytes[byteIndex] + ((int)inBytes[byteIndex + 1] << 8);
			q_out[i] = (float)((float)q_out[i] - 32768f) / 32768f;
			
			byteIndex += 2;
		}
		
		//Cal Score 
		byteIndex = 17;
		hv_pos[0] = (int)inBytes[byteIndex] + ((int)inBytes[byteIndex + 1] << 8);
		hv_pos[0] = (float)((float)hv_pos[0] - 32768f) / 32768f;
			
		byteIndex = 19;
		status[1] = inBytes[byteIndex]; //res gbias
		
		byteIndex = 20;
		status[2] = inBytes[byteIndex];   //calStatus
		status[3] = inBytes[byteIndex+1]; //transientCompensation
		
		byteIndex = 22;
		status[4] = inBytes[byteIndex];   //res cc1
		status[5] = inBytes[byteIndex+1]; //res cc2
		
		//Q-TimeStamp
		byteIndex = 24;
		for (int i = 0; i < 13; i++)
		{
			int val = (int)inBytes[byteIndex] + ((int)inBytes[byteIndex + 1] << 8);
			
			if (i % 4 == 0)
				SentralRaw[i] = val;
			else
				SentralRaw[i] = (short)val;
			byteIndex += 2;
		}
		
        return true;
    }
	
    // Interprets a "Intel orient" sentence   解析数据
    public static bool ParseData(string sentence, ref UInt32 timestamp, ref float[] rmatrix, ref float[] q_out)
    {
        // Divide the sentence into words
        string[] Words = GetWords(sentence);
        /*
        for (int i = 0; i < Words.Length; i++)
        {
            Debug.Log(i + ", " + Words[i]);
        }*/
        
        int index = 2;
        if (Words[index] != "")
        {
            //Words[index] = Words[index].Substring(1);

            timestamp = Convert.ToUInt32(Words[index]);
        }
        index = 3;
        if (Words[index] != "")
        {
            //Words[index] = Words[index].Substring(1);
            for (int i = 0; i < 9; i++)
            {
                rmatrix[i] = Convert.ToSingle(Words[index + i]);
            }
        }

        index = 12;
        if (Words[index] != "")
        {
            //Words[index] = Words[index].Substring(1);
            for (int i = 0; i < 4; i++)
            {
                q_out[i] = Convert.ToSingle(Words[index + i]);
            }
        }
        return true;
    }

    //$APPDAT,A0CC0,00F0,3F50,MFFBF,002B,FDB5,G0014,FFA9,004B,P0000,T0000,EC4A,*0C
    // Interprets a "APPDAT" NMEA sentence
    public bool ParseAPPDAT(string sentence, ref int[] acc, ref int[] mag, ref int[] gyr, ref int AMGcount)
    {
        // Divide the sentence into words
        string[] Words = GetWords(sentence);

        int index = 1;
        if (Words[index] != "")
        {
            Words[index] = Words[index].Substring(1);

            //debugstring = Words[index].Substring(0);
            for (int i = 0; i < 3; i++)
            {
                acc[i] = Convert.ToInt16(Words[index + i], 16) >> 4;
                //acc[i] = (short)Convert.ToInt16("ffff", 16);
            }
        }
        index = 4;
        if (Words[index] != "")
        {
            Words[index] = Words[index].Substring(1);
            for (int i = 0; i < 3; i++)
            {
                mag[i] = Convert.ToInt16(Words[index + i], 16);
            }
        }

        index = 7;
        if (Words[index] != "")
        {
            Words[index] = Words[index].Substring(1);
            for (int i = 0; i < 3; i++)
            {
                gyr[i] = Convert.ToInt16(Words[index + i], 16);
            }
        }

        index = 12;
        if (Words[index] != "")
        {
            //debugstring = Words[index].Substring(0);
            AMGcount = UInt16.Parse(Words[index], System.Globalization.NumberStyles.HexNumber);
        }
        return true;
    }

	
    public static bool Convert2PNIQ(float[] qin, ref float[] qout)
    {
        const float factor = 0.707f;

        //3rd solution
        //qout[0] = factor * qin[0] - factor * qin[1] + factor * qin[2];
        //qout[1] = factor * qin[0] + factor * qin[1] - factor * qin[3];
        //qout[2] = -factor * qin[0] + factor * qin[2] - factor * qin[3];
        //qout[3] = factor * qin[1] + factor * qin[2] + factor * qin[3];

        //2nd solution
        //qout[0] = factor * qin[0] - factor * qin[1];
        //qout[1] = factor * qin[0] + factor * qin[1];
        //qout[2] = factor * qin[2] - factor * qin[3];
        //qout[3] = factor * qin[2] + factor * qin[3];

        //1st solution
        qout[0] = -factor * qin[2] + factor * qin[3];
        qout[1] = factor * qin[2] + factor * qin[3];
        qout[2] = factor * qin[0] - factor * qin[1];
        qout[3] = -factor * qin[0] - factor * qin[1];

        return true;
    }


    // calculated checksum
    public static bool IsValid(string sentence)
    {
        // Compare the characters after the asterisk to the calculation
        return sentence.Substring(sentence.IndexOf("*") + 1) ==
          GetChecksum(sentence);
    }

    // Calculates the checksum for a sentence
    public static string GetChecksum(string sentence)
    {
        // Loop through all chars to get a checksum
        int Checksum = 0;
        foreach (char Character in sentence)
        {
            if (Character == '$')
            {
                // Ignore the dollar sign
            }
            else if (Character == '*')
            {
                // Stop processing before the asterisk
                break;
            }
            else
            {
                // Is this the first value for the checksum?
                if (Checksum == 0)
                {
                    // Yes. Set the checksum to the value
                    Checksum = Convert.ToByte(Character);
                }
                else
                {
                    // No. XOR the checksum with this character's value
                    Checksum = Checksum ^ Convert.ToByte(Character);
                }
            }
        }
        // Return the checksum formatted as a two-character hexadecimal
        return Checksum.ToString("X2");
    }
    #endregion
}
