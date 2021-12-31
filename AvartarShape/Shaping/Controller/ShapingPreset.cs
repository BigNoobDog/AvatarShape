using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;


namespace ShapingController
{

    public class ShapingPreset
    {
        public ShapingPreset()
        {
            Config = new List<ShapingPresetConfigItem>();
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

                    ShapingPresetConfigItem configitem = new ShapingPresetConfigItem();

                    int length = aryLine.Length;
                    if (length != columnCount)
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

                    int Index = int.Parse(aryLine[0]);
                    configitem.index = Index;

                    string icon = aryLine[1];
                    configitem.icon = icon;

                    string path = aryLine[2];
                    configitem.path = path;

                    string ModelName = aryLine[3];
                    configitem.model = ShapingModel.ModelStr2Enum(ModelName);

                    Config.Add(configitem);
                }

            }
        }

        public List<ShapingPresetConfigItem> GetConfig()
        {
            return Config;
        }

        public List<ShapingPresetConfigItem> Config;
    }
}
