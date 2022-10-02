using AdverGame.CameraGame;
using AdverGame.Chair;
using AdverGame.Customer;
using AdverGame.Player;
using AdverGame.UI;
using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    PAUSE,
    RUN,
}
namespace AdverGame.GameManager
{



    public class GameManager : MonoBehaviour
    {
        public static GameManager s_Instance;

        CustomerManager m_customerManager;
        PlayerManager m_playerManager;
        UIManager m_UIManager;
        CoinHUDHandler m_tes;
        ChairManager m_chairManager;
        CameraController m_cameraController;

        [SerializeField] GameObject m_UIPrefab;
        [SerializeField] GameObject m_playerManagerPrefab;
        [SerializeField] GameObject m_customerManagerPrefab;
        [SerializeField] GameObject m_UIManagerPrefab;
        [SerializeField] GameObject m_UTesPrefab;
        [SerializeField] GameObject m_ChairManagerPrefab;
        [SerializeField] GameObject m_PauseMenuManagerPrefab;

        public Action<GameState> OnGameStateChange;
        public GameState CurrentState;
        private void Awake()
        {
            if (s_Instance) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {

            StartCoroutine(SetupGame());

        }


        IEnumerator SetupGame()
        {
            // setup ui manager
            m_UIManager = Instantiate(m_UIManagerPrefab).GetComponent<UIManager>();

            // setup player
            yield return m_playerManager = Instantiate(m_playerManagerPrefab).GetComponent<PlayerManager>();

            // setup customer
            m_customerManager = Instantiate(m_customerManagerPrefab).GetComponent<CustomerManager>();

            // setup camera 
            m_cameraController = CameraController.s_Instance;
            m_cameraController.SetupCamera(m_playerManager.Player.InputBehaviour);

            //setup chair
            m_chairManager = Instantiate(m_ChairManagerPrefab).GetComponent<ChairManager>();

            // setup hud display coin
            var canvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            m_tes = Instantiate(m_UTesPrefab, canvas).GetComponent<CoinHUDHandler>();
            m_playerManager.OnIncreaseCoin += m_tes.UpdateCoin;
            m_tes.transform.SetAsFirstSibling();

            //setup pause menu
            Instantiate(m_PauseMenuManagerPrefab);
        }

    }
}
