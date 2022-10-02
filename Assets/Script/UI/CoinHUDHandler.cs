
using TMPro;
using UnityEngine;

namespace AdverGame.UI
{
    public class CoinHUDHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_coin;



        public void UpdateCoin(int coin)
        {
            m_coin.text = coin.ToString();
        }

    }
}
