using ShapingController;

namespace ShapingUI
{


    //For Editor
    public enum STEP
    {
        Entrance,
        Main,
        Face,
        Makeup,
        Hair,
        StepNum
    }

    public class ShapingUIEditorSlider
    {
        public TYPE type;
        public int index;
        public int firstlevel;
        public string firstlevelDesc;
        public int thirdlevel;
        public string thirdlevelDesc;
    }

    public class ShapingUIEditorColor
    {
        public TYPE type;
        public int index;
        public int firstlevel;
        public string firstlevelDesc;
        public int thirdlevel;
        public string thirdlevelDesc;
    }


    public static class UISize
    {
        public static float sliderwidth = 0;
        public static float sliderheight = 30;

        public static float imagewidth = 30;
        public static float Presetimagewidth = 80;
        public static float imagemarginwidth = 8;
        public static int imagenumperrow = 5;
        public static int Presetimagenumperrow = 3;

        public static float item_margin_width = 10;

        public static string PresetName = "预设脸";

        public static string HairName = "头发";

        public static string ShirtName = "上衣";
        
        public static string DressName = "裙子";
        
        public static string ShoesName = "鞋子";


    }

}