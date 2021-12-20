using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShapingController
{
    public class ShapingBody
    {
        public ShapingBody()
        {
            Config = new List<ShapingSkeletonTransConfig>();
            Datas = new List<float>();
        }

        public void LoadConfig(string config)
        {

            Encoding encoding = Encoding.ASCII; //Encoding.ASCII;//
            FileStream fs = new FileStream(config, System.IO.FileMode.Open, System.IO.FileAccess.Read);
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
                        //print("");
                    }


                    ShapingSkeletonTransConfig configitem = new ShapingSkeletonTransConfig();

                    int length = aryLine.Length;
                    if (length != columnCount)
                    {
                        continue;
                    }

                    for(int i = 0; i < aryLine.Length; i ++)
                    {
                        if(aryLine[i] == "")
                        {
                            aryLine[i] = "0";
                        }
                    }

                    int mask = 0;
                    //0
                    int Index = int.Parse(aryLine[0]);
                    configitem.index = Index;
                    //1
                    int SliderID = int.Parse(aryLine[1]);
                    configitem.index = SliderID;
                    //2
                    int FirLevel = int.Parse(aryLine[2]);
                    configitem.FirstLevel = FirLevel;
                    //3
                    string FirLevelDesc = aryLine[3];
                    configitem.FirstLevelDesc = FirLevelDesc;
                    //4
                    int SecLevel = int.Parse(aryLine[4]);
                    configitem.SecondLevel = SecLevel;
                    //5
                    int ThirdLevel = int.Parse(aryLine[5]);
                    configitem.ThirdLevel = ThirdLevel;
                    //6
                    string ThirdLevelDesc = aryLine[6];
                    configitem.ThirdLevelDesc = ThirdLevelDesc;
                    //7
                    string BoneName = aryLine[7];
                    configitem.BoneName = BoneName;
                    //8
                    int locationXmask = int.Parse(aryLine[8]);
                    mask += (locationXmask << (int)BONEMASK.LOCATIONX);
                    //9
                    int locationYmask = int.Parse(aryLine[9]);
                    mask += (locationYmask << (int)BONEMASK.LOCATIONY);
                    //10
                    int locationZmask = int.Parse(aryLine[10]);
                    mask += (locationZmask << (int)BONEMASK.LOCATIONZ);
                    //11
                    float LocationLimit = float.Parse(aryLine[11]);
                    configitem.LocationLimit = LocationLimit;

                    //12
                    int rotationXmask = int.Parse(aryLine[12]);
                    mask += (rotationXmask << (int)BONEMASK.ROTATIONX);
                    //13
                    int rotationYmask = int.Parse(aryLine[13]);
                    mask += (rotationYmask << (int)BONEMASK.ROTATIONY);
                    //14
                    int rotationZmask = int.Parse(aryLine[14]);
                    mask += (locationZmask << (int)BONEMASK.ROTATIONZ);
                    //15
                    float RotationLimit = float.Parse(aryLine[15]);
                    configitem.RotationLimit = RotationLimit;

                    //16
                    int ScaleXmask = int.Parse(aryLine[16]);
                    mask += (ScaleXmask << (int)BONEMASK.SCALEX);
                    //17
                    int ScaleYmask = int.Parse(aryLine[17]);
                    mask += (ScaleYmask << (int)BONEMASK.SCALEY);
                    //18
                    int ScaleZmask = int.Parse(aryLine[18]);
                    mask += (ScaleZmask << (int)BONEMASK.SCALEZ);
                    //19
                    float ScaleLimit = int.Parse(aryLine[19]);
                    configitem.ScaleLimit = ScaleLimit;


                    Config.Add(configitem);
                }
            }


            sr.Close();
            fs.Close();
        }

        public void ImportData(List<float> datas)
        {
            Datas = datas;
        }

        public bool ApplyData(ShapingUsableData UsableData)
        {
            if (Config.Count < Datas.Count)
                return false;

            UsableData.FaceBones.Clear();

            for (int i = 0; i < Config.Count; i++)
            {
                string key = Config[i].BoneName;

                float value = Datas[i];

                ShapingSkeletonTrans trans;

                if (UsableData.BodyBones.ContainsKey(key) == true)
                {
                    trans = UsableData.BodyBones[key];
                }
                else
                {
                    trans = new ShapingSkeletonTrans();
                }

                int mask = Config[i].Mask;
                if ((mask & (int)(1 << (int)BONEMASK.LOCATIONX)) != 0)
                {
                    trans.Location.x = 2.0f * (value - 0.5f) * Config[i].LocationLimit;
                }

                if ((mask & (int)(1 << (int)BONEMASK.LOCATIONY)) != 0)
                {
                    trans.Location.y = 2.0f * (value - 0.5f) * Config[i].LocationLimit;
                }
                if ((mask & (int)(1 << (int)BONEMASK.LOCATIONZ)) != 0)
                {
                    trans.Location.z = 2.0f * (value - 0.5f) * Config[i].LocationLimit;
                }

                if ((mask & (int)(1 << (int)BONEMASK.ROTATIONX)) != 0)
                {
                    trans.Rotation.x = 2.0f * (value - 0.5f) * Config[i].RotationLimit;
                }

                if ((mask & (int)(1 << (int)BONEMASK.ROTATIONY)) != 0)
                {
                    trans.Rotation.y = 2.0f * (value - 0.5f) * Config[i].RotationLimit;
                }

                if ((mask & (int)(1 << (int)BONEMASK.ROTATIONZ)) != 0)
                {
                    trans.Rotation.z = 2.0f * (value - 0.5f) * Config[i].RotationLimit;
                }

                if ((mask & (int)(1 << (int)BONEMASK.SCALEX)) != 0)
                {
                    trans.Scale.x = 2.0f * (value - 0.5f) * Config[i].ScaleLimit;
                }

                if ((mask & (int)(1 << (int)BONEMASK.SCALEY)) != 0)
                {
                    trans.Scale.y = 2.0f * (value - 0.5f) * Config[i].ScaleLimit;
                }

                if ((mask & (int)(1 << (int)BONEMASK.SCALEZ)) != 0)
                {
                    trans.Scale.z = 2.0f * (value - 0.5f) * Config[i].ScaleLimit;
                }

                UsableData.BodyBones[key] = trans;
            }

            return true;
        }

        public string ExportData()
        {
            string ret = "";

            int length = Datas.Count;

            ret += length.ToString();

            for (int i = 0; i < length; i++)
            {
                ret += " " + Datas[i].ToString();
            }

            return ret;
        }

        //For Editor
        public List<ShapingSkeletonTransConfig> GetSliderConfig()
        {
            return Config;
        }


        public List<ShapingSkeletonTransConfig> Config;
        public List<float> Datas;

    }
}
