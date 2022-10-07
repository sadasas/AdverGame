

using AdverGame.Player;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace AdverGame.Chair
{
    public enum ChairOffset
    {
        TOPLEFT,
        TOPRIGHT,
        BOTTOMLEFT,
        BOTTOMRIGHT
    }

    public class ChairManager : MonoBehaviour
    {

        Dictionary<ChairOffset, Vector2> chairOffsetPos;
        List<AddChairIndicator> m_addChairIndicators;
        [SerializeField] List<ChairController> Chairs;
        [SerializeField] List<Vector2> m_chairParents;
        InputBehaviour m_inputPlayer;

        [SerializeField] float m_distanceBetweenChair = 4f;
        [SerializeField] GameObject m_chairPrefab;
        [SerializeField] GameObject m_addChairIndicatorPrefab;

        private void Start()
        {
            m_inputPlayer = PlayerManager.s_Instance.Player.InputBehaviour;
            GetDefaultOffsetChair();
            LoadPlayerChairs();

            foreach (var chairOffset in chairOffsetPos)
            {
                var pos = new Vector2(chairOffset.Value.x + (chairOffset.Key == ChairOffset.TOPLEFT || chairOffset.Key == ChairOffset.BOTTOMLEFT ? -m_distanceBetweenChair : m_distanceBetweenChair), chairOffset.Value.y);
                InitAddChairIndicator(pos, chairOffset.Key);

            }
        }

        void GetDefaultOffsetChair()
        {
            chairOffsetPos ??= new();
            for (int i = 0; i < 4; i++)
            {
                var pos = GameObject.Find("ChairPos" + (i + 1)).transform;
                if (i == 0) chairOffsetPos.Add(ChairOffset.TOPLEFT, pos.position);
                else if (i == 1) chairOffsetPos.Add(ChairOffset.TOPRIGHT, pos.position);
                else if (i == 2) chairOffsetPos.Add(ChairOffset.BOTTOMRIGHT, pos.position);
                else if (i == 3) chairOffsetPos.Add(ChairOffset.BOTTOMLEFT, pos.position);
            }
        }

        void LoadPlayerChairs()
        {

            if (PlayerManager.s_Instance.Data.Chairs != null && PlayerManager.s_Instance.Data.Chairs.Count > 0)
            {
                for (int i = 0; i < PlayerManager.s_Instance.Data.Chairs.Count; i++)
                {
                    SpawnChair(PlayerManager.s_Instance.Data.Chairs[i]);
                }


            }
            else
            {


                foreach (var offset in chairOffsetPos.Values.ToArray())
                {

                    SpawnChair(offset);
                }

                foreach (var Vector2chairParent in m_chairParents)
                {
                    PlayerManager.s_Instance.SaveChair(Vector2chairParent);
                }
            }



        }


        void InitAddChairIndicator(Vector2 pos, ChairOffset offset)
        {

            m_addChairIndicators ??= new();
            var newIndicator = Instantiate(m_addChairIndicatorPrefab, pos, Quaternion.identity, GameObject.Find("Chair").transform).GetComponent<AddChairIndicator>();
            m_addChairIndicators.Add(newIndicator);
            m_inputPlayer.OnLeftClick += newIndicator.OnTouch;
            newIndicator.OnAddChair += AddIndicatorTriggered;
            newIndicator.Offset = offset;

        }

        void AddIndicatorTriggered(AddChairIndicator indicator)
        {
            var actualPos = new Vector2(indicator.transform.position.x > 0 ? indicator.transform.position.x + 0.72f : indicator.transform.position.x - 0.72f, indicator.transform.position.y);
            SpawnChair(actualPos);
            m_addChairIndicators.Remove(indicator);

            PlayerManager.s_Instance.SaveChair(actualPos);


            var pos = new Vector2(actualPos.x + (indicator.Offset == ChairOffset.TOPLEFT || indicator.Offset == ChairOffset.BOTTOMLEFT ? -m_distanceBetweenChair - 0.72f : m_distanceBetweenChair + 0.72f), actualPos.y);
            InitAddChairIndicator(pos, indicator.Offset);


            m_inputPlayer.OnLeftClick -= indicator.OnTouch;
            Destroy(indicator.gameObject);
        }

        void SpawnChair(Vector2 pos)
        {
            m_chairParents ??= new();
            var newChairParent = Instantiate(m_chairPrefab, pos, Quaternion.identity, GameObject.Find("Chair").transform);
            m_chairParents.Add(newChairParent.transform.position);

            Chairs ??= new();
            var chair1 = newChairParent.transform.GetChild(0).GetComponent<ChairController>();
            var chair2 = newChairParent.transform.GetChild(1).GetComponent<ChairController>();
            Chairs.Add(chair1);
            Chairs.Add(chair2);

            Vector2[] tempNewChair = { chair1.transform.position, chair2.transform.position };
            for (int i = 0; i < tempNewChair.Length; i++)
            {


                if (tempNewChair[i].x < chairOffsetPos[ChairOffset.TOPLEFT].x && math.abs(tempNewChair[i].y - chairOffsetPos[ChairOffset.TOPLEFT].y) < 0.5f)
                {


                    chairOffsetPos[ChairOffset.TOPLEFT] = tempNewChair[i];
                }
                else if (tempNewChair[i].x < chairOffsetPos[ChairOffset.BOTTOMLEFT].x && math.abs(tempNewChair[i].y - chairOffsetPos[ChairOffset.BOTTOMLEFT].y) < 0.5f)
                {

                    chairOffsetPos[ChairOffset.BOTTOMLEFT] = tempNewChair[i];
                }
                else if (tempNewChair[i].x > chairOffsetPos[ChairOffset.TOPRIGHT].x && math.abs(tempNewChair[i].y - chairOffsetPos[ChairOffset.TOPRIGHT].y) < 0.5f)
                {

                    chairOffsetPos[ChairOffset.TOPRIGHT] = tempNewChair[i];
                }
                else if (tempNewChair[i].x > chairOffsetPos[ChairOffset.BOTTOMRIGHT].x && math.abs(tempNewChair[i].y - chairOffsetPos[ChairOffset.BOTTOMRIGHT].y) < 0.5f)
                {

                    chairOffsetPos[ChairOffset.BOTTOMRIGHT] = tempNewChair[i];
                }


            }
        }


    }
}
