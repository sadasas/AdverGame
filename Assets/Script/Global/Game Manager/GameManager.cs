using AdverGame.CameraGame;
using AdverGame.Chair;
using AdverGame.CharacterCollection;
using AdverGame.Customer;
using AdverGame.Player;
using AdverGame.Sound;
using AdverGame.Tutorial;
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
        CoinHUDHandler m_coinHUDHandler;
        ExpHUDHandler m_expHUDHandler;
        ChairManager m_chairManager;
        SoundManager m_soundManager;
        private Transform canvas;
        CameraController m_cameraController;
        CharacterCollectionManager m_characterCollectionManager;
        TutorialHandler m_tutorialHandler;

        [SerializeField] GameObject m_UIPrefab;
        [SerializeField] GameObject m_playerManagerPrefab;
        [SerializeField] GameObject m_customerManagerPrefab;
        [SerializeField] GameObject m_UIManagerPrefab;
        [SerializeField] GameObject m_HUDCoinPrefab;
        [SerializeField] GameObject m_HUDExpPrefab;
        [SerializeField] GameObject m_ChairManagerPrefab;
        [SerializeField] GameObject m_PauseMenuManagerPrefab;
        [SerializeField] GameObject m_CharacterCollectionPrefab;
        [SerializeField] GameObject m_SoundManagerPrefab;
        [SerializeField] GameObject m_tutorialHandlerPrefab;

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

        /// <summary>
        /// TODO : Only manage entity manager 
        /// </summary>
        /// <returns></returns>
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
            m_playerManager.OnIncreaseLevel += m_chairManager.UnlockArea;

            // setup hud display coin
            canvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            m_coinHUDHandler = Instantiate(m_HUDCoinPrefab, canvas).GetComponent<CoinHUDHandler>();
            m_playerManager.OnIncreaseCoin += m_coinHUDHandler.UpdateCoin;
            m_coinHUDHandler.transform.SetAsFirstSibling();

            //setuo hud display exp
            m_expHUDHandler = Instantiate(m_HUDExpPrefab, canvas).GetComponent<ExpHUDHandler>();
            m_playerManager.OnIncreaseExp += m_expHUDHandler.IncreaseXP;
            m_playerManager.OnIncreaseLevel += m_expHUDHandler.IncreaseLevel;

            m_expHUDHandler.transform.SetAsFirstSibling();

            //setup pause menu
            Instantiate(m_PauseMenuManagerPrefab);

            //setup character collection
            m_characterCollectionManager = Instantiate(m_CharacterCollectionPrefab).GetComponent<CharacterCollectionManager>();

            m_soundManager = Instantiate(m_SoundManagerPrefab).GetComponent<SoundManager>();

            //m_tutorialHandler = Instantiate(m_tutorialHandlerPrefab).GetComponent<TutorialHandler>();

        }

    }
}
