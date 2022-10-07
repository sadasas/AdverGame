using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class ItemPlate : MonoBehaviour
    {
        Image m_progressBar;

        [SerializeField] Image m_imageItem;
        public bool IsEmpty { get; private set; } = true;
        public float ProggressCooking;
        public float TimeCooking;
        public ItemSerializable Item = null;
        public Action<ItemSerializable, ItemPlate> OnPutItem;

        private void Start()
        {
            m_progressBar = GetComponent<Image>();
        }
        private void Update()
        {
            if (!IsEmpty)
            {
                UpdateProggressBar(ProggressCooking, TimeCooking);
            }
        }

        public void Cooking(Sprite image)
        {
            m_imageItem.sprite = image;
            m_imageItem.preserveAspect = true;
            IsEmpty = false;
        }
        public void UpdateProggressBar(float amount, float TimeCooking)
        {

            m_progressBar.fillAmount = 1 - (amount / TimeCooking);
        }
        public void PutItem()
        {
            if (Item == null || Item.Content == null) return;
            OnPutItem?.Invoke(Item, this);

            m_imageItem.sprite = null;
            IsEmpty = true;
            Item = null;

        }
    }
}
