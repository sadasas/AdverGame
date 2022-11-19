using System;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class ItemPlate : MonoBehaviour
    {
        Image m_progressBar;

        [SerializeField] Image m_imageItem;
        [SerializeField] Image m_clove;

        [SerializeField] Animator m_cloveAnimController;
        public bool IsEmpty { get; private set; } = true;

        public float TimeCooking;
        public ItemSerializable Item = null;
        public Action<ItemSerializable, ItemPlate> OnPutItem;

        private void Start()
        {
            m_progressBar = GetComponent<Image>();
        }
     

        public void Cooking(float progressCooking)
        {
            UpdateProggressBar(progressCooking, TimeCooking);
            m_cloveAnimController.SetBool("IsCook", true);

            m_imageItem.gameObject.SetActive(false);

            IsEmpty = false;
        }

        public void FinishCooking(Sprite image)
        {
            m_cloveAnimController.SetBool("IsCook", false);
            m_clove.gameObject.SetActive(false);
            m_imageItem.gameObject.SetActive(true);
            m_imageItem.sprite = image;
            m_imageItem.preserveAspect = true;
        }
        public void UpdateProggressBar(float amount, float TimeCooking)
        {

            m_progressBar.fillAmount = 1 - (amount / TimeCooking);
        }
        public void PutItem()
        {
            if (Item == null || Item.Content == null) return;
            OnPutItem?.Invoke(Item, this);
            m_imageItem.gameObject.SetActive(false);
            m_imageItem.sprite = null;
            IsEmpty = true;
            Item = null;
            m_clove.gameObject.SetActive(true);

        }
    }
}
