using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{

    public class Stove : ItemPlate
    {

        Image m_progressBar;

        [SerializeField] Image m_imageItem;
        [SerializeField] Image m_clove;

        [SerializeField] Animator m_cloveAnimController;
      
        private void Start()
        {
            m_progressBar = GetComponent<Image>();
        }
        public override void StartState(Sprite image)
        {
            m_cloveAnimController.SetBool("IsStartCook", true);
        }

        public override void UpdateState(float progressCooking)
        {
            //UpdateProggressBar(progressCooking, TimeCooking);
            m_cloveAnimController.SetBool("IsCook", true);

            m_imageItem.gameObject.SetActive(false);

          this.IsEmpty = false;
        }

        public override void FinishState(Sprite image)
        {
            m_cloveAnimController.SetBool("IsCook", false);
            m_clove.gameObject.SetActive(false);
            m_imageItem.gameObject.SetActive(true);
            m_imageItem.sprite = image;
            m_imageItem.preserveAspect = true;
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
