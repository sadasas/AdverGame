using AdverGame.Player;
using System;
using UnityEngine;

namespace AdverGame.Chair
{

    public class AddChairIndicator : MonoBehaviour
    {
        public int Price;
        public int Exp;
        public Action<AddChairIndicator> OnAddChair;
        public ChairAnchor Offset;

        public void OnTouch(GameObject obj)
        {


            if (obj == this.gameObject)
            {

                if (PlayerManager.s_Instance.Data.Coin >= Price)
                {
                    PlayerManager.s_Instance.IncreaseCoin(-Price);
                    PlayerManager.s_Instance.IncreaseExp(Exp);
                    OnAddChair?.Invoke(this);

                }
            }

        }
    }
}
