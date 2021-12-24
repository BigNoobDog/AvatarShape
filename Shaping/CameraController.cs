using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShapingController
{
    public static class CameraTrans
    {
        public static Vector3 NearestLoc = new Vector3(0, 1.45f, -9.2f);
        public static Vector3 MidLoc = new Vector3(0, 1.36f, -9.55f);
        public static Vector3 FarestLoc = new Vector3(0, 1f, -10f);
    }


    public class CameraController : MonoBehaviour
    {

        private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
                                                                //private float totalRun = 1.0f;

        //Private Control data


        // := -1 Far
        // :=  0 Mid
        // :=  1 Near
        private int MouseMid = -1;

        private float MovingTime = 1f;
        
        //-1 back
        // 0 still
        // 1 Forward
        private int MovingState = 0;
        private float CurrentMovingTime;
        private GameObject Maincamera;

        private void Start()
        {
            lastMouse = Input.mousePosition;

            Maincamera = GameObject.Find("Main Camera");
            Maincamera.transform.position = CameraTrans.FarestLoc;
        }

        void Update()
        {

            //Use wasd to control the camera
            //Vector3 LocationOffset = GetKeyBoardBaseInput() * 0.005f;
            //Vector3 loc = camera.transform.position + LocationOffset;
            //camera.transform.position = loc;


            MouseMiddleInput();

            Vector3 curLoc = new Vector3();

            if (MovingState == 1)
            {
                if(MouseMid == 0)
                {
                    curLoc = CameraTrans.FarestLoc + (CameraTrans.MidLoc - CameraTrans.FarestLoc) * (MovingTime- CurrentMovingTime) / MovingTime;
                    Maincamera.transform.position = curLoc;
                }
                else if(MouseMid == 1)
                {
                    curLoc = CameraTrans.MidLoc + (CameraTrans.NearestLoc - CameraTrans.MidLoc) * (MovingTime - CurrentMovingTime) / MovingTime;
                    Maincamera.transform.position = curLoc;
                }
                else
                {

                }
            }
            else if(MovingState == -1)
            {
                if(MouseMid == 0)
                {
                    curLoc = CameraTrans.NearestLoc + (CameraTrans.MidLoc - CameraTrans.NearestLoc) * (MovingTime - CurrentMovingTime) / MovingTime;
                    Maincamera.transform.position = curLoc;
                }
                else
                {
                    curLoc = CameraTrans.MidLoc + (CameraTrans.FarestLoc - CameraTrans.MidLoc) * (MovingTime - CurrentMovingTime) / MovingTime;
                    Maincamera.transform.position = curLoc;
                }
            }


            if (MovingState != 0)
            {
                CurrentMovingTime -= Time.deltaTime;
            }

            if (CurrentMovingTime < 0.0f)
            {
                CurrentMovingTime = MovingTime;
                MovingState = 0;
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


            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (MovingState == 0 && MouseMid < 1)
                {
                    MovingState = 1;
                    MouseMid++;
                    CurrentMovingTime = MovingTime;
                }
                else
                    return;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (MovingState == 0 && MouseMid > -1)
                {
                    MovingState = -1;
                    MouseMid--;
                    CurrentMovingTime = MovingTime;
                }    
                else
                    return;

               
            }
        }
    }
}