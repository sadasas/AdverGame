
using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace AdverGame.UI
{
    public class ConversionHelper
    {
        public static string CoinToRupiah(long coin)
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

                    var a = coin.ToString().ToCharArray();

                    var actualLength = (int)digits - 5;


                    var length = 0;


                    if (actualLength == 1)
                    {
                        if (a[1] != '0') length++;
                        if (a[2] != '0') length++;
                    }
                    else
                    {

                        var cl = math.clamp(3 - actualLength, 0, 3);

                        for (var i = actualLength; i < actualLength + cl; i++)
                        {
                            if (a[i] != '0') length++;
                        }

                    }
                    if (length > 0)
                        return coin.ToString().Substring(0, actualLength) + "," + coin.ToString().Substring(actualLength, length) + "M";

                    return coin.ToString().Substring(0, actualLength) + "M";
                }

                return coin.ToString();
            }
        }
    }
    public class CoinHUDHandler : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI m_coin;
        [SerializeField]
        TextMeshProUGUI m_increment;

        public void UpdateCoin(long coin, int increment)
        {

            m_coin.text = ConversionHelper.CoinToRupiah(coin);
            StartCoroutine(IncrementNotif(increment));
        }

        IEnumerator IncrementNotif(int increment)
        {

            if (increment == 0)
                yield break;
            var simbol = increment > 0 ? "+ " : "- ";
            m_increment.text = simbol + Mathf.Abs(increment);
            m_increment.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            m_increment.transform.parent.gameObject.SetActive(false);
        }
    }
}
