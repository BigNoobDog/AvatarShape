using System;
using System.Collections.Generic;


namespace ShapingController
{

    public static class ShapingModel
    {
        public enum MODEL
        {
            F201,
            MODELNUM,
        }

        public static string[] MODELNAME =
        {
            "F201",
            "NULL"
        };

        public static MODEL ModelStr2Enum(string target)
        {
            if ((int)MODEL.MODELNUM != MODELNAME.Length)
                return MODEL.MODELNUM;


            for(int i = 0; i < MODELNAME.Length; i ++)
            {
                if(target == MODELNAME[i])
                {
                    return (MODEL)i;
                }
            }

            return MODEL.MODELNUM;
        }
    }

    public enum TYPE
    {
        FACE = 0,
        BODY,
        HAIR,
        CLOTH,
        MAKEUP,
        PRESET,
        CLOTH_HAIR,
        CLOTH_SHIRT,
        CLOTH_DRESS,
        CLOTH_SHOES,
        TYPENUM
    }



    public enum BONEMASK
    {
        LOCATIONX = 0,
        LOCATIONY = 1,
        LOCATIONZ = 2,
        ROTATIONX = 3,
        ROTATIONY = 4,
        ROTATIONZ = 5,
        SCALEX = 6,
        SCALEY = 7,
        SCALEZ = 8,
        BONEMASKNUM
    }

    public enum PART
    {
        HEAD         = 0,
        BODY         = 1,
        UPPERCLOTH   = 2,
        DOWNCLOTH    = 3,
        SHOE         = 4,
        HAIR         = 5,
        EYE          = 6,
        PART_NUM
    }

    //map to Slider
    public class ShapingSkeletonTransConfig
    {
        public int index;

        public int FirstLevel;

        public int SecondLevel;

        public int ThirdLevel;

        public string FirstLevelDesc;

        public string ThirdLevelDesc;

        public string BoneName;

        public int Mask;

        public float LocationLimit;
        public float RotationLimit;
        public float ScaleLimit;

    }

    //map to image
    public class ShapingSkeletonTrans : System.ICloneable
    {
        public ShapingSkeletonTrans()
        {
            Mask = 0;
            Location = new Vector3d(0, 0, 0);
            Rotation = new Vector3d(0, 0, 0);
            Scale    = new Vector3d(0, 0, 0);
        }

        public string bonename;

        public int Mask;

        public Vector3d Location;

        public Vector3d Rotation;

        public Vector3d Scale;

        public object Clone()
        {
            ShapingSkeletonTrans newTran = new ShapingSkeletonTrans();
            newTran.bonename = this.bonename;
            newTran.Mask = this.Mask;
            newTran.Location = this.Location;
            newTran.Rotation = this.Rotation;
            newTran.Scale = this.Scale;
            return (object)newTran;
        }
    }


    public class ShapingImageConfig
    {
        public int FirstLevelIndex;
        public string FirstLevelDesc;
        public int SecondLevelIndex;

        public int TextureIndex;
        public int ColorIndex;

        public int Mask;

        public float Slider1Limit;
        public float Slider2Limit;
        public float Slider3Limit;

        public string TextureDesc;
        public string ColorDesc;
        public string Slider1Desc;
        public string Slider2Desc;
        public string Slider3Desc;

        public string TextureParamName;
        public string ColorParamName;
        public string Slider1ParamName;
        public string Slider2ParamName;
        public string Slider3ParamName;
    }

    public class ShapingMaterialTextureItem
    {
        public PART part;
        //param name
        public string name;
        public int TableIndex;
    }

    public class ShapingMaterialColorItem
    {
        public PART part;
        public string name;
        public int TableIndex;
    }

    public class ShapingMaterialScalaItem
    {
        public PART part;
        public string name;
        public float limit;
    }

    public class ShapingPresetConfigItem
    {
        public int index;
        public string icon;
        public string path;
        public ShapingModel.MODEL model;
    }

    public class ShapingHairConfigItem
    {
        public int index;
        public string icon;
        public string path;
        //public ShapingModel.MODEL model;
    }

    public class ShapingUsableData
    {
        public List<ShapingSkeletonTrans> FaceBones;

        public List<ShapingSkeletonTrans> BodyBones;

        public Dictionary<PART, List<ShapingMaterialTextureParam>> TextureParams;

        public Dictionary<PART, List<ShapingMaterialScalaParam>>   ScalaParams;

        public Dictionary<PART, List<ShapingMaterialVectorParam>>  VectorParams;
    }

    public class ShapingMaterialTextureParam
    {
        public string ParamName;
        public string Value;
    }

    public class ShapingMaterialScalaParam
    {
        public string ParamName;
        public float Value;
    }

    public class ShapingMaterialVectorParam
    {
        public string ParamName;
        public float r, g, b;
    }

    public class ShapingTextureTableItem
    {
        //public int index;
        public string Path;
        public string ThumbPath;
    }

    public class ShapingColorTableItem
    {
        //public int index;
        public int R, G, B;
        public float R_f, G_f, B_f;
    }

    public static class GlobalFunAndVar
    {
        public static float GetCalculatedValue(float value, float limit)
        {
            return 2.0f * (value - 0.5f) * limit;
        }

        public static string TYPE2Str(TYPE type)
        {
            if (type == TYPE.FACE)
                return "FACE";
            else if (type == TYPE.BODY)
                return "BODY";
            else if (type == TYPE.MAKEUP)
                return "MAKEUP";
            else
                return "Undefined";
        }
        static int GetRandomSeedbyGuid()
        {
            return System.Guid.NewGuid().GetHashCode();
        }

        public static float GetRandomFloat(float max)
        {
            //Random ra = new Random(unchecked((int)DateTime.Now.Millisecond));
            Random ra = new Random(GetRandomSeedbyGuid());
            int i_tmp = ra.Next(10000);
            float ret = ((float)i_tmp / (float)10000) * max;
            return ret;
        }

        public static bool FloatEqual(float x, float y)
        {
            if (x - y > 0.001)
                return false;
            if (y - x > 0.001)
                return false;
            return true;
        }
    }
    public class FileNames
    {
        public static string DefaultFaceConfig = "Face.csv";
        public static string DefaultBodyConfig = "Body.csv";
        public static string DefaultMakeupConfig = "Makeup.csv";
        public static string DefaultClothConfig = "Cloth.csv";
        public static string DefaultHairConfig = "Hair.csv";
        public static string DefaultImportFile = "Face1.dat";
        public static string DefaultExportFile = "FaceCustomed1.dat";
        public static string DefaultPresetConfig = "Preset.csv";

        public static string DefaultTextureTable = "Texture.csv";
        public static string DefaultColorTable = "Color.csv";

        public static string DefaultPath = "Assets\\AvartarShape\\Shaping\\Config\\";
    }

}
