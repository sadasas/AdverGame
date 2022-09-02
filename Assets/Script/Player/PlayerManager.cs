﻿using AdverGame.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerData
{
    public int Coin;
    public List<Item> Items;
}
namespace AdverGame.Player
{


    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager s_Instance;

        IOBehaviour m_io;

        [SerializeField] GameObject m_playerPrefab;

        public PlayerController Player { get; private set; }

        public PlayerData Data;
     
        private void Awake()
        {
            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_io = new();
            Data = new();

            Player = Instantiate(m_playerPrefab, GameObject.Find("PlayerPos").transform.position, Quaternion.identity).GetComponent<PlayerController>();
            StartCoroutine(LoadDataPlayer());
        }


        IEnumerator LoadDataPlayer()
        {
            yield return m_io.LoadData(ref Data);
            Player.OnIncreaseCoin?.Invoke(Data.Coin);
        }

      
        public void SaveDataPlayer()
        {
            m_io.SaveData(Data);
        }
    }

}
