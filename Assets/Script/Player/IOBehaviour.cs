

using AdverGame.Utility;
using System.IO;
using UnityEngine;

namespace AdverGame.Player
{
    public class IOBehaviour
    {
        readonly string m_path = Application.persistentDataPath + "/DataPlayer.json";
        readonly object m_persistanDataLock = new();
        FileStream m_stream;


        public void SaveData(PlayerData data)
        {

            try
            {
                lock (m_persistanDataLock)
                {
                    using (m_stream = new(m_path, FileMode.Create))
                    {
                        using var sWriter = new StreamWriter(m_stream);

                        var pdTemp = new PlayerData();
                        pdTemp.Coin = data.Coin;
                        pdTemp.Items = new();
                        if (data.Items != null && data.Items.Count > 0)
                        {
                            foreach (var item in data.Items)
                            {
                                var itemTemp = new ItemSerializable();
                                itemTemp.m_content = item.Content.name;
                                pdTemp.Items.Add(itemTemp);
                            }
                        }


                        string json = JsonUtility.ToJson(pdTemp);

                        sWriter.Write(json);
                    }
                }

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public PlayerData LoadData(ref PlayerData data)
        {
            Debug.Log(m_path);
            PlayerData tempPd = new();
            try
            {
                if (File.Exists(m_path))
                {
                    lock (m_persistanDataLock)
                    {
                        using (m_stream = new(m_path, FileMode.Open))
                        {
                            using var rReader = new StreamReader(m_stream);
                            string json = rReader.ReadToEnd();
                            tempPd = JsonUtility.FromJson<PlayerData>(json);
                            foreach (var item in tempPd.Items)
                            {
                                var so = AssetHelpers.GetScriptableItemRegistered(item.m_content);
                                item.Content = so;
                                item.m_content = null;
                            }
                        }
                    }

                }
            }
            catch (System.Exception)
            {

                throw;
            }

            return data = tempPd;
        }
    }
}
