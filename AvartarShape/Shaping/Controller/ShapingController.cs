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

        private ShapingPreset  preset;

        private Dictionary<int, List<ShapingColorTableItem>> ColorTable;
        private Dictionary<int, List<ShapingTextureTableItem>> TextureTable;

        private FaceParser faceparser;

        public void Init()
        {
            face    = new ShapingFace();
            body    = new ShapingBody();
            makeup  = new ShapingMakeup();
            hair    = new ShapingHair();
            cloth   = new ShapingCloth();
            sockets = new ShapingSockets();
            preset  = new ShapingPreset();

            faceparser = new FaceParser();

            ColorTable = new Dictionary<int, List<ShapingColorTableItem>>();
            TextureTable = new Dictionary<int, List<ShapingTextureTableItem>>();
        }

        public void Setup(string faceconfig, string bodyconfig, string hairconfig, string makeupconfig, string clothconfig, string socketsconfig, string presetconfig,
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
            preset.LoadConfig(presetconfig);


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
                    if (length != columnCount || length < 4)
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
            faceparser.Setup();

            LoadTextureConfig(path + FileNames.DefaultTextureTable);
            LoadColorConfig(path + FileNames.DefaultColorTable);

            face.LoadConfig(path + FileNames.DefaultFaceConfig);
            body.LoadConfig(path + FileNames.DefaultBodyConfig);

            makeup.LoadConfig(path + FileNames.DefaultMakeupConfig);
            makeup.SetTextureTable(TextureTable);
            makeup.SetColorTable(ColorTable);

            preset.LoadConfig(path + FileNames.DefaultPresetConfig);
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
            if (0 >= length)
            {
                return false;
            }

            

            string[] num_strs = lines[0].Split(' ');

            if (num_strs.Length <= 0)
            {
                return false;
            }

            int iter = 0;

            //Face
            int num_length = int.Parse(num_strs[iter++]);

            List<float> tmp_arr = new List<float>();
            for (int j = 0; j < num_length; j++)
            {
                tmp_arr.Add(float.Parse(num_strs[iter ++]));
            }

            face.ImportData(tmp_arr);


            //Body

            num_length = int.Parse(num_strs[iter ++]);
           

            tmp_arr = new List<float>();
            for (int j = 0; j < num_length; j++)
            {
                tmp_arr.Add(float.Parse(num_strs[iter ++]));
            }

            body.ImportData(tmp_arr);


            //Hair

            //Makeup

            List<int> tex_arr = new List<int>(); ;
            List<int> col_arr = new List<int>(); ;
            List<float> sli_arr = new List<float>(); ;

            num_length = int.Parse(num_strs[iter ++]);


            for (int j = 0; j < num_length; j++)
            {
                tex_arr.Add(int.Parse(num_strs[iter ++]));
            }

            num_length = int.Parse(num_strs[iter ++]);

            for (int j = 0; j < num_length; j++)
            {
                col_arr.Add(int.Parse(num_strs[iter ++]));
            }



            num_length = int.Parse(num_strs[iter ++]);

            for (int j = 0; j < num_length; j++)
            {
                sli_arr.Add(float.Parse(num_strs[iter++]));
            }



            makeup.ImportData(tex_arr, col_arr, sli_arr);


            //Cloth

            //Sockets

            return true;
        }

        public void RandomData(TYPE type)
        {
            if (type == TYPE.FACE)
            {
                face.RandomData();
            }
        }

        public void RevertData(TYPE type)
        {
            if (type == TYPE.FACE)
            {
                face.RevertData();
            }
        }

        //Generate the usable data
        public bool ApplyData()
        {
            if(face != null)
            {
                if (!face.ApplyData())
                    return false;
            }

            if (body != null)
            {
                if (!body.ApplyData())
                    return false;
            }

            if (hair != null)
            {
                if (!hair.ApplyData())
                    return false;
            }

            if (makeup != null)
            {
                if (!makeup.ApplyData())
                    return false;
            }

            if (cloth != null)
            {
                if (!cloth.ApplyData())
                    return false;
            }

            if (sockets != null)
            {
                if (!sockets.ApplyData())
                    return false;
            }

            return true;
        }

        public ShapingUsableData GetUsableData()
        {
            ShapingUsableData data = new ShapingUsableData();
            data.FaceBones = face.GetBonesUsableData();
            data.BodyBones = body.GetBonesUsableData();

            data.ScalaParams = makeup.GetUsableScalaParams();
            data.VectorParams = makeup.GetUsableVectorParams();
            data.TextureParams = makeup.GetUsableTextureParams();
            //foreach(<>)

            return data;
        }

        //Useless Function
        //TODO
        public void SetUsableData(ShapingUsableData data)
        {
            if (data == null)
                return;

            //face.SetBonesUsableData(data.FaceBones);
        }

        public ShapingUsableData GetBlankUsableData()
        {
            ShapingUsableData data = new ShapingUsableData();
            data.FaceBones = face.GetBlankBonesUsableData();
            data.BodyBones = body.GetBlankBonesUsableData();

            data.ScalaParams = makeup.GetBlankUsableScalaParams();
            data.VectorParams = makeup.GetBlankUsableVectorParams();
            data.TextureParams = makeup.GetBlankUsableTextureParams();
            //foreach(<>)

            return data;
        }

        //export the usable data to dat file
        public void ExportData(string filename)
        {
            string filePath = filename;
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

        public List<ShapingTextureTableItem> GetTextureGroup(int tableindex)
        {
            return TextureTable[tableindex];
        }

        public List<ShapingPresetConfigItem> GetPresetConfig()
        {
            return preset.GetConfig();
        }

        public List<ShapingSkeletonTrans> SetOneBoneSliderValue(TYPE type, int index, float value)
        {
            List<ShapingSkeletonTrans> trans = new List<ShapingSkeletonTrans>();

            if (type == TYPE.FACE)
            {
                trans = face.SetOneBoneSliderValue(index, value);
            }
            else if (type == TYPE.BODY)
            {
                trans = body.SetOneBoneSliderValue(index, value);
            }
            else if(type == TYPE.MAKEUP)
            {
                makeup.SetMaterialScalaParam(index, value);
            }
                

            return trans;
        }


        public List<ShapingSkeletonTrans> SetOneBoneSliderValue_Pure(TYPE type, int index, float value)
        {
            List<ShapingSkeletonTrans> trans = new List<ShapingSkeletonTrans>();

            if (type == TYPE.FACE)
            {
                trans = face.SetOneBoneSliderValue_Pure(index, value);
            }
            else if (type == TYPE.BODY)
            {
                trans = body.SetOneBoneSliderValue(index, value);
            }
            else if (type == TYPE.MAKEUP)
            {
                makeup.SetMaterialScalaParam(index, value);
            }


            return trans;
        }

        public void ParsePhoto(string filename)
        {
            faceparser.LoadLandMarkInfo(filename);
            faceparser.Normalize();
            faceparser.Parse();
            List<float> tmp_list = faceparser.GenerateFaceShapingData();
            face.ImportData(tmp_list);
            face.ApplyData();
        }


        public ShapingMaterialColorItem GetMaterialColorConfigItem(TYPE type, int itemindex)
        {
            if (type == TYPE.MAKEUP)
            {
                return makeup.GetColorConfigItem(itemindex);
            }
            else if (type == TYPE.CLOTH_HAIR)
            {
                ShapingMaterialColorItem item = new ShapingMaterialColorItem();
                item.name = "_BaseColor";
                item.TableIndex = 1;
                item.part = PART.HAIR;
                return item;
            }
            else if (type == TYPE.CLOTH_DRESS)
            {
                ShapingMaterialColorItem item = new ShapingMaterialColorItem();
                item.name = "_BaseColor";
                item.TableIndex = 1;
                item.part = PART.DOWNCLOTH;
                return item;
            }
            else if (type == TYPE.CLOTH_SHIRT)
            {
                ShapingMaterialColorItem item = new ShapingMaterialColorItem();
                item.name = "_BaseColor";
                item.TableIndex = 1;
                item.part = PART.UPPERCLOTH;
                return item;
            }
            else if (type == TYPE.CLOTH_SHOES)
            {
                ShapingMaterialColorItem item = new ShapingMaterialColorItem();
                item.name = "_BaseColor";
                item.TableIndex = 1;
                item.part = PART.SHOE;
                return item;
            }
            else
                return null;
            
        }

        public ShapingMaterialTextureItem GetMaterialImageConfigItem(TYPE type, int itemindex)
        {
            if (type == TYPE.MAKEUP)
            {
                return makeup.GetTextureConfigItem(itemindex);
            }
            else
            {
                return null;
            }

        }

        public void SetMaterialImageParam(TYPE type, int valueindex, int value)
        {
            if (type == TYPE.MAKEUP)
            {
                makeup.SetMaterialImageParam(valueindex, value);
            }
            else
            {
               
            }

        }

        public void SetMaterialVectorParam(TYPE type, int valueindex, int value)
        {
            if (type == TYPE.MAKEUP)
            {
                makeup.SetMaterialVectorParam(valueindex, value);
            }
            else
            {

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


        //Public Function
        public List<float> GetFaceData()
        {
            return face.GetDatas();
        }
        public List<float> GetBodyData()
        {
            return body.GetDatas();
        }
        public List<float> GetMakeupSliderData()
        {
            return makeup.GetSliderDatas();
        }
        public List<int> GetMakeupTextureData()
        {
            return makeup.GetTextureDatas();
        }
        public List<int> GetMakeupColorData()
        {
            return makeup.GetColorDatas();
        }
    }
}
