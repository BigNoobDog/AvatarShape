using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ShapingUI;
using ShapingController;
using ShapingPlayer;


public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Shaping Controller
        controller = new ShapingControllerCore();
        SetupShapingController();

        //UIMan
        GameObject o = GameObject.Find("Canvas");
        UIMan = o.GetComponent<UIManager>();
        UIMan.SetController(controller);
        UIMan.Setup();

        //Player
        if (PlayerObject != null)
        {
            player = PlayerObject.GetComponent<Player>();
        }
        player.SetShapingController(controller);
        player.Setup();
        //UIEventMan

        UIEventManager.BindPlayer(player);
        UIEventManager.BindUIMan(UIMan);
        UIEventManager.Init();

        cameraMan = gameObject.AddComponent<CameraController>();
        characterPresentMan = gameObject.AddComponent<CharacterPresentController>();
        characterPresentMan.Setup(PlayerObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetupShapingController()
    {
        string path = "Assets\\AvartarShape\\Shaping\\Config\\";

        controller.Init();
        controller.Setup(path);
    }

    private ShapingControllerCore controller;
    UIManager UIMan;
    public GameObject PlayerObject;

    CameraController cameraMan;
    CharacterPresentController characterPresentMan;
    Player player;
}
