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



        public void OnPointerDown(PointerEventData eventData)
        {
            OnTouch?.Invoke(this);
        }
    }


}
