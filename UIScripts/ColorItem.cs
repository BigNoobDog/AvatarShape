using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ShapingController;

namespace ShapingUI
{
    public class ColorItem : UIBehaviour
    {
        private TYPE type;
        private Outline outline;
        private Button button;
        private Image image;
        private Color color;

        private int groupindex;
        private int itemindex;

        static public OnItemSelectedStateChange onUpdateColorStateValue = new OnItemSelectedStateChange();

        //Base Function
        public void Setup(TYPE typevalue, int groupid, int itemid, float r, float g, float b, ColorGroup group)
        {
            type = typevalue;
            groupindex = groupid;
            itemindex = itemid;

            outline = gameObject.GetComponentInChildren<Outline>();
            button = gameObject.GetComponentInChildren<Button>();
            image = gameObject.GetComponentInChildren<Image>();

            color = new Color(r, g, b);
            image.color = color;

            button.onClick.AddListener(OnButtonClicked);
            SetOutlineSelected(false);
            onUpdateColorStateValue.AddListener(group.UpdateState);
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
            UIEventManager.onUpdateColorValue.Invoke(type, groupindex, itemindex, color);
            onUpdateColorStateValue.Invoke(groupindex, itemindex);
        }



       public class OnItemSelectedStateChange : UnityEngine.Events.UnityEvent<int, int> { }
    }
}