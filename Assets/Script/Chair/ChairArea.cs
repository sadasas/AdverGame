


using AdverGame.Player;
using AdverGame.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Chair
{
    /// <summary>
    /// TODO: Make obj to buy more chair set  algoritm more dinamic and not constrained lke current algoritm
    /// </summary>
    [Serializable]
    public class ChairArea
    {
        public ChairArea(float distanceBetweenTable, GameObject chairCustomerPrefab, GameObject addChairIndicatorPrefab, InputBehaviour input, Vector2 defaultPos, int address, Vector2 centerPos, GameObject areaPrefab, int chairCost)
        {
            m_distanceBetweenTable = distanceBetweenTable;
            m_chairCustomerPrefab = chairCustomerPrefab;
            m_addChairIndicatorPrefab = addChairIndicatorPrefab;
            m_playerInput = input;
            m_defaultPos = defaultPos;
            m_address = address;
            m_areaLockPrefab = areaPrefab;
            m_centerPos = centerPos;
            m_chairCost = chairCost;
            LokArea();
            m_playerInput.OnLeftClick += OnClick;
            CurrentChairCost = m_chairCost;
        }

        (ChairAnchor, Vector2) m_lastChairAdded;

        AddChairIndicator m_chairIndicators;
        List<(ChairAnchor, Vector2)> m_chairParents;
        List<ChairController> m_customerChairs;
        InputBehaviour m_playerInput;
        Vector2 m_defaultPos;
        Vector2 m_centerPos;
        GameObject m_areaLockPrefab;
        int m_chairCost;
        public bool IsFull = false;

        [SerializeField] float m_distanceBetweenTable = 1.6f;
        [SerializeField] GameObject m_chairCustomerPrefab;
        [SerializeField] int m_address;
        [SerializeField] GameObject m_addChairIndicatorPrefab;

        public GameObject m_areaLock;
        public int CurrentChairCost { get; private set; }
        public Action<(ChairAnchor, Vector2), int> OnChairAdded;
        public Action<int, int> OnAreaFull;





        (ChairAnchor anchor, Vector2 pos, bool isEmpty) SearchEmptyField()
        {
            // in a area max table are 4
            // try get pos empty in area by search clockwise 
            var result = (m_lastChairAdded.Item1) switch
            {
                ChairAnchor.TOPLEFT => (ChairAnchor.TOPRIGHT, new Vector2(m_lastChairAdded.Item2.x + m_distanceBetweenTable, m_lastChairAdded.Item2.y), true),
                ChairAnchor.TOPRIGHT => (ChairAnchor.BOTTOMRIGHT, new Vector2(m_lastChairAdded.Item2.x, m_lastChairAdded.Item2.y - 1.7f), true),
                ChairAnchor.BOTTOMRIGHT => (ChairAnchor.BOTTOMLEFT, new Vector2(m_lastChairAdded.Item2.x - +m_distanceBetweenTable, m_lastChairAdded.Item2.y), true),
                ChairAnchor.BOTTOMLEFT => (ChairAnchor.DEFAULT, Vector2.zero, false),
                _ => (ChairAnchor.DEFAULT, Vector2.zero, false)
            };


            return result;
        }

        public void InitAddChairIndicator()
        {
            if (m_chairIndicators != null)
            {

                m_chairIndicators.OnAddChair -= AddIndicatorTriggered;
                GameObject.Destroy(m_chairIndicators.gameObject);
            }
            var newEmptyFieldData = SearchEmptyField();
            if (!newEmptyFieldData.isEmpty)
            {
                IsFull = true;
                OnAreaFull?.Invoke(m_address, CurrentChairCost);
                return;
            }

            m_chairIndicators = GameObject.Instantiate(m_addChairIndicatorPrefab, newEmptyFieldData.pos, Quaternion.identity, GameObject.Find("Chair").transform).GetComponent<AddChairIndicator>();

            m_chairIndicators.Price = CurrentChairCost;
            CurrentChairCost = (int)(CurrentChairCost * 1.5f);
            m_chairIndicators.OnAddChair += AddIndicatorTriggered;
            m_chairIndicators.Offset = newEmptyFieldData.anchor;

        }

        void InitAddChairIndicator(ChairAnchor anchor, Vector2 pos)
        {
            if (m_chairIndicators != null)
            {

                m_chairIndicators.OnAddChair -= AddIndicatorTriggered;
                GameObject.Destroy(m_chairIndicators.gameObject);
            }


            m_chairIndicators = GameObject.Instantiate(m_addChairIndicatorPrefab, pos, Quaternion.identity, GameObject.Find("Chair").transform).GetComponent<AddChairIndicator>();

          
            m_chairIndicators.Price = CurrentChairCost;
            CurrentChairCost =(int) (CurrentChairCost * 1.5f);
            m_chairIndicators.OnAddChair += AddIndicatorTriggered;
            m_chairIndicators.Offset = anchor;

        }

        void OnClick(GameObject obj)
        {
            if (m_areaLock == null) return;

            if (obj == m_areaLock.transform.GetChild(0).gameObject)
            {

                if (m_address == 1)
                {
                    UIManager.s_Instance.ShowNotification("Perlu experience level 4 untuk membuka area ini ");
                }
                else if (m_address == 2)
                {
                    UIManager.s_Instance.ShowNotification("Perlu experience level 7 untuk membuka area ini ");
                }
            }
        }
        void LokArea()
        {
            m_areaLock = GameObject.Instantiate(m_areaLockPrefab, m_centerPos, Quaternion.identity);
        }

       
        public void UnlockArea()
        {
            GameObject.Destroy(m_areaLock);
            m_areaLock = null;
        }
        public void SetupArea(int currentChairCost)
        {

            CurrentChairCost = currentChairCost;
            InitAddChairIndicator(ChairAnchor.TOPLEFT, m_defaultPos);
        }
        public void SetupArea()
        {

            SpawnChair(m_defaultPos, ChairAnchor.TOPLEFT);
            OnChairAdded?.Invoke(m_lastChairAdded, m_address);
            InitAddChairIndicator();
        }
        void AddIndicatorTriggered(AddChairIndicator indicator)
        {


            SpawnChair(indicator.transform.position, indicator.Offset);
            OnChairAdded?.Invoke(m_lastChairAdded, m_address);

            InitAddChairIndicator();

        }

        public void SpawnChair(Vector2 pos, ChairAnchor anchor)
        {
            m_chairParents ??= new();
            var newChairParent = GameObject.Instantiate(m_chairCustomerPrefab, pos, Quaternion.identity, GameObject.Find("Chair").transform);
            m_chairParents.Add((anchor, newChairParent.transform.position));

            for (int i = 0; i < newChairParent.transform.childCount; i++)
            {
                var lyr = anchor == ChairAnchor.TOPLEFT || anchor == ChairAnchor.TOPRIGHT ? 2 : 8;
                newChairParent.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = lyr;
            }

            m_customerChairs ??= new();
            var chair1 = newChairParent.transform.GetChild(0).GetComponent<ChairController>();
            var chair2 = newChairParent.transform.GetChild(1).GetComponent<ChairController>();
            m_customerChairs.Add(chair1);
            m_customerChairs.Add(chair2);

            m_lastChairAdded = (anchor, pos);



        }


    }
}
