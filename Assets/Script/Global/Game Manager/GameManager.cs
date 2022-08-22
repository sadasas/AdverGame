using AdverGame.Customer;
using AdverGame.Player;
using AdverGame.UI;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager s_Instance;

        CustomerController m_customerController;

        [Header("CUSTOMER SETTING")]
        [SerializeField] int m_delaySpawner;
        [SerializeField] List<GameObject> m_custPrefab;

        [Header("UI SETTING")]
        [SerializeField] GameObject m_UIPrefab;

        [Header("PlAYER SETTING")]
        [SerializeField] GameObject m_playerPrefab;
        private void Awake()
        {
            if (s_Instance) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Instantiate(m_playerPrefab);
            var canvas = GameObject.FindGameObjectWithTag("MainCanvas").transform;
            var ui = Instantiate(m_UIPrefab, canvas).GetComponent<TestUI>();
            PlayerManager.s_Instance.OnIncreaseCoin += ui.UpdateCoin;
            m_customerController = new(m_delaySpawner, this, m_custPrefab);
        }



    }
}
