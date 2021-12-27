using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ShapingController
{
    public class ShapingFace
    {
        public ShapingFace()
        {
            Config = new List<ShapingSkeletonTransConfig>();
            Datas = new List<float>();
            Bones = new List<ShapingSkeletonTrans>();
        }

        public void LoadConfig(string config)
        {
            Encoding encoding = Encoding.BigEndianUnicode; //Encoding.ASCII;//
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
                    
                    if(columnCount != strLine.Length)
                    {
                        //print("");
                    }


                    ShapingSkeletonTransConfig configitem = new ShapingSkeletonTransConfig();
                    
                    int length = aryLine.Length;
                    if(length != columnCount)
                    {
                        continue;
                    }

                    for (int i = 0; i < aryLine.Length; i++)
                    {
                        if (aryLine[i] == "")
                        {
                            aryLine[i] = "0";
                        }
                    }

                    int mask = 0;
                    //0
                    //int Index = int.Parse(aryLine[0]);
                    //configitem.index = Index;
                    //1
                    int SliderID = int.Parse(aryLine[1]);
                    configitem.index = SliderID;
                    if((SliderID + 1) > slidernum)
                    {
                        slidernum = SliderID + 1;
                    }
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
                    configitem.LocationLimit = LocationLimit * 0.01f;

                    //12
                    int rotationXmask = int.Parse(aryLine[12]);
                    mask += (rotationXmask << (int)BONEMASK.ROTATIONX);
                    //13
                    int rotationYmask = int.Parse(aryLine[13]);
                    mask += (rotationYmask << (int)BONEMASK.ROTATIONY);
                    //14
                    int rotationZmask = int.Parse(aryLine[14]);
                    mask += (rotationZmask << (int)BONEMASK.ROTATIONZ);
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

                    configitem.Mask = mask;

                    Config.Add(configitem);
                }
            }


            sr.Close();
            fs.Close();

            for(int i = 0; i < slidernum; i ++)
            {
                Datas.Add(0.5f);
            }
        }

        public void ImportData(List<float> datas)
        {
            Datas = datas;
        }

        public bool ApplyData()
        {
            Bones.Clear();

            for (int i = 0; i < Datas.Count; i++)
            {
                List<ShapingSkeletonTrans> oneDatatrans = SetOneBoneSliderValue_Internal(i, Datas[i]);
                foreach(ShapingSkeletonTrans tran in oneDatatrans)
                {
                    string bonename = tran.bonename;
                    int useablecontainindex = -1;
                    for(int useableindex = 0; useableindex < Bones.Count; useableindex ++)
                    {
                        if(Bones[useableindex].bonename == bonename)
                        {
                            useablecontainindex = useableindex;
                            break;
                        }
                    }

                    if(useablecontainindex == -1)
                    {
                        ShapingSkeletonTrans foo = new ShapingSkeletonTrans();
                        foo.bonename = bonename;
                        Bones.Add(foo);
                        useablecontainindex = Bones.Count - 1;
                    }

                    if(useablecontainindex >= 0 && useablecontainindex < Bones.Count)
                    {
                        Bones[useablecontainindex].Mask |= tran.Mask;
                        Bones[useablecontainindex].Location += tran.Location;
                        Bones[useablecontainindex].Rotation += tran.Rotation;
                        Bones[useablecontainindex].Scale += tran.Scale;
                    }
                }
            }

            return true;
        }

        public string ExportData()
        {
            string ret = "";

            int length = Datas.Count;

            ret += length.ToString();

            for(int i = 0; i < length; i ++)
            {
                ret += " " + Datas[i].ToString();   
            }

            ret += " ";

            return ret;
        }

        //For Editor
        //Base Function
        public List<ShapingSkeletonTransConfig> GetSliderConfig()
        {
            return Config;
        }

        public List<ShapingSkeletonTrans> SetOneBoneSliderValue(int index, float value)
        {
            //For Core
            List<ShapingSkeletonTrans> tmp = new List<ShapingSkeletonTrans>();
            tmp = SetOneBoneSliderValue_Internal(index, value);

            List<ShapingSkeletonTrans> diff = DiffUsableData(tmp);

            Datas[index] = value;
            ApplyData();

            return diff;
        }


        public List<ShapingSkeletonTrans> DiffUsableData(List<ShapingSkeletonTrans> target)
        {
            List<ShapingSkeletonTrans> ret = new List<ShapingSkeletonTrans>();

            foreach(ShapingSkeletonTrans tran in target)
            {
                foreach(ShapingSkeletonTrans bone in Bones)
                {
                    if(bone.bonename == tran.bonename)
                    {
                        ShapingSkeletonTrans newtep = new ShapingSkeletonTrans();
                        Vector3d loc = new Vector3d();

                        int mask = tran.Mask;
                        newtep.Mask |= mask;
                        newtep.bonename = bone.bonename;
                        if ((mask & (int)(1 << (int)BONEMASK.LOCATIONX)) != 0)
                        {
                            newtep.Location.x = tran.Location.x - bone.Location.x;
                        }

                        if ((mask & (int)(1 << (int)BONEMASK.LOCATIONY)) != 0)
                        {
                            newtep.Location.y = tran.Location.y - bone.Location.y;
                        }
                        if ((mask & (int)(1 << (int)BONEMASK.LOCATIONZ)) != 0)
                        {
                            newtep.Location.z = tran.Location.z - bone.Location.z;
                        }

                        if ((mask & (int)(1 << (int)BONEMASK.ROTATIONX)) != 0)
                        {
                            newtep.Rotation.x = tran.Rotation.x - bone.Rotation.x;
                        }

                        if ((mask & (int)(1 << (int)BONEMASK.ROTATIONY)) != 0)
                        {
                            newtep.Rotation.y = tran.Rotation.y - bone.Rotation.y;
                        }

                        if ((mask & (int)(1 << (int)BONEMASK.ROTATIONZ)) != 0)
                        {
                            newtep.Rotation.z = tran.Rotation.z - bone.Rotation.z;
                        }

                        if ((mask & (int)(1 << (int)BONEMASK.SCALEX)) != 0)
                        {
                            newtep.Scale.x = tran.Scale.x - bone.Scale.x;
                        }

                        if ((mask & (int)(1 << (int)BONEMASK.SCALEY)) != 0)
                        {
                            newtep.Scale.y = tran.Scale.y - bone.Scale.y;
                        }

                        if ((mask & (int)(1 << (int)BONEMASK.SCALEZ)) != 0)
                        {
                            newtep.Scale.z = tran.Scale.z - bone.Scale.z;
                        }

                        ret.Add(newtep);
                    }
                }
            }

            return ret;
        }

        public List<ShapingSkeletonTrans> SetOneBoneSliderValue_Internal(int index, float value)
        {
            //For Editor
            List<ShapingSkeletonTrans> trans = new List<ShapingSkeletonTrans>();

            foreach (ShapingSkeletonTransConfig configitem in Config)
            {
                if (configitem.index == index)
                {
                    ShapingSkeletonTrans tran = null;
                    string key = configitem.BoneName;
                    int transindex = 0;
                    for (; transindex < trans.Count; transindex ++)    
                    {
                        if(trans[transindex].bonename == key)
                        {
                            tran = trans[transindex];
                        }
                    }

                    if(tran == null)
                    {
                        tran = new ShapingSkeletonTrans();
                        tran.bonename = key;
                        trans.Add(tran);
                    }

                    int mask = configitem.Mask;
                    tran.Mask |= mask;
                    if ((mask & (int)(1 << (int)BONEMASK.LOCATIONX)) != 0)
                    {
                        tran.Location.x += 2.0f * (value - 0.5f) * configitem.LocationLimit;
                    }

                    if ((mask & (int)(1 << (int)BONEMASK.LOCATIONY)) != 0)
                    {
                        tran.Location.y += 2.0f * (value - 0.5f) * configitem.LocationLimit;
                    }
                    if ((mask & (int)(1 << (int)BONEMASK.LOCATIONZ)) != 0)
                    {
                        tran.Location.z += 2.0f * (value - 0.5f) * configitem.LocationLimit;
                    }

                    if ((mask & (int)(1 << (int)BONEMASK.ROTATIONX)) != 0)
                    {
                        tran.Rotation.x += 2.0f * (value - 0.5f) * configitem.RotationLimit;
                    }

                    if ((mask & (int)(1 << (int)BONEMASK.ROTATIONY)) != 0)
                    {
                        tran.Rotation.y += 2.0f * (value - 0.5f) * configitem.RotationLimit;
                    }

                    if ((mask & (int)(1 << (int)BONEMASK.ROTATIONZ)) != 0)
                    {
                        tran.Rotation.z += 2.0f * (value - 0.5f) * configitem.RotationLimit;
                    }

                    if ((mask & (int)(1 << (int)BONEMASK.SCALEX)) != 0)
                    {
                        tran.Scale.x += 2.0f * (value - 0.5f) * configitem.ScaleLimit;
                    }

                    if ((mask & (int)(1 << (int)BONEMASK.SCALEY)) != 0)
                    {
                        tran.Scale.y += 2.0f * (value - 0.5f) * configitem.ScaleLimit;
                    }

                    if ((mask & (int)(1 << (int)BONEMASK.SCALEZ)) != 0)
                    {
                        tran.Scale.z += 2.0f * (value - 0.5f) * configitem.ScaleLimit;
                    }

                    trans[transindex] = tran;

                }
            }

            return trans;
        }
        public List<ShapingSkeletonTrans> GetBonesUsableData()
        {
            return Bones;
        }



        public List<ShapingSkeletonTransConfig> Config;
        public List<float> Datas;
        private List<ShapingSkeletonTrans> Bones;

        private int slidernum = 0;
    }
}
