using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
namespace ShapingUI
{

    public class SliderScroller : UIBehaviour
    {
        [SerializeField]
        public RectTransform itemPrototype;

        [SerializeField, Range(0, 30)]
        int instantateItemCount = 4;

        [SerializeField, Range(1, 999)]
        private int max = 5;


        public OnItemPositionChange onUpdateItem = new OnItemPositionChange();

        private string DescStr
        {
            get;
            set;
        }

        public Text Desc;

        [System.NonSerialized]
        public LinkedList<RectTransform> itemList = new LinkedList<RectTransform>();

        protected float diffPreFramePosition = 0;

        protected int currentItemNo = 0;

        private RectTransform _rectTransform;
        protected RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private float anchoredPosition
        {
            get
            {
                return -rectTransform.anchoredPosition.y;
            }
        }

        private float _itemScale = -1;
        public float itemScale
        {
            get
            {
                if (itemPrototype != null && _itemScale == -1)
                {
                    float height = rectTransform.rect.height;

                    _itemScale = height / instantateItemCount;
                }
                return _itemScale;
            }
        }

        // Start is called before the first frame update
        protected override void Awake()
        {
            var controllers = GetComponents<MonoBehaviour>()
            .Where(item => item is ScrollerController)
            .Select(item => item as ScrollerController)
            .ToList();

            // create items

            var scrollRect = GetComponentInParent<ScrollRect>();
            scrollRect.vertical = true;
            scrollRect.content = rectTransform;

            itemPrototype.gameObject.SetActive(false);
            float height = max * itemScale;

            for (int i = 0; i < max; i++)
            {

                //var item = GameObject.Instantiate(itemPrototype) as RectTransform;
                //item.SetParent(transform, false);
                //item.name = i.ToString();
                //item.anchoredPosition = new Vector2(0, -itemScale * i);

                //item.sizeDelta = new Vector2(0, -instantateItemCount * itemScale);

                //itemList.AddLast(item);

                //item.gameObject.SetActive(true);

                //item.GetComponent<SliderItem>().UpdateItem(i);

            }

            rectTransform.sizeDelta = new Vector2(0, height - instantateItemCount * itemScale);
        }

        // Update is called once per frame
        void Update()
        {

        }

        [System.Serializable]
        public class OnItemPositionChange : UnityEngine.Events.UnityEvent<int, GameObject> { }
    }
}