using System;
using UnityEngine;
namespace AdverGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        public InputBehaviour InputBehaviour { get; private set; }
        ItemPlayerBehaviour m_itemPlayerBehaviour;


        [Header("INPUT BEHAVIOUR SETTING")]
        [SerializeField] LayerMask m_clickableMask;

        [Header("ITEM PLAYER BEHAVIOUR SETTING")]
        [SerializeField] GameObject m_findItemHUDPrefab;
        [SerializeField] GameObject m_itemAvailableHUDPrefab;
        [SerializeField] GameObject m_itemAvailableButtonPrefab;
        [SerializeField] int m_maxItemGetted;
        [SerializeField] int m_searchItemTime;

      

        private void Start()
        {
            InputBehaviour = new(m_clickableMask);
            m_itemPlayerBehaviour = new(InputBehaviour, this.transform, m_findItemHUDPrefab, m_itemAvailableHUDPrefab, m_itemAvailableButtonPrefab, m_searchItemTime, m_maxItemGetted);
        }

        private void Update()
        {
            InputBehaviour.Update();
        }

        private void OnDisable()
        {
            m_itemPlayerBehaviour.OnDisable();
        }

        private void OnDestroy()
        {
            m_itemPlayerBehaviour.OnDestroy();
        }
       


        public bool IsInstanced()
        {
            if(InputBehaviour == null) return false;
            if(m_itemPlayerBehaviour == null) return false;

            return true;
        }
    }

}
