
using AdverGame.Chair;
using AdverGame.Player;
using AdverGame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DataChairSerializable
{
    public ChairAnchor Anchor;
    public Vector2 Pos;
    public int Address;
}

[Serializable]
public struct DataRandomItem
{
    public List<ItemSerializable> RandomItemGetted;
    public float CooldownBtnFaster;
}
[Serializable]
public class DataLevel
{
    Level[] m_levelVariant;
    public int m_currentLevel = 0;
    public Level CurrentLevel;
    public int CurrentExp;
    public Action<Level> OnLevelUpgrade;


    public void SetDefaultLevel()
    {
        m_levelVariant ??= AssetHelpers.GetAllLevelVariantRegistered();
        CurrentLevel = m_levelVariant[0];

    }

    public void IncreaseExp(int exp)
    {
        if (CurrentExp + exp <= CurrentLevel.MaxExp) CurrentExp += exp;
        else
        {
            m_levelVariant ??= AssetHelpers.GetAllLevelVariantRegistered();
            if (CurrentLevel.Sequence >= m_levelVariant.Length) CurrentExp += exp;
            else
            {
                CurrentLevel = m_levelVariant[CurrentLevel.Sequence];
                CurrentExp = exp;
                OnLevelUpgrade?.Invoke(CurrentLevel);
            }
        }
    }

}
[Serializable]
public class PlayerData
{
    public long Coin;
    public DataRandomItem RandomItem;
    public DataLevel Level;
    public List<ItemSerializable> Items;
    public List<DataChairSerializable> DataChairsAreas;
    public List<String> CharacterCollection;


}
namespace AdverGame.Player
{


    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager s_Instance;

        IOBehaviour m_io;

        PlayerData m_data;

        [SerializeField] GameObject m_playerPrefab;

        public PlayerController Player { get; private set; }


        public PlayerData Data => m_data;

        public Action<long, int> OnIncreaseCoin;
        public Action<int, int> OnIncreaseExp;
        public Action<Level> OnIncreaseLevel;
        public Action<PlayerData> OnDataLoaded;
        private void Awake()
        {
            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_io = new();
            m_data = new();

            Player = Instantiate(m_playerPrefab, GameObject.Find("PlayerPos").transform.position, Quaternion.identity).GetComponent<PlayerController>();
            StartCoroutine(LoadDataPlayer());
        }


        IEnumerator LoadDataPlayer()
        {
            yield return m_io.LoadData(ref m_data);

            OnIncreaseCoin?.Invoke(Data.Coin, 0);

            if (Data.Level == null)
            {
                Data.Level = new DataLevel();
                Data.Level.SetDefaultLevel();
                SaveDataPlayer();
            }
            OnDataLoaded?.Invoke(Data);
            Data.Level.OnLevelUpgrade += (newLevel) => { OnIncreaseLevel?.Invoke(newLevel); };

        }

        public Level GetCurrentLevel()
        {
            return Data.Level.CurrentLevel;
        }
        public void SaveDataPlayer()
        {
            m_io.SaveData(Data);
        }

        public void SaveItem(List<ItemSerializable> items)
        {
            Data.Items = items;
            SaveDataPlayer();
        }


        public void SaveChair((ChairAnchor, Vector2) chairData, int address)
        {
            //weird algoritmh
            Data.DataChairsAreas ??= new();
            var newData = new DataChairSerializable { Anchor = chairData.Item1, Pos = chairData.Item2, Address = address };


            Data.DataChairsAreas.Add(newData);

            SaveDataPlayer();

        }

        public void SaveDataRandomItem(DataRandomItem data)
        {
            Data.RandomItem = data;
            SaveDataPlayer();
        }

        public DataRandomItem GetDataRandomItem()
        {
            return Data.RandomItem;
        }
        public List<List<(ChairAnchor anchor, Vector2 pos)>> GetDataChairs()
        {
            if (Data.DataChairsAreas == null || Data.DataChairsAreas.Count == 0) return null;
            var dataChairAreas = new List<List<(ChairAnchor anchor, Vector2 pos)>>();
            for (int i = 0; i < Data.DataChairsAreas.Count; i++)
            {
                if (dataChairAreas.Count - 1 < Data.DataChairsAreas[i].Address) dataChairAreas.Add(new List<(ChairAnchor anchor, Vector2 pos)>());
                var chair = (Data.DataChairsAreas[i].Anchor, Data.DataChairsAreas[i].Pos);
                dataChairAreas[Data.DataChairsAreas[i].Address].Add(chair);

            }


            return dataChairAreas;
        }
        public void IncreaseCoin(int increment)
        {
            Data.Coin += increment;
            OnIncreaseCoin?.Invoke(Data.Coin, increment);
            SaveDataPlayer();

        }

        public void IncreaseExp(int increment)
        {
            Data.Level.IncreaseExp(increment);
            OnIncreaseExp?.Invoke(Data.Level.CurrentExp, increment);
            SaveDataPlayer();

        }

        public void SaveCharacterCollected(string characterName)
        {
            Data.CharacterCollection ??= new List<string>();
            Data.CharacterCollection.Add(characterName);
            SaveDataPlayer();
        }

    }

}
