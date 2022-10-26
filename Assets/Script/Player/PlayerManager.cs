
using AdverGame.Chair;
using AdverGame.Player;
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
public class PlayerData
{
    public int Coin;
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

        [SerializeField] GameObject m_playerPrefab;

        public PlayerController Player { get; private set; }

        public PlayerData Data;

        public Action<int> OnIncreaseCoin;
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
            OnIncreaseCoin?.Invoke(Data.Coin);
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

        public List<List<(ChairAnchor anchor, Vector2 pos)>> GetDataChairs()
        {
            if (Data.DataChairsAreas == null || Data.DataChairsAreas.Count == 0) return null;
            var dataChairAreas = new List<List<(ChairAnchor anchor, Vector2 pos)>>();
            for (int i = 0; i < Data.DataChairsAreas.Count; i++)
            {
                if (dataChairAreas.Count - 1 <= Data.DataChairsAreas[i].Address) dataChairAreas.Add(new List<(ChairAnchor anchor, Vector2 pos)>());
                var chair = (Data.DataChairsAreas[i].Anchor, Data.DataChairsAreas[i].Pos);
                dataChairAreas[Data.DataChairsAreas[i].Address].Add(chair);

            }


            return dataChairAreas;
        }
        public void IncreaseCoin(int coin)
        {
            Data.Coin += coin;
            OnIncreaseCoin?.Invoke(Data.Coin);
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
