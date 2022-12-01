

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

                        pdTemp.CharacterCollection = data.CharacterCollection;
                        pdTemp.Items = new();
                        pdTemp.DataChairsAreas = data.DataChairsAreas;



                        pdTemp.Level = new DataLevel();
                        pdTemp.Level.CurrentLevel = data.Level.CurrentLevel;
                        pdTemp.RandomItem = new DataRandomItem();
                        pdTemp.RandomItem.CooldownBtnFaster = data.RandomItem.CooldownBtnFaster;
                      
                        pdTemp.Level.CurrentExp = data.Level.CurrentExp;
                        if (pdTemp.Level != null && pdTemp.Level.CurrentLevel != null)
                        {
                            pdTemp.Level.m_currentLevel = pdTemp.Level.CurrentLevel.Sequence;
                            pdTemp.Level.CurrentLevel = null;
                        }
                        if (data.RandomItem.RandomItemGetted != null && data.RandomItem.RandomItemGetted.Count > 0)
                        {
                           
                            pdTemp.RandomItem.RandomItemGetted = new();
                            foreach (var item in data.RandomItem.RandomItemGetted)
                            {
                                var itemTemp = new ItemSerializable();
                                itemTemp.m_content = item.Content.name;
                                itemTemp.IncreaseStack(item.Stack - 1);
                                pdTemp.RandomItem.RandomItemGetted.Add(itemTemp);
                            }
                        }

                        if (data.Items != null && data.Items.Count > 0)
                        {
                            foreach (var item in data.Items)
                            {
                                var itemTemp = new ItemSerializable();
                                itemTemp.m_content = item.Content.name;
                                itemTemp.IncreaseStack(item.Stack - 1);
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
                            if (tempPd.RandomItem.RandomItemGetted != null && tempPd.RandomItem.RandomItemGetted.Count > 0)
                            {
                                foreach (var item in tempPd.RandomItem.RandomItemGetted)
                                {
                                    var so = AssetHelpers.GetScriptableItemRegistered(item.m_content);
                                    item.Content = so;
                                    item.m_content = null;
                                }
                            }
                           
                            var soLevel = AssetHelpers.GetAllLevelVariantRegistered();
                            foreach (var so in soLevel)
                            {
                                if (so.Sequence == tempPd.Level.m_currentLevel)
                                {
                                    tempPd.Level.CurrentLevel = so;

                                    break;
                                }
                            }
                            tempPd.Level.m_currentLevel = 0;

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
