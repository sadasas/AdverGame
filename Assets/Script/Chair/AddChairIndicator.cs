using AdverGame.Player;
using System;
using UnityEngine;

namespace AdverGame.Chair
{

    public class AddChairIndicator : MonoBehaviour
    {
        [SerializeField] int m_price;
        public Action<AddChairIndicator> OnAddChair;
        public ChairOffset Offset;

        public void OnTouch(GameObject obj)
        {
            if (obj == this.gameObject)
            {
                if (PlayerManager.s_Instance.Data.Coin >= m_price)
                {
                    PlayerManager.s_Instance.IncreaseCoin(-m_price);
                    OnAddChair?.Invoke(this);

                }
            }

        }
    }
}
