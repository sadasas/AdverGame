
using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace AdverGame.UI
{
    public class ConversionHelper
    {
        public static string CoinToRupiah(int coin)
        {
            var digits = Math.Floor(Math.Log10(coin));

           
            var multipleValue = math.floor(digits / 3);

            if (multipleValue == 1)
            {
                var length = (int)digits - 2;
                return coin.ToString().Substring(0, length) + "K";
            }

            else
            {
                if (multipleValue >= 2)
                {
                    var length = (int)digits - 5;
                    return coin.ToString().Substring(0, length) + "M";
                }

                return coin.ToString();
            }
        }
    }
    public class CoinHUDHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI m_coin;
        [SerializeField] TextMeshProUGUI m_increment;



        public void UpdateCoin(int coin, int increment)
        {

            m_coin.text = ConversionHelper.CoinToRupiah(coin);
            StartCoroutine(IncrementNotif(increment));
        }


        IEnumerator IncrementNotif(int increment)
        {

            if (increment == 0) yield break;
            var simbol = increment > 0 ? "+ " : "- ";
            m_increment.text = simbol + Mathf.Abs(increment);
            m_increment.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            m_increment.transform.parent.gameObject.SetActive(false);

        }

    }
}
