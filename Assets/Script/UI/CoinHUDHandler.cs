
using System.Collections;
using TMPro;
using UnityEngine;

namespace AdverGame.UI
{
    public class CoinHUDHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_coin;
        [SerializeField] TextMeshProUGUI m_increment;



        public void UpdateCoin(int coin, int increment)
        {

            m_coin.text = coin.ToString();
            StartCoroutine(IncrementNotif(increment));
        }


        IEnumerator IncrementNotif(int increment)
        {
            if (increment == 0) yield break;
            var simbol = increment > 0 ? "+ " : "- ";
            m_increment.text = simbol + increment;
            m_increment.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            m_increment.transform.parent.gameObject.SetActive(false);

        }

    }
}
