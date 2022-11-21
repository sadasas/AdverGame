using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class DrinksPlate: ItemPlate
    {
        Image m_progressBar;

        [SerializeField] Image m_imageItem;

        private void Start()
        {
            m_progressBar = GetComponent<Image>();
        }

        public override void StartState(Sprite image)
        {
            m_imageItem.gameObject.SetActive(true);
            m_imageItem.sprite = image;
            m_imageItem.preserveAspect = true;
        }
        public override void UpdateState(float progressCooking)
        {
            UpdateProggressBar(progressCooking);
           

            this.IsEmpty = false;
        }
       public override void UpdateProggressBar(float amount)
        {

            m_progressBar.fillAmount = 1 - (amount / TimeCooking);
        }
        public override void FinishState(Sprite image)
        {
           

        }

        public void PutItem()
        {
            if (Item == null || Item.Content == null) return;
            OnPutItem?.Invoke(Item, this);
            m_imageItem.gameObject.SetActive(false);
            m_imageItem.sprite = null;
            IsEmpty = true;
            Item = null;
            UpdateProggressBar(0);

        }
    }
}
