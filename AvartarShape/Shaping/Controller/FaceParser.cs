
using System.Collections.Generic;
using System.IO;
using System.Text;

//68 Labels
namespace ShapingController
{
    public static class _68LandMarkMeaning
    {
        public static int ChinPoint;
        public static int LeftLipPoint;
        public static int RightLipPoint;
        public static int LeftEyeLeftPoint;
        public static int LeftEyeRightPoint;
        public static int RightEyeLeftPoint;
        public static int RightEyeRightPoint;

        public static int LeftEyeBrowLeftPoint;
        public static int LeftEyeBrowRightPoint;
        public static int RightEyeBrowLeftPoint;
        public static int RightEyeBrowRightPoint;
        public static int UpNosePoint;
        public static int DownNosePoint;

        public static List<int> NoseBridge;
        public static List<int> Nose;

        public static List<int> UpUpLips;
        public static List<int> UpDownLips;
        public static List<int> DownUpLips;
        public static List<int> DownDownLips;

        public static List<int> LeftUpEyes;
        public static List<int> LeftDownEyes;
        public static List<int> RightUpEyes;
        public static List<int> RightDownEyes;

        public static List<int> LeftEyeBrows;
        public static List<int> RightEyeBrows;

        public static List<int> LeftUpperFicalOutline;
        public static List<int> RightUpperFicalOutline;
        public static List<int> LeftDownFicalOutline;
        public static List<int> RightDownFicalOutline;

        static public void Assign()
        {
            ChinPoint              = 8;
            LeftLipPoint           = 48;
            RightLipPoint          = 54;
            LeftEyeLeftPoint       = 36;
            LeftEyeRightPoint      = 39;
            RightEyeLeftPoint      = 42;
            RightEyeRightPoint     = 45;

            LeftEyeBrowLeftPoint   = 39;
            LeftEyeBrowRightPoint  = 36;
            RightEyeBrowLeftPoint  = 22;
            RightEyeBrowRightPoint = 26;
            UpNosePoint            = 27;
            DownNosePoint          = 33;

            NoseBridge             = new List<int>() { 27, 28, 29, 30};

            Nose                   = new List<int>() { 31, 32, 33, 34, 35};
           
            UpUpLips               = new List<int>() { 48, 49, 50, 51, 52, 53, 54};
            UpDownLips             = new List<int>() { 60, 61, 62, 63, 64, 65 };
            DownUpLips             = new List<int>() { 65, 66, 67};
            DownDownLips           = new List<int>() { 55, 56, 57, 58, 59 };

            LeftUpEyes             = new List<int>() { 36, 37, 38, 39 };
            LeftDownEyes           = new List<int>() { 40, 41 };

            RightUpEyes            = new List<int>() { 42, 43, 44, 45 };
            RightDownEyes          = new List<int>() { 46, 47 };

            LeftEyeBrows           = new List<int>() { 17, 18, 19, 20, 21};
            RightEyeBrows          = new List<int>() { 22, 23, 24, 25, 26};

            LeftUpperFicalOutline  = new List<int>() { 0, 1, 2, 3};
            RightUpperFicalOutline = new List<int>() { 17, 16, 15, 14};
            LeftDownFicalOutline   = new List<int>() { 4, 5, 6, 7 };
            RightDownFicalOutline  = new List<int>() { 13, 12, 11, 10};
        }
    }

    public class FaceInfo
    {
        //public enum EYEBROWSHPE
        //{

        //}
        public float OverWidth { get; set; }
        public float OverHeight { get; set; }

        public float fatness { get; set; }

        public float FaceOutlineWidth { get; set; }
        public float FaceOutlineHeight { get; set; }

        public float FaceUpperDownRatio { get; set; }
        public float UpperFaceLongness { get; set; }
        public float UpperFaceWidth { get; set; }
        public float DownFaceWidth { get; set; }
        public float DownFaceLongness { get; set; }
        public float ChinSharpness { get; set; }
        public float compacting { get; set; }
        public float DistInterEyes { get; set; }
        public float DistInterEyebrows { get; set; }
        public float DistBetwEyesAndNose { get; set; }
        public float DistBetweenNoseAndMouth { get; set; }
        public float EyesNarrowness { get; set; }
        public float EyesLongness { get; set; }
        public float EyebrowLongness { get; set; }

        public float NosePostion { get; set; }
        public float NoseSize { get; set; }
        public float EyeBrowAngle { get; set; }
        public float EyeBrowInAngle { get; set; }
        public float EyeBrowOutAngle { get; set; }
        public float EyeInAngle1 { get; set; }
        public float EyeInAngle2 { get; set; }

        public float EyeOutAngle1 { get; set; }
        public float EyeOutAngle2 { get; set; }

        public float EyeAngle { get; set; }
        public float UpperLipsThickness { get; set; }
        public float DownLipsThickness { get; set; }
        public float LipsLongness { get; set; }

        public void SetStandard()
        {
            OverWidth = 0.7219371f;
            OverHeight = 0.9668486f;

            fatness = 0.75f;

            FaceOutlineWidth = 1.0f;
            FaceOutlineHeight = 1.0f;

            FaceUpperDownRatio = 1.398f;
            UpperFaceLongness = 0.4056556f;
            UpperFaceWidth = 1.0f;
            DownFaceWidth = 1.0f;
            DownFaceLongness = 0.35353f;
            ChinSharpness = 0.648f;

            compacting = 4.4f;
            DistInterEyes = 0.2647999f;
            DistInterEyebrows = 0.1552234f;

            DistBetwEyesAndNose = 0.336f;
            DistBetweenNoseAndMouth = 1.0f;

            EyesNarrowness = 0.193f;
            EyesLongness = 0.197f;
            EyebrowLongness = 1.0f;
            NoseSize = 0.061f;

            EyeBrowAngle = 1.0f;
            EyeBrowInAngle = 0.072f;
            EyeBrowOutAngle = 0.26f;
            EyeInAngle1 = -0.076f;
            EyeInAngle2 = 0.1086f;
            EyeOutAngle1 = 0.2188f;
            EyeOutAngle2 = -0.291f;
            EyeAngle = 0.2598f;

            UpperLipsThickness = 0.0328f;
            DownLipsThickness = 0.0606f;
            LipsLongness = 0.34f;
            //NosePosition = 1.0f;
            
        }
    }
    public class FaceParser
    {

        public void Setup()
        {
            _68LandMarkMeaning.Assign();

            marks = new List<Vector2d>();
            FaceData = new List<float>();
            info = new FaceInfo();
            StandardInfo = new FaceInfo();
            StandardInfo.SetStandard();
        }

        public bool LoadLandMarkInfo(string filename)
        {
            marks.Clear();
            Encoding encoding = Encoding.UTF8; //Encoding.ASCII;//
            FileStream fs = new FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, encoding);

            string strLine = "";
            string[] aryLine = null;

            int rownum = 0;

            while ((strLine = sr.ReadLine()) != null)
            {
                rownum++;
                aryLine = strLine.Split(',');
                if (aryLine.Length != 2)
                    return false;
                float x = float.Parse(aryLine[0]);
                float y = float.Parse(aryLine[1]);
                Vector2d tmp_v = new Vector2d(x, y);
                marks.Add(tmp_v);
            }

            if (rownum == 68)
                return true;
            else
                return false;
        }

        public void Normalize()
        {
            float minx = 100000.0f;
            float miny = 100000.0f;
            float maxx = 0.0f;
            float maxy = 0.0f;

            foreach (Vector2d item in marks)
            {
                if (minx > item.x)
                {
                    minx = item.x;
                }
                if (miny > item.y)
                {
                    miny = item.y;
                }
                if (maxx < item.x)
                {
                    maxx = item.x;
                }
                if (maxy < item.y)
                {
                    maxy = item.y;
                }
            }

            if (GlobalFunAndVar.FloatEqual(maxx, minx) || GlobalFunAndVar.FloatEqual(maxy, miny))
            {
                return;
            }

            foreach (Vector2d item in marks)
            {
                item.x = (item.x - minx) / (maxx - minx);
                item.y = (item.y - miny) / (maxy - miny);
            }
        }

        public void Parse()
        {
            if (marks == null || marks.Count != 68)
                return;

            ParseFaceOutline();

            ParseNose();

            ParseEyes();

            ParseLips();

            ParseEyeBrows();
        }

        public List<float> GenerateFaceShapingData()
        {
            List<float> data = new List<float>();
            for(int i = 0; i < 50; i ++)
            {
                data.Add(0.5f);
            }

            //整体脸大小
            data[0] = Handle(1.0f / info.compacting, 1.0f / StandardInfo.compacting, 5.0f);
            //整体脸长度
            data[2] = Handle(info.UpperFaceLongness + info.DownFaceLongness, StandardInfo.UpperFaceLongness + StandardInfo.DownFaceLongness, 1.0f);

            //上脸长度
            data[4] = Handle(info.UpperFaceLongness , StandardInfo.UpperFaceLongness, 5.0f);
            
            //上脸宽
            data[3] = Handle((info.OverWidth * info.fatness),  (StandardInfo.OverWidth * StandardInfo.fatness), 3.5f);

            //下脸长
            data[11] = Handle(info.DownFaceLongness, StandardInfo.DownFaceLongness, 5.0f);

            //下脸宽度
            data[6] = Handle(info.fatness, info.fatness, 5.0f);
            data[12] = Handle(info.fatness, info.fatness, 5.0f);

            
            //data[6] = Handle(1.0f / info.ChinSharpness, 1.0f / StandardInfo.ChinSharpness, 5.0f);
            //data[12] = Handle(1.0f / info.ChinSharpness, 1.0f / StandardInfo.ChinSharpness, 5.0f);

            //眼睛间距
            data[29] = Handle(info.DistInterEyes, StandardInfo.DistInterEyes, 5.0f);

            //眼睛长度
            data[31] = Handle(info.EyesNarrowness, StandardInfo.EyesNarrowness, 5.0f);

            //眼睛宽度
            data[32] = Handle(info.EyesLongness, StandardInfo.EyesLongness, 3.0f);

            //眼睛角度
            data[30] = Handle(info.EyeAngle, StandardInfo.EyeAngle, 1.0f);

            //内眼角度1
            data[33] = Handle(info.EyeInAngle1, StandardInfo.EyeInAngle1, 0.5f);

            //内眼角度2
            data[36] = Handle(info.EyeInAngle2, StandardInfo.EyeInAngle2, 2.0f);

            //外眼角度1
            data[35] = Handle(info.EyeOutAngle1, StandardInfo.EyeOutAngle1, 0.5f);

            //外眼角度2
            data[38] = Handle(info.EyeOutAngle2, StandardInfo.EyeOutAngle2, 2.0f);

            //眉毛内角度
            data[19] = Handle(1.0f / info.EyeBrowInAngle, 1.0f / StandardInfo.EyeBrowInAngle, 0.3f);

            //眉毛外角度
            data[20] = Handle(info.EyeBrowOutAngle, StandardInfo.EyeBrowOutAngle, 0.6f);
            //眉毛间距
            //data[]

            //眉毛长度
            //data

            //眉毛粗度
            //data[20] = info.EyebrowLongness

            //眉毛高度
            data[15] = Handle(info.DistInterEyebrows, StandardInfo.DistInterEyebrows, 3);

            //上嘴唇厚度


            //下嘴唇厚度
            data[44] = Handle((info.UpperLipsThickness + info.DownLipsThickness),
                (StandardInfo.UpperLipsThickness + StandardInfo.DownLipsThickness), 2);

            //鼻子位置
            data[22] = Handle(1.0f - info.DistBetwEyesAndNose, 1.0f - StandardInfo.DistBetwEyesAndNose, 5);

            //鼻子大小
            data[23] = Handle(info.NoseSize, StandardInfo.NoseSize, 0.5f);

            return data;
        }

        float Handle(float a, float b, float times)
        {
            float ret = 0.0f;
            ret = a / b - 1.0f;
            ret = ret * times;
            ret += 0.5f;
            if (ret < 0.0f)
                ret = 0.0f;
            if (ret > 1.0f)
            {
                ret = 1.0f;
            }

            return ret;
        }

        private void ParseFaceOutline()
        {
            float foo = 0.0f;
            float bar = 0.0f;
            float baz = 0.0f;

            // OverWidth
            for (int i = 0; i < _68LandMarkMeaning.LeftUpperFicalOutline.Count; i++)
            {
                foo += (marks[_68LandMarkMeaning.RightUpperFicalOutline[i]].x - marks[_68LandMarkMeaning.LeftUpperFicalOutline[i]].x);
            }
            info.OverWidth = foo / _68LandMarkMeaning.LeftUpperFicalOutline.Count;

            //OverHeight
            foo = 0.0f;
            bar = 0.0f;
            foreach (int index in _68LandMarkMeaning.LeftEyeBrows)
            {
                foo += marks[index].y;
            }
            foreach (int index in _68LandMarkMeaning.RightEyeBrows)
            {
                foo += marks[index].y;
            }
            bar = marks[_68LandMarkMeaning.ChinPoint].y;
            info.OverHeight = bar - foo / (_68LandMarkMeaning.LeftEyeBrows.Count + _68LandMarkMeaning.RightEyeBrows.Count);

            info.fatness = info.OverWidth / info.OverHeight;

            //FaceUpperDownRatio
            foo = 0.0f;
            bar = 0.0f;
            foo = marks[_68LandMarkMeaning.LeftUpperFicalOutline[_68LandMarkMeaning.LeftUpperFicalOutline.Count - 1]].y
                - marks[_68LandMarkMeaning.LeftUpperFicalOutline[0]].y;
            bar = marks[_68LandMarkMeaning.RightUpperFicalOutline[_68LandMarkMeaning.RightUpperFicalOutline.Count - 1]].y
                - marks[_68LandMarkMeaning.RightUpperFicalOutline[0]].y;
            foo = (foo + bar) / 2.0f;
            bar = marks[_68LandMarkMeaning.LeftDownFicalOutline[_68LandMarkMeaning.LeftDownFicalOutline.Count - 1]].y
                - marks[_68LandMarkMeaning.LeftDownFicalOutline[0]].y;
            baz = marks[_68LandMarkMeaning.RightDownFicalOutline[_68LandMarkMeaning.RightDownFicalOutline.Count - 1]].y
                - marks[_68LandMarkMeaning.RightDownFicalOutline[0]].y;
            bar = (bar + baz) / 2.0f;
            info.FaceUpperDownRatio = foo / bar;

            //UpperFaceLongness
            foo = 0.0f;
            bar = 0.0f;
            foo = marks[_68LandMarkMeaning.RightUpperFicalOutline[_68LandMarkMeaning.RightUpperFicalOutline.Count - 1]].y
                - marks[_68LandMarkMeaning.RightUpperFicalOutline[0]].y;
            bar = marks[_68LandMarkMeaning.LeftUpperFicalOutline[_68LandMarkMeaning.LeftUpperFicalOutline.Count - 1]].y
                - marks[_68LandMarkMeaning.LeftUpperFicalOutline[0]].y;
            info.UpperFaceLongness = (foo + bar) / 2.0f;

            //DownFaceLongness
            foo = 0.0f;
            bar = 0.0f;
            foo = marks[_68LandMarkMeaning.RightDownFicalOutline[_68LandMarkMeaning.RightDownFicalOutline.Count - 1]].y
                - marks[_68LandMarkMeaning.RightDownFicalOutline[0]].y;
            bar = marks[_68LandMarkMeaning.LeftUpperFicalOutline[_68LandMarkMeaning.LeftUpperFicalOutline.Count - 1]].y
                - marks[_68LandMarkMeaning.LeftUpperFicalOutline[0]].y;
            info.DownFaceLongness = (foo + bar) / 2.0f;

            ////UpperFaceWidth
            //foo = 0.0f;
            //bar = 0.0f;
            //foo = marks[_68LandMarkMeaning.RightUpperFicalOutline[_68LandMarkMeaning.RightUpperFicalOutline.Count - 1]].y
            //    - marks[_68LandMarkMeaning.RightUpperFicalOutline[0]].y;
            //bar = marks[_68LandMarkMeaning.LeftUpperFicalOutline[_68LandMarkMeaning.LeftUpperFicalOutline.Count - 1]].y
            //    - marks[_68LandMarkMeaning.LeftUpperFicalOutline[0]].y;
            //info.UpperFaceLongness = (foo + bar) / 2.0f;

            //Compact = eye / facewidth + mouth / facewidth + nose / facewidth
            foo = 0.0f;
            bar = 0.0f;
            baz = 0.0f;
            //Eye area width
            foo = marks[_68LandMarkMeaning.RightEyeRightPoint].x -
                -marks[_68LandMarkMeaning.LeftEyeLeftPoint].x;
            bar = marks[_68LandMarkMeaning.RightLipPoint].x - marks[_68LandMarkMeaning.LeftLipPoint].x;
            baz = marks[_68LandMarkMeaning.DownNosePoint].y - marks[_68LandMarkMeaning.UpNosePoint].y;

            foo = foo / info.OverWidth;
            bar = foo / info.OverWidth;
            baz = foo / info.OverHeight;
            info.compacting = foo + bar + baz;

            //Chin Sharpness
            baz = 0.0f;
            for (int i = 0; (i + 1) < _68LandMarkMeaning.LeftDownFicalOutline.Count; i++)
            {
                foo = marks[_68LandMarkMeaning.LeftDownFicalOutline[i + 1]].y
                    - marks[_68LandMarkMeaning.LeftDownFicalOutline[i]].y;
                bar = marks[_68LandMarkMeaning.LeftDownFicalOutline[i + 1]].x
                    - marks[_68LandMarkMeaning.LeftDownFicalOutline[i]].x;
                foo = foo / bar;
                bar = (float)System.Math.Atan(foo);
                baz += bar;
            }
            for (int i = 0; (i + 1) < _68LandMarkMeaning.RightDownFicalOutline.Count; i++)
            {
                foo = marks[_68LandMarkMeaning.RightDownFicalOutline[i + 1]].y
                    - marks[_68LandMarkMeaning.RightDownFicalOutline[i]].y;
                bar = marks[_68LandMarkMeaning.RightDownFicalOutline[i + 1]].x
                    - marks[_68LandMarkMeaning.RightDownFicalOutline[i]].x;
                foo = (- foo / bar);
                bar = (float)System.Math.Atan(foo);
                baz += bar;
            }
            baz = baz / (_68LandMarkMeaning.LeftDownFicalOutline.Count
                + _68LandMarkMeaning.RightDownFicalOutline.Count);
            info.ChinSharpness = baz;

        }

        private void ParseNose()
        {
            float foo = 0.0f;
            float bar = 0.0f;
            float baz = 0.0f;

            //Nose Size
            foo = marks[35].x - marks[31].x;
            bar = marks[33].y - marks[35].y;
            baz = marks[33].y - marks[31].y;
            info.NoseSize = (float)System.Math.Sqrt( foo * (bar + baz) / 2.0f);

            foo = 0.0f;
            bar = 0.0f;
            baz = 0.0f;
            foreach(int index in _68LandMarkMeaning.LeftDownEyes)
            {
                foo += marks[index].y;
            }
            foreach(int index in _68LandMarkMeaning.RightDownEyes)
            {
                foo += marks[index].y;
            }
            foo = foo / (_68LandMarkMeaning.LeftDownEyes.Count + _68LandMarkMeaning.RightDownEyes.Count);

            foreach(int index in _68LandMarkMeaning.Nose)
            {
                bar += marks[index].y;
            }
            bar = bar / _68LandMarkMeaning.Nose.Count;

            info.DistBetwEyesAndNose = bar - foo;
            
        }

        private void ParseEyes()
        {
            float foo = 0.0f;
            float bar = 0.0f;
            float baz = 0.0f;

            info.EyesNarrowness = ((marks[40].y + marks[41].y - marks[37].y - marks[38].y) + 
                (marks[47].y + marks[48].y - marks[43].y - marks[44].y))/ 4.0f;
            
            info.EyesLongness = ((marks[39].x - marks[36].x) + (marks[45].x - marks[42].x)) / 2.0f;

            info.DistInterEyes = marks[42].x - marks[39].x;

            //info.DistBetwEyesAndNose

            //EyeAngle
            foo = marks[45].y - marks[42].y;
            bar = marks[45].x - marks[42].x;
            baz = (float)System.Math.Atan(foo / bar);

            foo = marks[36].y - marks[39].y;
            bar = marks[36].x - marks[39].y;
            baz = baz - (float)System.Math.Atan(foo / bar);

            baz = - baz / 2.0f;

            info.EyeAngle = baz;

            foo = 0.0f;
            bar = 0.0f;
            baz = 0.0f;

            //EyeInAngle
            foo = marks[43].y - marks[42].y;
            bar = marks[43].x - marks[42].x;
            baz = (float)System.Math.Atan(foo / bar);

            foo = marks[38].y - marks[39].y;
            bar = marks[38].x - marks[39].y;
            baz = baz - (float)System.Math.Atan(foo / bar);

            baz = -baz / 2.0f;

            info.EyeInAngle1 = baz;

            foo = marks[40].y - marks[39].y;
            bar = marks[40].x - marks[39].x;
            baz = (float)System.Math.Atan(foo / bar);

            foo = marks[47].y - marks[42].y;
            bar = marks[47].x - marks[42].y;
            baz = baz - (float)System.Math.Atan(foo / bar);

            baz = -baz / 2.0f;

            info.EyeInAngle2 = baz;
            //Out Eye Angle
            foo = marks[37].y - marks[36].y;
            bar = marks[37].x - marks[36].x;
            baz = (float)System.Math.Atan(foo / bar);

            foo = marks[44].y - marks[45].y;
            bar = marks[44].x - marks[45].y;
            baz = baz - (float)System.Math.Atan(foo / bar);

            baz = -baz / 2.0f;

            info.EyeOutAngle1 = baz;

            foo = marks[41].y - marks[36].y;
            bar = marks[41].x - marks[36].x;
            baz = (float)System.Math.Atan(foo / bar);

            foo = marks[46].y - marks[45].y;
            bar = marks[46].x - marks[45].y;
            baz = baz - (float)System.Math.Atan(foo / bar);

            baz = -baz / 2.0f;

            info.EyeOutAngle2 = baz;
        }

        private void ParseEyeBrows()
        {
            float foo = 0.0f;
            float bar = 0.0f;
            float baz = 0.0f;
            
            foreach(int index in _68LandMarkMeaning.LeftEyeBrows)
            {
                Vector2d v = marks[index];
                foo += v.y;
            }
            foreach (int index in _68LandMarkMeaning.RightEyeBrows)
            {
                Vector2d v = marks[index];
                foo += v.y;
            }
            foo = foo / (_68LandMarkMeaning.LeftEyeBrows.Count + _68LandMarkMeaning.RightEyeBrows.Count);
            
            foreach (int index in _68LandMarkMeaning.LeftUpEyes)
            {
                Vector2d v = marks[index];
                bar += v.y;
            }

            foreach (int index in _68LandMarkMeaning.RightUpEyes)
            {
                Vector2d v = marks[index];
                bar += v.y;
            }

            bar = bar / (_68LandMarkMeaning.LeftUpEyes.Count + _68LandMarkMeaning.RightUpEyes.Count);

            info.DistInterEyebrows = bar - foo;

            //EyeBrowInAngle
            foo = marks[23].y - marks[22].y;
            bar = marks[23].x - marks[22].x;
            baz = (float)System.Math.Atan(foo / bar);

            foo = marks[20].y - marks[21].y;
            bar = marks[20].x - marks[21].y;
            baz = baz - (float)System.Math.Atan(foo / bar);

            baz = -baz / 2.0f;
            //baz = 1.0f / baz;
            info.EyeBrowInAngle = baz;

            foo = marks[18].y - marks[17].y;
            bar = marks[18].x - marks[17].x;
            baz = (float)System.Math.Atan(foo / bar);

            foo = marks[25].y - marks[26].y;
            bar = marks[25].x - marks[26].y;
            baz = baz - (float)System.Math.Atan(foo / bar);

            baz = -baz / 2.0f;

            
            info.EyeBrowOutAngle = baz;
        }

        private void ParseLips()
        {
            float foo = 0.0f;
            float bar = 0.0f;
            float baz = 0.0f;

            info.LipsLongness = marks[54].x - marks[48].x;

            foreach(int index in _68LandMarkMeaning.DownDownLips)
            {
                foo += marks[index].y;
            }
            foo = foo / _68LandMarkMeaning.DownDownLips.Count;

            foreach(int index in _68LandMarkMeaning.DownUpLips)
            {
                bar += marks[index].y;
            }
            bar = bar / _68LandMarkMeaning.DownUpLips.Count;

            info.DownLipsThickness = foo - bar;

            foo = 0.0f;
            bar = 0.0f;
            foreach(int index in _68LandMarkMeaning.UpDownLips)
            {
                foo += marks[index].y;
            }
            foo = foo / _68LandMarkMeaning.UpDownLips.Count;

            foreach(int index in _68LandMarkMeaning.UpUpLips)
            {
                bar += marks[index].y;
            }
            bar = bar / _68LandMarkMeaning.UpUpLips.Count;

            info.UpperLipsThickness = foo - bar;
        }

        private List<Vector2d> marks;

        public List<float> FaceData;

        private FaceInfo info;

        private FaceInfo StandardInfo;
    }
}
