using AdverGame.Customer;
using AdverGame.Player;
using AdverGame.UI;
using UnityEngine;

namespace AdverGame.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager s_Instance;

        CustomerManager m_customerManager;
        PlayerManager m_playerManager;



        [SerializeField] GameObject m_UIPrefab;


        [SerializeField] GameObject m_playerManagerPrefab;
        [SerializeField] GameObject m_customerManagerPrefab;
        private void Awake()
        {
            if (s_Instance) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_playerManager = Instantiate(m_playerManagerPrefab).GetComponent<PlayerManager>();
            m_customerManager = Instantiate(m_customerManagerPrefab).GetComponent<CustomerManager>();

            var canvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            var ui = Instantiate(m_UIPrefab, canvas).GetComponent<TestUI>();
            m_playerManager.OnIncreaseCoin += ui.UpdateCoin;

        }



    }
}
