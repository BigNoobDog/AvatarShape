using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;

namespace ShapingPlayer
{
    public class PlayerPresetController : MonoBehaviour
    {


        // Start is called before the first frame update
        void Start()
        {



        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Setup(ShapingControllerCore core, Player character)
        {
            controller = core;

            player = character;
        }


        public void SetShapingController(ShapingControllerCore core)
        {
            controller = core;
        }


        public void OnImageValueChangedFromUI(TYPE type, int index, int value, string path)
        {
            if(player != null)
            {
                player.ImportData(path);
            }
        }

        
        
        private ShapingControllerCore controller;
        private Player player;
    }
}