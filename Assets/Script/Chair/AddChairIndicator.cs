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
            m_priceText.text = ConversionHelper.CoinToRupiah(Price);
        }

        public void UpdateCost(int cost)
        {
            Price = cost;
            m_priceText.text = ConversionHelper.CoinToRupiah(Price);
        }
        public void Buy()
        {


            if (PlayerManager.s_Instance.Data.Coin >= Price && PlayerManager.s_Instance.Data.Level.CurrentLevel.MaxChair > ChairManager.s_Instance.ChairsInstanced)
            {

                PlayerManager.s_Instance.IncreaseCoin(-Price);
                PlayerManager.s_Instance.IncreaseExp(Exp);
                OnAddChair?.Invoke(this);

            }
            else
            {
                if (PlayerManager.s_Instance.Data.Coin < Price)
                    UIManager.s_Instance.ShowNotification("Coin anda tidak cukup untuk membeli set meja kursi");
                else UIManager.s_Instance.ShowNotification("Tingkatkan level experience anda agar bisa membeli set meja kursi");
            }


        }
    }
}
