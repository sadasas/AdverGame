using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class Item : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] TextMeshProUGUI m_stack;
        [SerializeField] Image m_image;
        [SerializeField] Image m_bgSelected;

        [field: SerializeField]
        public TextMeshProUGUI Name { get; private set; }
        public ItemSerializable Content;
        public Action<Item> OnTouch;
        public void UpdateItem(ItemContent content, int stack)
        {
            Name.text = content.Name;
            m_image.sprite = content.Image;
            m_stack.text = stack.ToString();

        }

        private void OnDisable()
        {
            m_bgSelected.gameObject.SetActive(false);
        }
        public void Selected()
        {
            m_bgSelected.gameObject.SetActive(true);
        }

        public void UnSelected()
        {
            m_bgSelected.gameObject.SetActive(false);
        }
        public void OnPointerDown(PointerEventData eventData)
        {

            if (Int32.Parse(m_stack.text) == 0) return;
            OnTouch?.Invoke(this);
            m_bgSelected.gameObject.SetActive(false);
        }
    }


}
