using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShapingController;
using UnityEditor;
using System.IO;

namespace ShapingPlayer
{
    public class PlayerImportPhoto : MonoBehaviour
    {


        // Start is called before the first frame update
        void Start()
        {
            bShouldCatchName = false;


        }

        // Update is called once per frame
        void Update()
        {
            if(bShouldCatchName == true)
            {
                string fullPath = "D:\\WorkGround\\AI\\pytorch_face_landmark\\results\\";

                //获取指定路径下面的所有资源文件  
                if (Directory.Exists(fullPath))
                {
                    DirectoryInfo direction = new DirectoryInfo(fullPath);

                    string[] splitblocks = CatchJPGName.Split('\\');
                    if (splitblocks == null)
                        return;
                    string filename = splitblocks[splitblocks.Length - 1];

                    FileInfo[] files = direction.GetFiles(filename, SearchOption.AllDirectories);
                    if(files.Length != 0)
                    {
                        bShouldCatchName = false;
                        player.ApplyData(controller.GetBlankUsableData());
                        controller.ParsePhoto(fullPath + filename + ".txt");
                        player.ImportPhotoData();
                    }
                }
            }
        }

        public void Setup(ShapingControllerCore core, Player character)
        {
            controller = core;

            player = character;

        }

        public void ImportJPG(string filename)
        {
            bShouldCatchName = true;
            CatchJPGName = filename;
            string[] splitblocks = CatchJPGName.Split('\\');
            if (splitblocks == null)
                return;
            string tmpfilename = splitblocks[splitblocks.Length - 1];

            RunCmd("cmd.exe", "/c copy "+ filename + " D:\\WorkGround\\AI\\pytorch_face_landmark\\samples\\12--Group\\" + tmpfilename);
            //RunCmd("PowerShell.exe", "cd D:\\WorkGround\\AI\\pytorch_face_landmark\\");
            RunCmd("PowerShell.exe", "python test_batch_detections.py", "D:\\WorkGround\\AI\\pytorch_face_landmark\\");
        }


        public static void RunCmd(string cmd, string args, string workdir = null)
        {
            string[] res = new string[2];
            var p = CreateCmdProcess(cmd, args, workdir);
            res[0] = p.StandardOutput.ReadToEnd();
            res[1] = p.StandardError.ReadToEnd();
            p.Close();
            //return res;
        }

        public static System.Diagnostics.Process CreateCmdProcess(string cmd, string args, string workdir = null)
        {
            //var en = System.Text.UTF8Encoding.UTF8;
            //if (Application.platform == RuntimePlatform.WindowsEditor)
            //    en = System.Text.Encoding.GetEncoding("gb2312");



            var pStartInfo = new System.Diagnostics.ProcessStartInfo(cmd);
            pStartInfo.Arguments = args;
            pStartInfo.CreateNoWindow = false;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardError = true;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
            pStartInfo.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            if (!string.IsNullOrEmpty(workdir))
                pStartInfo.WorkingDirectory = workdir;
            return System.Diagnostics.Process.Start(pStartInfo);
        
        }

        private GameController gamecontroller;
        private ShapingControllerCore controller;
        private bool bShouldCatchName;
        private string CatchJPGName;
        private Player player;
    }
}