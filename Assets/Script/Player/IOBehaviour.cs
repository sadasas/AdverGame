

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

                        string json = JsonUtility.ToJson(data);

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
