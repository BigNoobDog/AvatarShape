using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ShapingPlayer
{
    public static class CameraTrans
    {
        public static Vector3 NearestLoc;
        public static Vector3 MidLoc;
        public static Vector3 FarestLoc;
        public static Vector3 StartLoc;

        public static Vector3 GetLocFromEnum(CameraPos t)
        {
            if (t == CameraPos.FAR)
                return FarestLoc;
            else if (t == CameraPos.MID)
                return MidLoc;
            else if (t == CameraPos.NEAR)
                return NearestLoc;
            else if (t == CameraPos.Start)
                return StartLoc;
            else
                return StartLoc;
        }
        public static void LoadConfig(string filepath)
        {
            Encoding encoding = Encoding.BigEndianUnicode; //Encoding.ASCII;//
            FileStream fs = new FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, encoding);

            string strLine = "";
            string[] aryLine = null;
            string[] tableHead = null;
            int columnCount = 0;
            bool IsFirst = true;
            int rownum = 0;

            while ((strLine = sr.ReadLine()) != null)
            {
                rownum++;
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                }
                else
                {
                    aryLine = strLine.Split(',');

                    if (columnCount != aryLine.Length)
                    {
                        continue;
                    }

                    if (aryLine[0] == "Start")
                    {
                        float locx = float.Parse(aryLine[1]);
                        float locy = float.Parse(aryLine[2]);
                        float locz = float.Parse(aryLine[3]);

                        StartLoc = new Vector3(locx, locy, locz);
                    }

                    if (aryLine[0] == "Start")
                    {
                        float locx = float.Parse(aryLine[1]);
                        float locy = float.Parse(aryLine[2]);
                        float locz = float.Parse(aryLine[3]);

                        StartLoc = new Vector3(locx, locy, locz);
                    }

                    if (aryLine[0] == "Far")
                    {
                        float locx = float.Parse(aryLine[1]);
                        float locy = float.Parse(aryLine[2]);
                        float locz = float.Parse(aryLine[3]);

                        FarestLoc = new Vector3(locx, locy, locz);
                    }

                    if (aryLine[0] == "Mid")
                    {
                        float locx = float.Parse(aryLine[1]);
                        float locy = float.Parse(aryLine[2]);
                        float locz = float.Parse(aryLine[3]);

                        MidLoc = new Vector3(locx, locy, locz);
                    }

                    if (aryLine[0] == "Near")
                    {
                        float locx = float.Parse(aryLine[1]);
                        float locy = float.Parse(aryLine[2]);
                        float locz = float.Parse(aryLine[3]);

                        NearestLoc = new Vector3(locx, locy, locz);
                    }

                }
            }
        }
    }


    public class CameraController : MonoBehaviour
    {

        private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
                                                                //private float totalRun = 1.0f;

        //Private Control data
        private CameraPos LastPos;
        private CameraPos AimPos;

        private float MovingDuration = 1f;
        private float CurrentMovingTime;

        private bool bMoving = false;

        private GameObject Maincamera;

        private void Start()
        {
            lastMouse = Input.mousePosition;

            CameraTrans.LoadConfig("Assets\\AvartarShape\\Shaping\\Config\\CameraConfig.csv");
            Maincamera = GameObject.Find("Main Camera");
            Maincamera.transform.position = CameraTrans.StartLoc;
            LastPos = CameraPos.Start;
            AimPos = CameraPos.Start;
        }

        void Update()
        {
            MouseMiddleInput();

            //Vector3 curLoc = new Vector3();
            if(bMoving)
            {
                CurrentMovingTime += Time.deltaTime;
            }
            else
            {
                return;
            }


            Vector3 pos = Degree2Curve(CameraTrans.GetLocFromEnum(LastPos), CameraTrans.GetLocFromEnum(AimPos), CurrentMovingTime, MovingDuration);
            Maincamera.transform.position = pos;
            if (CurrentMovingTime > MovingDuration)
            {
                CurrentMovingTime = 0;
                bMoving = false;
                LastPos = AimPos;
            }

        }

        private Vector3 GetKeyBoardBaseInput()
        { //returns the basic values, if it's 0 than it's not active.
            Vector3 p_Velocity = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += new Vector3(0, 0, 1);
            }
            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += new Vector3(0, 0, -1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += new Vector3(-1, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += new Vector3(1, 0, 0);
            }
            if (Input.GetKey(KeyCode.Z))
            {
                p_Velocity += new Vector3(0, -1, 0);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                p_Velocity += new Vector3(0, 1, 0);
            }
            return p_Velocity;
        }


        private void MouseMiddleInput()
        {

            if (bMoving == true)
                return;

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                MoveForward();
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                MoveBack();
            }
        }

        public bool MoveTo(CameraPos pos)
        {
            if (bMoving == true)
                return false;

            AimPos = pos;
            bMoving = true;
            return true;
        }

        public void MoveForward()
        {
            if(LastPos == CameraPos.FAR)
            {
                MoveTo(CameraPos.MID);
            }
            else if(LastPos == CameraPos.MID)
            {
                MoveTo(CameraPos.NEAR);
            }
        }

        public void MoveBack()
        {
            if(LastPos == CameraPos.NEAR)
            {
                MoveTo(CameraPos.MID);
            }
            else if(LastPos == CameraPos.MID)
            {
                MoveTo(CameraPos.FAR);
            }
        }

        //Y = 2x - x^2
        //|             *    
        //|         *
        //|     *
        //|   *
        //| *
        //|*
        //|________________________
        public Vector3 Degree2Curve(Vector3 srcpos, Vector3 dstpos, float interval, float duration)
        {
            if (duration < 0.0001 && duration > -0.0001)
                return srcpos;
            float tmp = interval / duration;
            if (tmp > 1.0f)
                tmp = 1.0f;

            return srcpos + (dstpos - srcpos) * (2 * tmp - tmp * tmp);
        }

        //Y = 2x - x^2
        //|                    * 
        //|                 *  
        //|              *
        //|           *
        //|        *
        //|     *
        //|  *
        //|________________________

        public Vector3 LinearCurve(Vector3 srcpos, Vector3 dstpos, float interval, float duration)
        {
            if (duration < 0.0001 && duration > -0.0001)
                return srcpos;
            float tmp = interval / duration;
            if (tmp > 1.0f)
                tmp = 1.0f;

            return srcpos + (dstpos - srcpos) * tmp;
        }
    }
}