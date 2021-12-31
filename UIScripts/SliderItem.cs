using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ShapingController;

namespace ShapingUI
{
    public class SliderItem : UIBehaviour
    {
        TYPE type;
        int index;
        public Text Desc;

        private Slider Slider;

        private InputField Value;

        private Button Button;

        private string desc;
        public float value;

        protected override void Awake()
        {
            Desc = transform.gameObject.GetComponentInChildren<Text>();
            Slider = transform.gameObject.GetComponentInChildren<Slider>();
            Value = transform.gameObject.GetComponentInChildren<InputField>();
            Button = transform.gameObject.GetComponentInChildren<Button>();

            //Button[] xx = GameObject.Find("Button").GetComponents<Button>();

            Button.onClick.AddListener(Revert);
            Value.onEndEdit.AddListener(OnValueChanged);
            Slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        //protected override void Setup()
        //{
        //    Revert();
        //}

        public void UpdateInfo(TYPE typevalue, int value, string desc)
        {
            type = typevalue;
            index = value;
            Desc.text = desc;
        }

        void Revert()
        {
            value = 0.5f;
            ApplyValue();
        }

        void OnValueChanged(string str)
        {
            float tmp = float.Parse(str);
            if (tmp > 1.0)
                value = 1.0f;
            else if (tmp < 0.0)
                value = 0.0f;
            else
                value = tmp;

            ValueChanged();
            UIApplyValue();
        }

        void UIApplyValue()
        {
            Value.text = value.ToString();
            Slider.SetValueWithoutNotify(value);
        }

        void ApplyValue()
        {
            Value.text = value.ToString();
            Slider.value = value;
        }

        void OnSliderValueChanged(float v)
        {
            value = v;
            UIApplyValue();
            ValueChanged();
        }

        public void UpdateItem()
        {
            value = 0.5f;

            UIApplyValue();
            ValueChanged();
        }

        void ValueChanged()
        {
            UIEventManager.onUpdateSliderValue.Invoke(type, index, value);
        }

        public bool SetShowValue(int sliderindex, float value)
        {
            if (index == sliderindex)
            {
                Value.text = value.ToString();
                Slider.SetValueWithoutNotify(value);
                return true;
            }
            else
                return false;
        }
    }
}