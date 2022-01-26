using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ShapingController;

namespace ShapingUI
{

    public class ModelItem : UIBehaviour
    {
        private TYPE type;
        private Outline outline;
        private Button button;
        private Image image;

        private string iconpath;
        private string texturepath;

        private int groupindex;
        private int itemindex;

        static public OnItemSelectedStateChange onUpdateImageStateValue = new OnItemSelectedStateChange();

        //Base Function
        public void Setup(TYPE typevalue, int groupid, int itemid, string icon_path, string texture_path , ImageGroup group)
        {
            type = typevalue;
            groupindex = groupid;
            itemindex = itemid;

            outline = gameObject.GetComponentInChildren<Outline>();
            button = gameObject.GetComponentInChildren<Button>();

            Image[] images = gameObject.GetComponentsInChildren<Image>();
            foreach(Image i in images)
            {
                if (i.name == "Image")
                {
                    image = i;
                    break;
                }
                    
            }
            

            iconpath = icon_path;
            texturepath = texture_path;

            button.onClick.AddListener(OnButtonClicked);
            SetOutlineSelected(false);
            onUpdateImageStateValue.AddListener(group.UpdateStateEventHandle);

            iconpath = "UI/Textures/" + iconpath;

            
            Texture2D tmpt = Resources.Load(iconpath) as Texture2D;
            Sprite tmps = Sprite.Create(tmpt, new Rect(0, 0, tmpt.width, tmpt.height), Vector2.zero);
            image.sprite = tmps;
        }

        //Public Function
        public int GetIndex()
        {
            return itemindex;
        }

        public void SetOutlineSelected(bool state)
        {
            if (state == true)
            {
                outline.enabled = true;
            }
            else
            {
                outline.enabled = false;
            }
        }

        public void OnButtonClicked()
        {
            UIEventManager.onUpdateImageValue.Invoke(type, groupindex, itemindex, texturepath);
            onUpdateImageStateValue.Invoke(groupindex, itemindex);
        }

        public class OnItemSelectedStateChange : UnityEngine.Events.UnityEvent<int, int> { }
    }
}