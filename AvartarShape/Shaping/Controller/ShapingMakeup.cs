using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShapingController
{
    public class ShapingMakeup
    {
        public ShapingMakeup()
        {
            Config = new List<ShapingImageConfig>();
            textures = new List<ShapingMaterialTextureItem>();
            colors = new List<ShapingMaterialColorItem>();
            scalas = new List<ShapingMaterialScalaItem>();
            TextureDatas = new List<int>();
            ColorDatas = new List<int>();
            SliderDatas = new List<float>();

            ColorTable = new Dictionary<int, List<ShapingColorTableItem>>();
            TextureTable = new Dictionary<int, List<ShapingTextureTableItem>>();
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

                    if (columnCount != aryLine.Length)
                    {
                        //print("");
                    }

                    ShapingImageConfig configitem = new ShapingImageConfig();

                    int length = aryLine.Length;
                    if (length != columnCount)
                    {
                        continue;
                    }

                    int tmp_int;
                    string tmp_str;
                    float tmp_f;

                    tmp_int = int.Parse(aryLine[1]);
                    configitem.FirstLevelIndex = tmp_int;

                    tmp_str = aryLine[2];
                    configitem.FirstLevelDesc = tmp_str;

                    tmp_int = int.Parse(aryLine[3]);
                    configitem.SecondLevelIndex = tmp_int;

                    tmp_str = aryLine[4];
                    if(tmp_str != "None" && tmp_str != "")
                    {
                        configitem.TextureParamName = tmp_str;
                       
                        tmp_str = aryLine[5];
                        configitem.TextureDesc = tmp_str;

                        tmp_int = int.Parse(aryLine[6]);
                        configitem.TextureIndex = tmp_int;
                    }

                    tmp_str = aryLine[7];
                    if (tmp_str != "None" && tmp_str != "")
                    {
                        configitem.ColorParamName = tmp_str;
                       
                        tmp_str = aryLine[8];
                        configitem.ColorDesc = tmp_str;

                        tmp_int = int.Parse(aryLine[9]);
                        configitem.ColorIndex = tmp_int;

                    }

                    tmp_str = aryLine[10];
                    if (tmp_str != "None" && tmp_str != "")
                    {
                        configitem.Slider1ParamName = tmp_str;
                        
                        tmp_str = aryLine[11];
                        configitem.Slider1Desc = tmp_str;

                        tmp_f = float.Parse(aryLine[12]);
                        configitem.Slider1Limit = tmp_f;

                    }

                    tmp_str = aryLine[13];
                    if(tmp_str != "None" && tmp_str != "")
                    {
                        configitem.Slider2ParamName = tmp_str;
                        
                        tmp_str = aryLine[14];
                        configitem.Slider2Desc = tmp_str;

                        tmp_int = int.Parse(aryLine[15]);
                        configitem.Slider2Limit = tmp_int;

                    }

                    tmp_str = aryLine[16];
                    if(tmp_str != "None" && tmp_str != "")
                    {
                        configitem.Slider3ParamName = tmp_str;

                        tmp_str = aryLine[17];
                        configitem.Slider3Desc = tmp_str;

                        tmp_int = int.Parse(aryLine[18]);
                        configitem.Slider3Limit = tmp_int;

                    }

                    Config.Add(configitem);
                }
            }

            GenerateInterMaterialConfigItems();
        }

        public void SetTextureTable(Dictionary<int, List<ShapingTextureTableItem>> table)
        {
            TextureTable = table;
        }

        public void SetColorTable(Dictionary<int, List<ShapingColorTableItem>> table)
        {
            ColorTable = table;
        }

        public void GenerateInterMaterialConfigItems()
        {
            for(int i = 0; i < Config.Count; i ++)
            {
                ShapingImageConfig config = Config[i];

                if(config.TextureDesc != null && config.TextureDesc != "None" && config.TextureDesc != "")
                {
                    ShapingMaterialTextureItem item = new ShapingMaterialTextureItem();

                    if (config.FirstLevelDesc == "ÑÛ¾¦")
                        item.part = PART.EYE;
                    else
                        item.part = PART.HEAD;
                    item.TableIndex = config.TextureIndex;
                    item.name = config.TextureParamName;
                    textures.Add(item);
                }

                if(config.ColorDesc != null && config.ColorDesc != "None" && config.ColorDesc != "")
                {
                    ShapingMaterialColorItem item = new ShapingMaterialColorItem();
                    if (config.FirstLevelDesc == "ÑÛ¾¦")
                        item.part = PART.EYE;
                    else
                        item.part = PART.HEAD;
                    item.TableIndex = config.ColorIndex;
                    item.name = config.ColorParamName;
                    colors.Add(item);
                }

                if(config.Slider1Desc != null && config.Slider1Desc != "None" && config.Slider1Desc != "")
                {
                    ShapingMaterialScalaItem item = new ShapingMaterialScalaItem();
                    if (config.FirstLevelDesc == "ÑÛ¾¦")
                        item.part = PART.EYE;
                    else
                        item.part = PART.HEAD;
                    item.name = config.Slider1ParamName;
                    item.limit = config.Slider1Limit;
                    scalas.Add(item);
                }

                if (config.Slider2Desc != null && config.Slider2Desc != "None" && config.Slider2Desc != "")
                {
                    ShapingMaterialScalaItem item = new ShapingMaterialScalaItem();
                    if (config.FirstLevelDesc == "ÑÛ¾¦")
                        item.part = PART.EYE;
                    else
                        item.part = PART.HEAD;
                    item.name = config.Slider2ParamName;
                    item.limit = config.Slider2Limit;
                    scalas.Add(item);
                }

                if (config.Slider3Desc != null && config.Slider3Desc != "None" && config.Slider3Desc != "")
                {
                    ShapingMaterialScalaItem item = new ShapingMaterialScalaItem();
                    if (config.FirstLevelDesc == "ÑÛ¾¦")
                        item.part = PART.EYE;
                    else
                        item.part = PART.HEAD;
                    item.name = config.Slider3ParamName;
                    item.limit = config.Slider3Limit;
                    scalas.Add(item);
                }
            }
        }


        public List<ShapingImageConfig> GetImageConfig()
        {
            return Config;
        }
        public ShapingMaterialTextureItem GetTextureConfigItem(int itemindex)
        {
            if (itemindex < textures.Count)
                return textures[itemindex];
            else
                return null;
        }

        public ShapingMaterialColorItem GetColorConfigItem(int itemindex)
        {
            if (itemindex < colors.Count)
                return colors[itemindex];
            else
                return null;
        }

        public ShapingMaterialScalaItem GetScalaConfigItem(int itemindex)
        {
            if (itemindex < colors.Count)
                return scalas[itemindex];
            else
                return null;
        }

        public void ImportData(List<int> textures, List<int> colors, List<float> sliders)
        {
            TextureDatas = textures;
            ColorDatas = colors;
            SliderDatas = sliders;
        }

        public bool ApplyData(ShapingUsableData UsableData)
        {
            //Texture Usable Data
            for(int i = 0; i < TextureDatas.Count; i ++)
            {
                int value = TextureDatas[i];
                ShapingMaterialTextureItem configitem = textures[i];

                int tableid = configitem.TableIndex;
                List<ShapingTextureTableItem> texturetableitem = TextureTable[tableid];

                ShapingMaterialTextureParam newparam = new ShapingMaterialTextureParam();
                newparam.ParamName = configitem.name;
                newparam.Value = texturetableitem[value].Path;

                PART part = configitem.part;
                if(!UsableData.TextureParams.ContainsKey(part))
                {
                    UsableData.TextureParams[part] = new List<ShapingMaterialTextureParam>();
                }

                UsableData.TextureParams[part].Add(newparam);
            }

            //Color Usable Data
            for (int i = 0; i < ColorDatas.Count; i++)
            {
                int value = ColorDatas[i];
                ShapingMaterialColorItem configitem = colors[i];

                int tableid = configitem.TableIndex;
                List<ShapingColorTableItem> colortableitem = ColorTable[tableid];

                ShapingMaterialVectorParam newparam = new ShapingMaterialVectorParam();
                newparam.ParamName = configitem.name;
                newparam.r = colortableitem[value].R_f;
                newparam.g = colortableitem[value].G_f;
                newparam.b = colortableitem[value].B_f;

                PART part = configitem.part;
                if (!UsableData.VectorParams.ContainsKey(part))
                {
                    UsableData.VectorParams[part] = new List<ShapingMaterialVectorParam>();
                }

                UsableData.VectorParams[part].Add(newparam);
            }

            //Scala Usable Data
            for (int i = 0; i < SliderDatas.Count; i++)
            {
                float value = SliderDatas[i];
                ShapingMaterialScalaItem configitem = scalas[i];

                ShapingMaterialScalaParam newparam = new ShapingMaterialScalaParam();
                newparam.Value = GlobalFunAndVar.GetCalculatedValue(value, configitem.limit);

                PART part = configitem.part;
                if (!UsableData.ScalaParams.ContainsKey(part))
                {
                    UsableData.ScalaParams[part] = new List<ShapingMaterialScalaParam>();
                }

                UsableData.ScalaParams[part].Add(newparam);
            }

            return true;
        }
        public string ExportData()
        {
            string ret = "";

            int length = TextureDatas.Count;

            ret += length.ToString();

            for (int i = 0; i < length; i++)
            {
                ret += " " + TextureDatas[i].ToString();
            }

            length = ColorDatas.Count;

            ret += length.ToString();

            for (int i = 0; i < length; i++)
            {
                ret += " " + ColorDatas[i].ToString();
            }

            length = SliderDatas.Count;

            ret += length.ToString();

            for (int i = 0; i < length; i++)
            {
                ret += " " + SliderDatas[i].ToString();
            }

            return ret;
        }

        private List<ShapingImageConfig> Config;
        private List<ShapingMaterialTextureItem> textures;
        private List<ShapingMaterialColorItem> colors;
        private List<ShapingMaterialScalaItem> scalas;
        private List<int> TextureDatas;
        private List<int> ColorDatas;
        private List<float> SliderDatas;

        private Dictionary<int, List<ShapingColorTableItem>> ColorTable;
        private Dictionary<int, List<ShapingTextureTableItem>> TextureTable;
    }
}
