

using AdverGame.Player;
using System.Collections.Generic;
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
        List<ChairController> Chairs;
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


                foreach (var offset in chairOffsetPos)
                {

                    SpawnChair(offset.Value);
                }


                foreach (var chair in Chairs)
                {
                    PlayerManager.s_Instance.SaveChair(chair.transform.position);
                }



            }

            foreach (var chair in Chairs)
            {
                if (chair.transform.position.x < chairOffsetPos[ChairOffset.TOPLEFT].x && chair.transform.position.y == chairOffsetPos[ChairOffset.TOPLEFT].y) chairOffsetPos[ChairOffset.TOPLEFT] = chair.transform.position;
                else if (chair.transform.position.x < chairOffsetPos[ChairOffset.BOTTOMLEFT].x && chair.transform.position.y == chairOffsetPos[ChairOffset.BOTTOMLEFT].y) chairOffsetPos[ChairOffset.BOTTOMLEFT] = chair.transform.position;
                else if (chair.transform.position.x > chairOffsetPos[ChairOffset.TOPRIGHT].x && chair.transform.position.y == chairOffsetPos[ChairOffset.TOPRIGHT].y) chairOffsetPos[ChairOffset.TOPRIGHT] = chair.transform.position;
                else if (chair.transform.position.x > chairOffsetPos[ChairOffset.BOTTOMRIGHT].x && chair.transform.position.y == chairOffsetPos[ChairOffset.BOTTOMRIGHT].y) chairOffsetPos[ChairOffset.BOTTOMRIGHT] = chair.transform.position;
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

            SpawnChair(indicator.transform.position);
            m_addChairIndicators.Remove(indicator);

            PlayerManager.s_Instance.SaveChair(indicator.transform.position);


            var pos = new Vector2(indicator.transform.position.x + (indicator.Offset == ChairOffset.TOPLEFT || indicator.Offset == ChairOffset.BOTTOMLEFT ? -m_distanceBetweenChair : m_distanceBetweenChair), indicator.transform.position.y);
            InitAddChairIndicator(pos, indicator.Offset);


            m_inputPlayer.OnLeftClick -= indicator.OnTouch;
            Destroy(indicator.gameObject);
        }

        void SpawnChair(Vector2 pos)
        {
            var newChair = Instantiate(m_chairPrefab, pos, Quaternion.identity, GameObject.Find("Chair").transform).GetComponent<ChairController>();
            Chairs ??= new();
            Chairs.Add(newChair);

            if (pos.x < chairOffsetPos[ChairOffset.TOPLEFT].x && pos.y == chairOffsetPos[ChairOffset.TOPLEFT].y) chairOffsetPos[ChairOffset.TOPLEFT] = pos;
            else if (pos.x < chairOffsetPos[ChairOffset.BOTTOMLEFT].x && pos.y == chairOffsetPos[ChairOffset.BOTTOMLEFT].y) chairOffsetPos[ChairOffset.BOTTOMLEFT] = pos;
            else if (pos.x > chairOffsetPos[ChairOffset.TOPRIGHT].x && pos.y == chairOffsetPos[ChairOffset.TOPRIGHT].y) chairOffsetPos[ChairOffset.TOPRIGHT] = pos;
            else if (pos.x > chairOffsetPos[ChairOffset.BOTTOMRIGHT].x && pos.y == chairOffsetPos[ChairOffset.BOTTOMRIGHT].y) chairOffsetPos[ChairOffset.BOTTOMRIGHT] = pos;

        }


    }
}
