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
        player.Setup(this);
        //UIEventMan

        UIEventManager.BindPlayer(player);
        UIEventManager.BindUIMan(UIMan);
        UIEventManager.BindGameController(this);
        UIEventManager.Init();

        cameraMan = gameObject.AddComponent<CameraController>();
        characterPresentMan = gameObject.AddComponent<CharacterPresentController>();
        characterPresentMan.Setup(PlayerObject);

        camerapos = CameraPos.Start;
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

    public void ClickPlayer()
    {
        if(camerapos == CameraPos.Start)
        {
            if(cameraMan.MoveTo(CameraPos.FAR))
                camerapos = CameraPos.FAR;
        }
    }

    public void Return()
    {
        if(camerapos != CameraPos.Start)
        {
            if(cameraMan.MoveTo(CameraPos.Start))
                camerapos = CameraPos.Start;
        }
    }

    private ShapingControllerCore controller;
    UIManager UIMan;
    public GameObject PlayerObject;

    CameraController cameraMan;

    UVCameraController UVcameraMan;
    CharacterPresentController characterPresentMan;
    Player player;

    CameraPos camerapos;
}
