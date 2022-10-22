
using AdverGame.Chair;
using AdverGame.Player;
using AdverGame.UI;
using AdverGame.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Customer
{
    public class CustomerController : MonoBehaviour, ICustomer
    {
        ItemSerializable m_currentOrder;
        ChairController m_currentChair;

        int m_touchCount = 0;
        float m_countDownWaitOrder = 0;
        float m_countDownIdle = 0;
        float m_eatTime = 0;
        List<ItemSerializable> m_itemsRegistered;
        Animator m_animDummyCharacter;
        Animator m_animRealCharacter;

        [SerializeField] Slider m_touchSlider;
        [SerializeField] Image m_noticeImage;

        public GameObject DummyCharacter;
        public GameObject RealCharacter;

        public float widhtOffset { get; private set; }
        public float heightOffset { get; private set; }

        [HideInInspector] public Vector3 TargetPos;
        [HideInInspector] public Vector2 DefaultPos;
        public float SpawnDelay { get; set; } = 0;
        public CustomerState CurrentState { get; set; } = CustomerState.DEFAULT;
        public Action<CustomerController, ItemSerializable> OnCreateOrder;
        public Action<ItemSerializable> OnCancelOrder;
        public Action<CustomerController> OnResetPos;
        public Action OnToChair;
        public Action<Vector2, Vector2, CustomerController, CustomerState> OnRequestPath;
        public CustomerVariant Variant;
        public Coroutine CurrentCoro;

        private void Start()
        {

            Setup();
        }
        private void Update()
        {
            m_touchSlider.value = m_touchCount;
        }




        void Setup()
        {
            m_animDummyCharacter = DummyCharacter.GetComponent<Animator>();
            m_animRealCharacter = RealCharacter.GetComponent<Animator>();

            RealCharacter.SetActive(false);
            DummyCharacter.SetActive(true);
            m_touchSlider = DummyCharacter.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
            m_noticeImage = DummyCharacter.transform.GetChild(0).GetChild(1).GetComponent<Image>();

            m_itemsRegistered = AssetHelpers.GetAllItemRegistered();

            widhtOffset = DummyCharacter.GetComponent<SpriteRenderer>().bounds.size.x;
            heightOffset = DummyCharacter.GetComponent<SpriteRenderer>().bounds.size.y / 2;

            DefaultPos = transform.position;
            TargetPos = transform.position;
            TargetPos.x = -TargetPos.x;
            if (transform.position.x < 0) DummyCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);

            SpawnDelay += Variant.SpawnDelay;
            m_eatTime += Variant.EatTime;
            m_countDownWaitOrder = Variant.WaitOrderMaxTime;
            m_countDownIdle = Variant.WaitChairAvailableTime;

        }



        void Order()
        {
            m_animRealCharacter.speed = 0;

            var index = UnityEngine.Random.Range(0, m_itemsRegistered.Count);
            m_currentOrder = m_itemsRegistered[index];
            m_noticeImage.sprite = m_currentOrder.Content.Image;
            m_noticeImage.preserveAspect = true;
            m_noticeImage.gameObject.SetActive(true);

            OnCreateOrder?.Invoke(this, m_currentOrder);

            CurrentState = CustomerState.WAITORDER;
            CurrentCoro = StartCoroutine(WaitOrder());

        }
        bool IsChairAvailable()
        {
            var chairs = GameObject.FindGameObjectsWithTag("Chair");
            foreach (var chair in chairs)
            {
                var cc = chair.GetComponent<ChairController>();
                if (cc.Customer == null)
                {
                    cc.Customer = this;
                    m_currentChair = cc;

                    return true;
                }
            }

            return false;
        }


        bool IsReachDestination()
        {
            if (Vector2.Distance(transform.position, TargetPos) == 0) return true;
            return false;
        }
        bool IsReachDestination(Vector2 target)
        {
            if (Vector2.Distance(transform.position, target) == 0) return true;
            return false;
        }
        public void Pay()
        {
            m_noticeImage.sprite = Variant.HappyImage;
            m_noticeImage.gameObject.SetActive(true);
            TargetPos = DefaultPos;
            m_touchCount = 3;

            var distanceX = transform.position.x - TargetPos.x;
            if (distanceX < 0) RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            PlayerManager.s_Instance.IncreaseCoin(Variant.Coin);
            if (m_currentChair) m_currentChair.Customer = null;
            m_currentChair = null;
            CurrentState = CustomerState.LEAVE;

            OnRequestPath?.Invoke(transform.position, TargetPos, this, CurrentState);
        }
        public void ResetPos()
        {
            RealCharacter.SetActive(false);
            DummyCharacter.SetActive(true);
            RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
            DummyCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
            m_touchSlider = DummyCharacter.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
            m_noticeImage = DummyCharacter.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            CurrentState = CustomerState.DEFAULT;

            transform.position = DefaultPos;

            m_eatTime += Variant.EatTime;
            m_countDownWaitOrder = Variant.WaitOrderMaxTime;
            m_touchCount = 0;

            if (transform.position.x < 0) DummyCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);

            m_noticeImage.gameObject.SetActive(false);
            m_touchSlider.gameObject.SetActive(false);



        }
        public void Move()
        {

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(TargetPos.x, TargetPos.y), Variant.Speed * Time.deltaTime);
        }

        public void Move(Vector2 target)
        {

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, target.y), Variant.Speed * Time.deltaTime);
        }
        public void OnTouch(GameObject obj)
        {

            if (obj == DummyCharacter || obj == RealCharacter)
            {


                if (m_touchCount == 0 && CurrentState == CustomerState.WALK)
                {
                    StopCoroutine(CurrentCoro);
                    m_touchCount++;
                    CurrentState = CustomerState.IDLE;
                    m_touchSlider.gameObject.SetActive(true);

                    m_animDummyCharacter.speed = 0;

                    CurrentCoro = StartCoroutine(WaitDoubleClick());

                }
                else if (m_touchCount == 1 && CurrentState == CustomerState.IDLE || m_touchCount == 1 && CurrentState == CustomerState.WALK)
                {
                    StopCoroutine(CurrentCoro);
                    m_touchSlider.gameObject.SetActive(true);
                    if (IsChairAvailable())
                    {
                        m_touchSlider = RealCharacter.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
                        m_noticeImage = RealCharacter.transform.GetChild(0).GetChild(1).GetComponent<Image>();

                        m_animDummyCharacter.speed = 1;


                        DefaultPos = TargetPos;
                        TargetPos = m_currentChair.transform.position;
                        m_touchCount = 0;
                        m_noticeImage.gameObject.SetActive(false);
                        m_touchSlider.gameObject.SetActive(false);
                        OnToChair?.Invoke();

                        DummyCharacter.SetActive(false);
                        RealCharacter.SetActive(true);
                        var distanceX = transform.position.x - TargetPos.x;

                        if (distanceX < 0) RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
                        else RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

                        CurrentState = CustomerState.TOCHAIR;
                        OnRequestPath?.Invoke(transform.position, TargetPos, this, CurrentState);
                    }
                    else
                    {
                        m_touchCount++;
                        CurrentState = CustomerState.WAITCHAIRAVAILABLE;
                        CurrentCoro = StartCoroutine(WaitChairAvailable());
                    }
                }
                else if (CurrentState == CustomerState.WAITORDER)
                {
                    UIManager.s_Instance.ForceHUD(HUDName.ITEM_AVAILABLE);
                }



            }

        }
        public void ResetOrder()
        {
            if (m_currentOrder == null) return;
            m_currentOrder = null;
            m_noticeImage.gameObject.SetActive(false);


        }

        public IEnumerator WaitDoubleClick()
        {
            while (m_countDownIdle > 0)
            {
                m_countDownIdle -= Time.deltaTime;
                yield return null;
            }
            m_animDummyCharacter.speed = 1;
            CurrentState = CustomerState.WALK;
            m_countDownIdle = Variant.WaitChairAvailableTime;
            CurrentCoro = StartCoroutine(walking());
        }

        public IEnumerator WaitChairAvailable()
        {
            while (m_countDownIdle > 0)
            {
                m_countDownIdle -= Time.deltaTime;
                if (IsChairAvailable())
                {
                    m_animDummyCharacter.speed = 1;

                    DummyCharacter.SetActive(false);
                    RealCharacter.SetActive(true);
                    m_touchSlider = RealCharacter.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
                    m_noticeImage = RealCharacter.transform.GetChild(0).GetChild(1).GetComponent<Image>();

                    TargetPos = m_currentChair.transform.position;
                    m_touchCount = 0;
                    var distanceX = transform.position.x - TargetPos.x;

                    if (distanceX < 0) RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);

                    m_touchSlider.gameObject.SetActive(false);
                    m_noticeImage.gameObject.SetActive(false);
                    OnToChair?.Invoke();
                    CurrentState = CustomerState.TOCHAIR;
                    OnRequestPath?.Invoke(transform.position, TargetPos, this, CurrentState);

                }
                yield return null;
            }

            m_animDummyCharacter.speed = 1;
            m_noticeImage.sprite = Variant.ConfusedImage;
            m_noticeImage.preserveAspect = true;
            m_noticeImage.transform.rotation = Quaternion.identity;
            m_noticeImage.gameObject.SetActive(true);
            CurrentState = CustomerState.WALK;
            m_countDownIdle = Variant.WaitChairAvailableTime;
            m_touchSlider.gameObject.SetActive(false);
            m_touchCount = 3;

            CurrentCoro = StartCoroutine(walking());
        }
        public IEnumerator ToChair(Vector2[] path)
        {
            for (int i = 0; i < path.Length; i++)
            {
                while (!IsReachDestination(path[i]))
                {
                    Move(path[i]);
                    yield return null;
                }

            }
            Order();

        }

        public IEnumerator WaitOrder()
        {
            while (m_countDownWaitOrder > 0)
            {
                m_countDownWaitOrder -= Time.deltaTime;
                yield return null;

            }
            m_animRealCharacter.speed = 1;
            OnCancelOrder?.Invoke(m_currentOrder);
            ResetOrder();
            m_noticeImage.sprite = Variant.AngryImage;
            m_noticeImage.gameObject.SetActive(true);
            TargetPos = DefaultPos;
            m_touchCount = 3;
            CurrentState = CustomerState.LEAVE;


            var distanceX = transform.position.x - TargetPos.x;

            if (distanceX < 0) RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            OnRequestPath?.Invoke(transform.position, TargetPos, this, CurrentState);

        }

        public IEnumerator Eating()
        {
            m_animRealCharacter.speed = 1;
            while (m_eatTime > 0)
            {
                m_animRealCharacter.SetBool("isEat", true);
                m_eatTime -= Time.deltaTime;
                m_noticeImage.sprite = Variant.EatImage;
                m_noticeImage.gameObject.SetActive(true);
                yield return null;
            }
            CurrentState = CustomerState.PAY;
            m_animRealCharacter.SetBool("isEat", false);
            m_eatTime = 0;
            m_noticeImage.gameObject.SetActive(false);
            Pay();
        }
        public IEnumerator Leaving()
        {
            if (m_currentChair) m_currentChair.Customer = null;
            m_currentChair = null;

            while (!IsReachDestination(TargetPos))
            {
                Move();
                yield return null;
            }

            OnResetPos.Invoke(this);
        }

        public IEnumerator walking()
        {
            while (SpawnDelay > 0)
            {
                SpawnDelay -= Time.deltaTime;
                yield return null;
            }
            while (CurrentState == CustomerState.WALK && SpawnDelay <= 0 && !IsReachDestination())
            {

                Move();
                yield return null;
            }
            OnResetPos.Invoke(this);
        }

    }
}


