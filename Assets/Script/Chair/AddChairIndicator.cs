using AdverGame.Player;
using AdverGame.UI;
using System;
using TMPro;
using UnityEngine;

namespace AdverGame.Chair
{

    public class AddChairIndicator : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_priceText;
        public int Price;
        public int Exp;
        public Action<AddChairIndicator> OnAddChair;
        public ChairAnchor Offset;

        private void Start()
        {
            m_priceText.text = Price.ToString();
        }
        public void Buy()
        {


            if (PlayerManager.s_Instance.Data.Coin >= Price)
            {
                PlayerManager.s_Instance.IncreaseCoin(-Price);
                PlayerManager.s_Instance.IncreaseExp(Exp);
                OnAddChair?.Invoke(this);

            }
            else
            {
                UIManager.s_Instance.ShowNotification("Coin anda tidak cukup untuk membeli set meja kursi");
            }


        }
    }
}
