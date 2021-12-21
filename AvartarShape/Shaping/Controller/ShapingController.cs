using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShapingController
{
    //three data format
    //config data: load from config file
    //export data: simplified data
    //usable data: transport to the player. the concrete data
    public class ShapingControllerCore
    {
        //Every model should have a identity ID
        private int ModelIndex;

        private ShapingFace    face;
        private ShapingBody    body;
        private ShapingMakeup  makeup;
        private ShapingHair    hair;
        private ShapingCloth   cloth;
        private ShapingSockets sockets;

        private Dictionary<int, List<ShapingColorTableItem>> ColorTable;
        private Dictionary<int, List<ShapingTextureTableItem>> TextureTable;

        private ShapingUsableData UsableData;

        public void Init()
        {
            face    = new ShapingFace();
            body    = new ShapingBody();
            makeup  = new ShapingMakeup();
            hair    = new ShapingHair();
            cloth   = new ShapingCloth();
            sockets = new ShapingSockets();

            ColorTable = new Dictionary<int, List<ShapingColorTableItem>>();
            TextureTable = new Dictionary<int, List<ShapingTextureTableItem>>();
        }

        public void Setup(string faceconfig, string bodyconfig, string hairconfig, string makeupconfig, string clothconfig, string socketsconfig,
            string texturetable, string colortable)
        {
            LoadTextureConfig(texturetable);
            LoadColorConfig(colortable);

            face.LoadConfig(faceconfig);
            body.LoadConfig(bodyconfig);
            hair.LoadConfig(hairconfig);
            cloth.LoadConfig(clothconfig);
            makeup.LoadConfig(makeupconfig);
            makeup.SetTextureTable(TextureTable);
            makeup.SetColorTable(ColorTable);
            sockets.LoadConfig(socketsconfig);


        }

        public void LoadColorConfig(string filename)
        {
            Encoding encoding = Encoding.ASCII; //Encoding.ASCII;//
            FileStream fs = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
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

                    if (columnCount != strLine.Length)
                    {
                        //printLog("");
                    }

                    int length = aryLine.Length;
                    if (length != columnCount || length < 5)
                    {
                        continue;
                    }

                    int key = int.Parse(aryLine[1]);
                    if(ColorTable.ContainsKey(key) == false)
                    {
                        ColorTable.Add(key, new List<ShapingColorTableItem>());
                    }

                    ShapingColorTableItem item = new ShapingColorTableItem();

                    item.R = int.Parse(aryLine[2]);
                    item.G = int.Parse(aryLine[3]);
                    item.B = int.Parse(aryLine[4]);
                    item.R_f = (float)item.R / 255.0f;
                    item.G_f = (float)item.G / 255.0f;
                    item.B_f = (float)item.B / 255.0f;

                    ColorTable[key].Add(item);
                }
            }


            sr.Close();
            fs.Close();
        }
        public void LoadTextureConfig(string filename)
        {
            Encoding encoding = Encoding.ASCII; //Encoding.ASCII;//
            FileStream fs = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
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

                    if (columnCount != strLine.Length)
                    {
                        //printLog("");
                    }

                    int length = aryLine.Length;
                    if (length != columnCount || length < 5)
                    {
                        continue;
                    }

                    int key = int.Parse(aryLine[1]);
                    if (TextureTable.ContainsKey(key) == false)
                    {
                        TextureTable.Add(key, new List<ShapingTextureTableItem>());
                    }

                    ShapingTextureTableItem item = new ShapingTextureTableItem();

                    item.Path = aryLine[2];
                    item.ThumbPath = aryLine[3];

                    TextureTable[key].Add(item);
                }
            }

            sr.Close();
            fs.Close();
        }

        public void Setup(string path)
        {
            LoadTextureConfig(path + FileNames.DefaultTextureTable);
            LoadColorConfig(path + FileNames.DefaultColorTable);

            face.LoadConfig(path + FileNames.DefaultFaceConfig);
            body.LoadConfig(path + FileNames.DefaultBodyConfig);
            makeup.LoadConfig(path + FileNames.DefaultMakeupConfig);
            makeup.SetTextureTable(TextureTable);
            makeup.SetColorTable(ColorTable);
        }

        public void ImportData()
        {
            ImportData(FileNames.DefaultImportFile);
        }

        //import the data file to export data
        public bool ImportData(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);

            int length = lines.Length;

            //Model Index
            if(0 < length)
            {
                try
                {
                    ModelIndex = int.Parse(lines[0]);
                }
                catch
                {

                }
            }

            //Face
            if(1 < length)
            {
                string[] num_strs = lines[1].Split(' ');

                if(num_strs.Length > 0)
                {
                    int num_length = int.Parse(num_strs[0]);

                    if(num_length == (num_strs.Length - 1))
                    {
                        List<float> tmp_arr = new List<float>();
                        for (int j = 1; j < num_length; j ++)
                        {
                            tmp_arr.Add(float.Parse(num_strs[j]));
                        }

                        face.ImportData(tmp_arr);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            //Body
            if (2 < length)
            {
                string[] num_strs = lines[2].Split(' ');

                if (num_strs.Length > 0)
                {
                    int num_length = int.Parse(num_strs[0]);

                    if (num_length == (num_strs.Length - 1))
                    {
                        List<float> tmp_arr = new List<float>();
                        for (int j = 1; j < num_length; j++)
                        {
                            tmp_arr.Add(float.Parse(num_strs[j]));
                        }

                        body.ImportData(tmp_arr);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            //Hair

            //Makeup
            if (6 < length)
            {
                List<int> tex_arr   = new List<int>(); ;
                List<int> col_arr   = new List<int>(); ;
                List<float> sli_arr = new List<float>(); ;
                string[] num_strs = lines[4].Split(' ');

                if (num_strs.Length > 0)
                {
                    int num_length = int.Parse(num_strs[0]);

                    if (num_length == (num_strs.Length - 1))
                    {
                        for (int j = 1; j < num_length; j++)
                        {
                            tex_arr.Add(int.Parse(num_strs[j]));
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                num_strs = lines[5].Split(' ');

                if (num_strs.Length > 0)
                {
                    int num_length = int.Parse(num_strs[0]);

                    if (num_length == (num_strs.Length - 1))
                    {
                        for (int j = 1; j < num_length; j++)
                        {
                            col_arr.Add(int.Parse(num_strs[j]));
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                num_strs = lines[6].Split(' ');

                if (num_strs.Length > 0)
                {
                    int num_length = int.Parse(num_strs[0]);

                    if (num_length == (num_strs.Length - 1))
                    {
                        for (int j = 1; j < num_length; j++)
                        {
                            sli_arr.Add(float.Parse(num_strs[j]));
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                makeup.ImportData(tex_arr, col_arr, sli_arr);
            }

            //Cloth

            //Sockets

            return true;
        }

        //Generate the usable data
        public bool ApplyData()
        {
            if (UsableData == null)
                return false;

            if(face != null)
            {
                if (!face.ApplyData(UsableData))
                    return false;
            }

            if (body != null)
            {
                if (!body.ApplyData(UsableData))
                    return false;
            }

            if (hair != null)
            {
                if (!hair.ApplyData(UsableData))
                    return false;
            }

            if (makeup != null)
            {
                if (!makeup.ApplyData(UsableData))
                    return false;
            }

            if (cloth != null)
            {
                if (!cloth.ApplyData(UsableData))
                    return false;
            }

            if (sockets != null)
            {
                if (!sockets.ApplyData(UsableData))
                    return false;
            }

            return true;
        }

        //export the usable data to dat file
        public void ExportData(string filename)
        {
            string filePath = filename + ".dat";
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            FileStream fs = new FileStream(filePath, FileMode.Create);

            string export_str = "";

            export_str += face.   ExportData();
            export_str += body.   ExportData();
            export_str += hair.   ExportData();
            export_str += makeup. ExportData();
            export_str += cloth.  ExportData();
            export_str += sockets.ExportData();

            byte[] data = System.Text.Encoding.Default.GetBytes(export_str);
            fs.Write(data, 0, data.Length);

            fs.Flush();
            fs.Close();
        }

        public void ExportData()
        {
            ExportData(FileNames.DefaultExportFile);
        }


        //For UI Editor
        public List<ShapingSkeletonTransConfig> GetFaceSliderConfig()
        {
            return face.GetSliderConfig();
        }

        public List<ShapingSkeletonTransConfig> GetBodySliderConfig()
        {
            return body.GetSliderConfig();
        }

        public List<ShapingImageConfig> GetMakeupImageConfig()
        {
            return makeup.GetImageConfig();
        }

        public List<ShapingColorTableItem> GetColorGroup(int tableindex)
        {
            return ColorTable[tableindex];
        }

        public List<ShapingSkeletonTrans> SetOneBoneSliderValue(TYPE type, int index, float value)
        {
            List<ShapingSkeletonTrans> trans;

            if(type == TYPE.FACE)
            {
                trans = face.SetOneBoneSliderValue(index, value);
            }
            else
            {
                trans = null;
            }

            return trans;
        }


        public ShapingMaterialColorItem GetMaterialColorConfigItem(TYPE type, int itemindex)
        {
            if(type == TYPE.MAKEUP)
            {
                return makeup.GetColorConfigItem(itemindex);
            }
            else
            {
                return null;
            }
            
        }

        public ShapingMaterialScalaItem GetMaterialScalaConfigItem(TYPE type, int itemindex)
        {
            if (type == TYPE.MAKEUP)
            {
                return makeup.GetScalaConfigItem(itemindex);
            }
            else
            {
                return null;
            }

        }

    }
}