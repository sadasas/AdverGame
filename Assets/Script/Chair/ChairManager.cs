


using AdverGame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Chair
{
    public enum ChairAnchor
    {
        TOPLEFT,
        TOPRIGHT,
        BOTTOMLEFT,
        BOTTOMRIGHT,
        DEFAULT
    }
    public class ChairManager : MonoBehaviour
    {
        public static ChairManager s_Instance;

        InputBehaviour m_inputPlayer;
        List<ChairArea> m_chairAreas;
        ChairArea m_currentChairArea = null;
        
        Vector2[] m_defaultPos;
        Vector2[] m_centerAreaPos;
        int m_areaUnlocked = 0;


        [SerializeField] int m_chairCost;
        [SerializeField] float m_distanceBetweenTable = 1.6f;
        [SerializeField] GameObject m_chairCustomerPrefab;
        [SerializeField] GameObject m_chairOjolPrefab;
        [SerializeField] GameObject m_areaPrefab;
        [SerializeField] GameObject m_addChairIndicatorPrefab;

        public int ChairsInstanced =1;
        public List<ChairController> m_ojolChairs;
        private void Awake()
        {
            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;

        }
     
        private void Start()
        {
            m_inputPlayer = PlayerManager.s_Instance.Player.InputBehaviour;

            SearchDefaultPos();
            SearchCenterPos();
            var dataLevel = PlayerManager.s_Instance.Data.Level.CurrentLevel;
            for (int i = 0; i < 3; i++)
            {
                SpawnAreas();
            }

            StartCoroutine(LoadPlayerChairs());

            SpawnOjolChairs();


        }
        void SearchDefaultPos()
        {
            m_defaultPos = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                m_defaultPos[i] = GameObject.Find("DefaultChairPos" + i).transform.position;
            }
        }

        void SearchCenterPos()
        {
            m_centerAreaPos = new Vector2[3];
            for (int i = 0; i < 3; i++)
            {
                m_centerAreaPos[i] = GameObject.Find("CenterAreaPos" + i).transform.position;
            }
        }

        void SpawnAreas()
        {

            m_chairAreas ??= new List<ChairArea>();

            var chairArea = new ChairArea(m_distanceBetweenTable, m_chairCustomerPrefab, m_addChairIndicatorPrefab, m_inputPlayer, m_defaultPos[m_chairAreas.Count], m_chairAreas.Count, m_centerAreaPos[m_chairAreas.Count], m_areaPrefab, m_chairCost);
            chairArea.OnChairAdded += SaveDataChair;
            chairArea.OnAreaFull += GoToNextChairArea;
            m_chairAreas.Add(chairArea);


        }


        void SpawnOjolChairs()
        {
            var chairOjolOffsetPos = GameObject.Find("DefaultOjolChairPos").transform;

            var chairOjol = Instantiate(m_chairOjolPrefab, chairOjolOffsetPos.position, Quaternion.identity).GetComponent<ChairController>();
            m_ojolChairs ??= new();
            m_ojolChairs.Add(chairOjol);
        }

        public IEnumerator LoadPlayerChairs()
        {

            var dataChairsAreas = PlayerManager.s_Instance.GetDataChairs();
            if (dataChairsAreas != null && dataChairsAreas.Count > 0)
            {
                
                for (int a = 0; a < PlayerManager.s_Instance.GetCurrentLevel().MaxArea; a++)
                {
                    m_areaUnlocked++;
                    m_chairAreas[a].UnlockArea();
                    m_currentChairArea = m_chairAreas[a];
                   
                }

                yield return null;
                for (int i = 0; i < dataChairsAreas.Count; i++)
                {

                    foreach (var data in dataChairsAreas[i])
                    {
                        ChairsInstanced++;
                        m_chairAreas[i].SpawnChair(data.pos, data.anchor);
                        m_chairAreas[i].InitAddChairIndicator();
                    }
                   
                }
               
             
               
               
               
            }
            else
            {

                m_chairAreas[0].SetupArea();
                m_chairAreas[0].UnlockArea();
                m_currentChairArea = m_chairAreas[index: 0];
                m_areaUnlocked++;

            }

            
        }


        void SaveDataChair((ChairAnchor, Vector2) chairData, int address)
        {
            ChairsInstanced++;
            PlayerManager.s_Instance.SaveChair(chairData, address);
        }
        public bool IsOjolChairAvailable()
        {
            foreach (var chair in m_ojolChairs)
            {
                if (chair.Customer == null) return true;
            }
            return false;
        }

        public void UnlockArea(Level newLevel)
        {
            
            if (newLevel.MaxArea <= m_areaUnlocked) return;
            m_areaUnlocked++;
            m_chairAreas[newLevel.MaxArea - 1].UnlockArea();
            if (m_currentChairArea.IsFull) m_chairAreas[newLevel.MaxArea - 1].SetupArea(m_currentChairArea.CurrentChairCost);
            
            m_currentChairArea = m_chairAreas[newLevel.MaxArea - 1];

        }

        void GoToNextChairArea(int address, int currentChairCost)
        {
            
            if (address + 1 > m_chairAreas.Count-1 || m_chairAreas[address + 1].m_areaLock != null) return;
            
            m_chairAreas[address + 1].SetupArea(currentChairCost );
        }
    }
}
