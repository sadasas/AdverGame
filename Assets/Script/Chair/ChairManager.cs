

using AdverGame.Player;
using System;
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

    public class ChairArea
    {
        public ChairArea(float distanceBetweenTable, GameObject chairCustomerPrefab, GameObject addChairIndicatorPrefab, InputBehaviour input, Vector2 defaultPos, int address)
        {
            m_distanceBetweenTable = distanceBetweenTable;
            m_chairCustomerPrefab = chairCustomerPrefab;
            m_addChairIndicatorPrefab = addChairIndicatorPrefab;
            m_playerInput = input;
            m_defaultPos = defaultPos;
            m_address = address;
        }

        (ChairAnchor, Vector2) m_lastChairAdded;

        AddChairIndicator m_chairIndicators;
        List<(ChairAnchor, Vector2)> m_chairParents;
        List<ChairController> m_customerChairs;
        InputBehaviour m_playerInput;
        Vector2 m_defaultPos;

        [SerializeField] float m_distanceBetweenTable = 1.6f;
        [SerializeField] GameObject m_chairCustomerPrefab;
        [SerializeField] int m_address;
        [SerializeField] GameObject m_addChairIndicatorPrefab;

        public Action<(ChairAnchor, Vector2), int> OnChairAdded;



        (ChairAnchor anchor, Vector2 pos, bool isEmpty) SearchEmptyField()
        {
            // in a area max table are 4
            // try get pos empty in area by search clockwise 
            var result = (m_lastChairAdded.Item1) switch
            {
                ChairAnchor.TOPLEFT => (ChairAnchor.TOPRIGHT, new Vector2(m_lastChairAdded.Item2.x + m_distanceBetweenTable, m_lastChairAdded.Item2.y), true),
                ChairAnchor.TOPRIGHT => (ChairAnchor.BOTTOMRIGHT, new Vector2(m_lastChairAdded.Item2.x, m_lastChairAdded.Item2.y - 1.7f), true),
                ChairAnchor.BOTTOMRIGHT => (ChairAnchor.BOTTOMLEFT, new Vector2(m_lastChairAdded.Item2.x - +m_distanceBetweenTable, m_lastChairAdded.Item2.y), true),
                ChairAnchor.BOTTOMLEFT => (ChairAnchor.DEFAULT, Vector2.zero, false)
            };


            return result;
        }

        void InitAddChairIndicator(ChairAnchor offset, Vector2 pos)
        {

            if (m_chairIndicators != null)
            {
                m_playerInput.OnLeftClick -= m_chairIndicators.OnTouch;
                m_chairIndicators.OnAddChair -= AddIndicatorTriggered;
                GameObject.Destroy(m_chairIndicators.gameObject);
            }
            m_chairIndicators = GameObject.Instantiate(m_addChairIndicatorPrefab, pos, Quaternion.identity, GameObject.Find("Chair").transform).GetComponent<AddChairIndicator>();

            m_playerInput.OnLeftClick += m_chairIndicators.OnTouch;
            m_chairIndicators.OnAddChair += AddIndicatorTriggered;
            m_chairIndicators.Offset = offset;

        }

        public void SetupArea(bool isDefaultArea)
        {

            if (isDefaultArea)
            {
                SpawnChair(m_defaultPos, ChairAnchor.TOPLEFT);
                OnChairAdded?.Invoke(m_lastChairAdded, m_address);

            }
            else
            {
                InitAddChairIndicator(ChairAnchor.TOPLEFT, m_defaultPos);
            }
        }
        void AddIndicatorTriggered(AddChairIndicator indicator)
        {


            SpawnChair(indicator.transform.position, indicator.Offset);
            OnChairAdded?.Invoke(m_lastChairAdded, m_address);


            var newEmptyFieldData = SearchEmptyField();
            if (m_chairIndicators != null)
            {
                m_playerInput.OnLeftClick -= m_chairIndicators.OnTouch;
                m_chairIndicators.OnAddChair -= AddIndicatorTriggered;
                GameObject.Destroy(m_chairIndicators.gameObject);
            }
            if (!newEmptyFieldData.isEmpty) return;
            InitAddChairIndicator(newEmptyFieldData.anchor, newEmptyFieldData.pos);

            m_playerInput.OnLeftClick -= indicator.OnTouch;

        }

        public void SpawnChair(Vector2 pos, ChairAnchor anchor)
        {
            m_chairParents ??= new();
            var newChairParent = GameObject.Instantiate(m_chairCustomerPrefab, pos, Quaternion.identity, GameObject.Find("Chair").transform);
            m_chairParents.Add((anchor, newChairParent.transform.position));

            m_customerChairs ??= new();
            var chair1 = newChairParent.transform.GetChild(0).GetComponent<ChairController>();
            var chair2 = newChairParent.transform.GetChild(1).GetComponent<ChairController>();
            m_customerChairs.Add(chair1);
            m_customerChairs.Add(chair2);

            m_lastChairAdded = (anchor, pos);
            var newEmptyFieldData = SearchEmptyField();
            if (!newEmptyFieldData.isEmpty) return;
            InitAddChairIndicator(newEmptyFieldData.anchor, newEmptyFieldData.pos);

        }


    }
    public class ChairManager : MonoBehaviour
    {
        public static ChairManager s_Instance;

        InputBehaviour m_inputPlayer;
        ChairArea[] m_chairAreas;
        Vector2[] m_defaultPos;

        [SerializeField] float m_distanceBetweenTable = 1.6f;
        [SerializeField] GameObject m_chairCustomerPrefab;
        [SerializeField] GameObject m_chairOjolPrefab;
        [SerializeField] GameObject m_addChairIndicatorPrefab;


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
            SpawnAreas();

            LoadPlayerChairs();
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
        void SpawnAreas()
        {
            m_chairAreas = new ChairArea[3];
            for (int i = 0; i < 3; i++)
            {
                var chairArea = new ChairArea(m_distanceBetweenTable, m_chairCustomerPrefab, m_addChairIndicatorPrefab, m_inputPlayer, m_defaultPos[i], i);
                chairArea.OnChairAdded += SaveDataChair;
                m_chairAreas[i] = chairArea;
            }


        }

        private void SpawnOjolChairs()
        {
            var chairOjolOffsetPos = GameObject.Find("DefaultOjolChairPos").transform;

            var chairOjol = Instantiate(m_chairOjolPrefab, chairOjolOffsetPos.position, Quaternion.identity).GetComponent<ChairController>();
            m_ojolChairs ??= new();
            m_ojolChairs.Add(chairOjol);
        }

        void LoadPlayerChairs()
        {


            var dataChairsAreas = PlayerManager.s_Instance.GetDataChairs();
            if (dataChairsAreas != null && dataChairsAreas.Count > 0)
            {

                for (int i = 0; i < dataChairsAreas.Count; i++)
                {

                    foreach (var data in dataChairsAreas[i])
                    {
                        m_chairAreas[i].SpawnChair(data.pos, data.anchor);
                    }
                }

            }
            else
            {
                m_chairAreas[0].SetupArea(true);


            }



        }


        void SaveDataChair((ChairAnchor, Vector2) chairData, int address)
        {
            Debug.Log("save chairs");
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


    }
}
