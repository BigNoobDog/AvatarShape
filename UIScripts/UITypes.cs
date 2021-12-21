using ShapingController;

namespace ShapingUI
{


    //For Editor
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
        public static float imagemarginwidth = 8;
        public static int imagenumperrow = 5;

        public static float item_margin_width = 10;
    }

}