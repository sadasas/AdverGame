using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        public InputBehaviour InputBehaviour { get; private set; }
        public ItemPlayerBehaviour ItemPlayerBehaviour;

      

        [Header("INPUT BEHAVIOUR SETTING")]
        [SerializeField] LayerMask m_clickableMask;

        [Header("ITEM PLAYER BEHAVIOUR SETTING")]
        [SerializeField] GameObject m_cookRandomItemHUDPrefab;
        [SerializeField] GameObject m_cookItemHUDPrefab;
        [SerializeField] GameObject m_itemAvailableHUDPrefab;
        [SerializeField] GameObject m_itemAvailableButtonPrefab;
        [SerializeField] SpriteRenderer m_etalase;
        [SerializeField] Sprite m_etalaseFull, m_etalaseHalf, m_etalaseEmpty;
        [SerializeField] int m_maxItemGetted;
        [SerializeField] int m_searchItemTime;
        [SerializeField] GameObject m_cookRandomItemClickable;
        [SerializeField] GameObject m_cookItemClickable;
        [SerializeField] float m_timeCooking;



        private void Start()
        {
            InputBehaviour = new(m_clickableMask);
            ItemPlayerBehaviour = new(InputBehaviour, m_cookRandomItemClickable, m_cookRandomItemHUDPrefab, m_itemAvailableHUDPrefab, m_itemAvailableButtonPrefab, m_searchItemTime, m_maxItemGetted, this, m_cookItemHUDPrefab, m_cookItemClickable, m_timeCooking, m_etalase,m_etalaseFull,m_etalaseHalf,m_etalaseEmpty);
        }

        private void Update()
        {
            InputBehaviour.Update();
        }

        private void OnDisable()
        {
            ItemPlayerBehaviour.OnDisable();
        }

        private void OnDestroy()
        {
            ItemPlayerBehaviour.OnDestroy();
        }



        public bool IsInstanced()
        {
            if (InputBehaviour == null) return false;
            if (ItemPlayerBehaviour == null) return false;

            return true;
        }
    }

}
