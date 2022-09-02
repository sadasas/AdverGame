using System;
using UnityEngine;
namespace AdverGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        InputBehaviour m_inputBehaviour;
        ItemPlayerBehaviour m_itemPlayerBehaviour;


        [Header("INPUT BEHAVIOUR SETTING")]
        [SerializeField] LayerMask m_clickableMask;

        [Header("ITEM PLAYER BEHAVIOUR SETTING")]
        [SerializeField] GameObject m_findItemHUDPrefab;
        [SerializeField] GameObject m_ItemListHUDPrefab;
        [SerializeField] GameObject m_ItemListButtonPrefab;

        public Action<int> OnIncreaseCoin;

        private void Start()
        {
            m_inputBehaviour = new(m_clickableMask);
            m_itemPlayerBehaviour = new(m_inputBehaviour, this.transform, m_findItemHUDPrefab, m_ItemListHUDPrefab, m_ItemListButtonPrefab);
        }

        private void Update()
        {
            m_inputBehaviour.Update();
        }

        private void OnDisable()
        {
            m_itemPlayerBehaviour.OnDisable();
        }

        public void IncreaseCoin(int coin)
        {
            var data = PlayerManager.s_Instance.Data;
            data.Coin += coin;
            OnIncreaseCoin?.Invoke(data.Coin);
            PlayerManager.s_Instance.SaveDataPlayer();
        }

    }

}
