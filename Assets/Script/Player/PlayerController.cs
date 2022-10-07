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
        [SerializeField] GameObject m_cookRandomItemHUDPrefab;
        [SerializeField] GameObject m_cookItemHUDPrefab;
        [SerializeField] GameObject m_itemAvailableHUDPrefab;
        [SerializeField] GameObject m_itemAvailableButtonPrefab;
        [SerializeField] int m_maxItemGetted;
        [SerializeField] int m_searchItemTime;
        [SerializeField] GameObject m_cookRandomItemClickable;
        [SerializeField] GameObject m_cookItemClickable;
        [SerializeField] float m_timeCooking;



        private void Start()
        {
            InputBehaviour = new(m_clickableMask);
            m_itemPlayerBehaviour = new(InputBehaviour, m_cookRandomItemClickable, m_cookRandomItemHUDPrefab, m_itemAvailableHUDPrefab, m_itemAvailableButtonPrefab, m_searchItemTime, m_maxItemGetted, this, m_cookItemHUDPrefab, m_cookItemClickable, m_timeCooking);
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
            if (InputBehaviour == null) return false;
            if (m_itemPlayerBehaviour == null) return false;

            return true;
        }
    }

}
