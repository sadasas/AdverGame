
using AdverGame.Chair;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Customer
{
    public enum DummyState
    {
        WALK,
        IDLE,
        DEFAULT
    }
    public class DummyController : MonoBehaviour
    {
        private int m_touchCount;
        private Animator m_animDummyCharacter;

        [SerializeField] Slider m_touchSlider;
        [SerializeField] Image m_noticeImage;

        public DummyState CurrentState;
        public DummyVariant Variant;
        public Coroutine CurrentCoro;
        public float SpawnDelay;
        public Action<DummyController> OnResetPos;
        public Action<List<ChairController>, DummyController, Vector2> OnToChair;
        public Vector2 TargetPos;


        private void Update()
        {

            m_touchSlider.value = m_touchCount;
        }
        bool IsReachDestination()
        {
            if (Vector2.Distance(transform.position, TargetPos) == 0) return true;
            return false;
        }
        (bool isTrue, List<ChairController> chairs) IsChairAvailable()
        {
            var chairs = GameObject.FindGameObjectsWithTag("Chair");
            var chairAvalaibles = new List<ChairController>();

            foreach (var chair in chairs)
            {
                var cc = chair.GetComponent<ChairController>();

                if (cc.Customer == null)
                {

                    chairAvalaibles.Add(cc);

                }

            }

            return (chairAvalaibles == null || chairAvalaibles.Count == 0 ? false : true, chairAvalaibles);
        }

        (bool isTrue, List<ChairController> chairs) IsChairAvailable(CustomerType type)
        {
            var chairs = GameObject.FindGameObjectsWithTag("Chair");
            var chairAvalaibles = new List<ChairController>();

            var typeMatch = (type) switch
            {
                CustomerType.COMMON => ChairType.CUSTOMERS,
                CustomerType.OJOL => ChairType.DRIVERS,
                CustomerType.RARE => ChairType.CUSTOMERS,
                _ => ChairType.CUSTOMERS


            };
            foreach (var chair in chairs)
            {
                var cc = chair.GetComponent<ChairController>();

                if (cc.Customer == null && cc.Type == typeMatch)
                {

                    chairAvalaibles.Add(cc);

                }

            }

            return (chairAvalaibles == null || chairAvalaibles.Count == 0 ? false : true, chairAvalaibles);
        }

        public void Setup()
        {
            if (CurrentCoro != null) StopCoroutine(CurrentCoro);
            m_touchCount = 0;
            m_animDummyCharacter ??= GetComponent<Animator>();
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
            m_animDummyCharacter.SetBool("IsWalk", false);
            m_touchSlider.gameObject.SetActive(false);
            m_noticeImage.gameObject.SetActive(false);
            if (transform.position.x < 0) transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            CurrentState = DummyState.DEFAULT;
        }
        public void OnTouch(GameObject obj)
        {
            if (obj != this.gameObject) return;
            if (m_touchCount == 0)
            {

                if (CurrentCoro != null) StopCoroutine(CurrentCoro);
                m_touchCount++;
                m_animDummyCharacter.SetBool("IsWalk", false);
                m_touchSlider.gameObject.SetActive(true);

                CurrentState = DummyState.IDLE;

                CurrentCoro = StartCoroutine(WaitDoubleClick());


            }
            else if (m_touchCount == 1)
            {

                if (CurrentCoro != null) StopCoroutine(CurrentCoro);
                m_animDummyCharacter.SetBool("IsWalk", false);
                m_touchSlider.gameObject.SetActive(true);
                var isChairAvailData = IsChairAvailable();


                m_touchCount = 0;
                m_noticeImage.gameObject.SetActive(false);
                m_touchSlider.gameObject.SetActive(false);
                CurrentState = DummyState.DEFAULT;
                OnToChair?.Invoke(isChairAvailData.chairs, this, TargetPos);



            }



        }
        public void Move()
        {

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(TargetPos.x, TargetPos.y), Variant.Speed * Time.deltaTime);
        }

        public IEnumerator Walking()
        {
            CurrentState = DummyState.WALK;
            m_animDummyCharacter ??= GetComponent<Animator>();
            m_animDummyCharacter.SetBool("IsWalk", true);


            while (SpawnDelay > 0)
            {
                SpawnDelay -= Time.deltaTime;
                yield return null;
            }
            while (SpawnDelay <= 0 && !IsReachDestination())
            {

                Move();
                yield return null;
            }
            OnResetPos?.Invoke(this);
        }
        public IEnumerator WaitDoubleClick()
        {
            var countDown = Variant.IdleTime;
            while (countDown > 0)
            {
                countDown -= Time.deltaTime;
                yield return null;
            }

            CurrentCoro = StartCoroutine(Walking());
        }
        public IEnumerator WaitChairAvailable(CustomerType type)
        {
            m_touchCount++;
            var isAvail = false;
            var countDown = Variant.IdleTime;
            while (countDown > 0)
            {
                countDown -= Time.deltaTime;
                var isChairAvailableData = IsChairAvailable(type);
                if (isChairAvailableData.isTrue)
                {
                    isAvail = true;

                    m_touchCount = 0;

                    m_touchSlider.gameObject.SetActive(false);
                    m_noticeImage.gameObject.SetActive(false);
                    OnToChair?.Invoke(isChairAvailableData.chairs, this, TargetPos);
                    break;
                }
                yield return null;
            }

            if (!isAvail)
            {

                m_noticeImage.sprite = Variant.ConfusedImage;
                m_noticeImage.preserveAspect = true;
                m_noticeImage.transform.rotation = Quaternion.identity;
                m_noticeImage.gameObject.SetActive(true);
                m_touchSlider.gameObject.SetActive(false);
                m_touchCount = 3;

                CurrentCoro = StartCoroutine(Walking());
            }

        }
    }
}


