using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class PlayerData
{
    public int Coin;
}
namespace AdverGame.Player
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager s_Instance;

        InputBehaviour m_inputPlayer;
        IOBehaviour m_io;
        public PlayerData Data;

        [Header("INPUT BEHAVIOUR SETTING")]
        [SerializeField] LayerMask m_clickableMask;

        public Action<int> OnIncreaseCoin;
        private void Awake()
        {
            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_inputPlayer = new(m_clickableMask);
            m_io = new();
            Data = new();
            StartCoroutine(LoadDataPlayer());
        }

        IEnumerator LoadDataPlayer()
        {
            yield return m_io.LoadData(ref Data);
            OnIncreaseCoin?.Invoke(Data.Coin);
        }

        private void Update()
        {
            m_inputPlayer.Update();
        }

        public void IncreaseCoin(int coin)
        {
            Data.Coin += coin;
            OnIncreaseCoin?.Invoke(Data.Coin);
            m_io.SaveData(Data);
        }
    }

}
