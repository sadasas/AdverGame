
using AdverGame.Chair;
using AdverGame.Player;
using AdverGame.UI;
using AdverGame.Utility;
using System;
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
        public CustomerVariant Variant;


        private void Start()
        {

            Setup();
        }
        private void Update()
        {
            if (CurrentState == CustomerState.WALK)
            {
                if (SpawnDelay > 0) SpawnDelay -= Time.deltaTime;
                else
                {
                    SpawnDelay = 0;
                    Move();
                }
            }
            if (CurrentState == CustomerState.WALK && IsReachDestination()) OnResetPos.Invoke(this);
            if (CurrentState == CustomerState.LEAVE) Move();
            if (CurrentState == CustomerState.LEAVE && IsReachDestination()) OnResetPos.Invoke(this);
            if (CurrentState == CustomerState.TOCHAIR) Move();
            if (CurrentState == CustomerState.TOCHAIR && IsReachDestination())
            {
                Order();
                CurrentState = CustomerState.WAITORDER;
            }
            if (CurrentState == CustomerState.WAITORDER) WaitOrder();
            if (CurrentState == CustomerState.WAITCHAIRAVAILABLE) WaitChairAvailable();
            if (CurrentState == CustomerState.IDLE) WaitTofindChair();
            if (CurrentState == CustomerState.EAT) Eating();
            if (CurrentState == CustomerState.PAY) Pay();
            m_touchSlider.value = m_touchCount;
        }


        void Eating()
        {
            m_animRealCharacter.speed = 1;
            if (m_eatTime > 0)
            {
                m_animRealCharacter.SetBool("IsEat", true);
                m_animRealCharacter.SetBool("IsWait", false);
                m_animRealCharacter.SetBool("IsWalk", false);
                m_eatTime -= Time.deltaTime;
                m_noticeImage.sprite = Variant.EatImage;
                m_noticeImage.gameObject.SetActive(true);

            }
            else if (m_eatTime <= 0)
            {
                m_animRealCharacter.SetBool("IsEat", false);
                m_animRealCharacter.SetBool("IsWait", false);
                m_animRealCharacter.SetBool("IsWalk", false);
                CurrentState = CustomerState.PAY;
                m_eatTime = 0;
                m_noticeImage.gameObject.SetActive(false);
            }
        }
        void Setup()
        {
            m_animDummyCharacter = DummyCharacter.GetComponent<Animator>();
            m_animRealCharacter = RealCharacter.GetComponent<Animator>();

            m_animRealCharacter.SetBool("IsEat", false);
            m_animRealCharacter.SetBool("IsWait", false);
            m_animRealCharacter.SetBool("IsWalk", false);
         

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

        void WaitTofindChair()
        {
            if (m_countDownIdle > 0)
            {
                m_countDownIdle -= Time.deltaTime;

            }
            else
            {
                m_animDummyCharacter.speed = 1;
                CurrentState = CustomerState.WALK;
                m_countDownIdle = Variant.WaitChairAvailableTime;
            }
        }
        void WaitChairAvailable()
        {
            if (m_countDownIdle > 0)
            {
                m_countDownIdle -= Time.deltaTime;
                if (IsChairAvailable())
                {
                    m_animDummyCharacter.speed = 1;

                    DummyCharacter.SetActive(false);
                    RealCharacter.SetActive(true);
                    m_touchSlider = RealCharacter.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
                    m_noticeImage = RealCharacter.transform.GetChild(0).GetChild(1).GetComponent<Image>();

                    CurrentState = CustomerState.TOCHAIR;
                    TargetPos = m_currentChair.transform.position;
                    m_touchCount = 0;
                    var distanceX = transform.position.x - TargetPos.x;

                    if (distanceX < 0) RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);

                    m_touchSlider.gameObject.SetActive(false);
                    m_noticeImage.gameObject.SetActive(false);
                    OnToChair?.Invoke();
                }
            }
            else
            {
                m_animDummyCharacter.speed = 1;
                m_noticeImage.sprite = Variant.ConfusedImage;
                m_noticeImage.preserveAspect = true;
                m_noticeImage.transform.rotation = Quaternion.identity;
                m_noticeImage.gameObject.SetActive(true);
                CurrentState = CustomerState.WALK;
                m_countDownIdle = Variant.WaitChairAvailableTime;
                m_touchSlider.gameObject.SetActive(false);
                m_touchCount = 3;
            }
        }
        void Order()
        {
            m_animRealCharacter.SetBool("IsEat", false);
            m_animRealCharacter.SetBool("IsWait", true);
            m_animRealCharacter.SetBool("IsWalk", false);

            var index = UnityEngine.Random.Range(0, m_itemsRegistered.Count);
            m_currentOrder = m_itemsRegistered[index];
            m_noticeImage.sprite = m_currentOrder.Content.Image;
            m_noticeImage.preserveAspect = true;
            m_noticeImage.gameObject.SetActive(true);

            OnCreateOrder?.Invoke(this, m_currentOrder);

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
        void WaitOrder()
        {

            if (m_countDownWaitOrder > 0)
            {
                m_countDownWaitOrder -= Time.deltaTime;
            }
            else
            {

                m_animRealCharacter.SetBool("IsWait", false);
                m_animRealCharacter.SetBool("IsWalk", true);
                OnCancelOrder?.Invoke(m_currentOrder);
                ResetOrder();
                m_noticeImage.sprite = Variant.AngryImage;
                m_noticeImage.gameObject.SetActive(true);
                TargetPos = DefaultPos;
                m_touchCount = 3;
                CurrentState = CustomerState.LEAVE;
                if (m_currentChair) m_currentChair.Customer = null;
                m_currentChair = null;

                var distanceX = transform.position.x - TargetPos.x;

                if (distanceX < 0) RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
                else RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
            }
        }
        bool IsReachDestination()
        {
            if (Vector2.Distance(transform.position, TargetPos) == 0) return true;
            return false;
        }
        public void Pay()
        {
            m_noticeImage.sprite = Variant.HappyImage;
            m_noticeImage.gameObject.SetActive(true);
            TargetPos = DefaultPos;
            m_touchCount = 3;
            CurrentState = CustomerState.LEAVE;

            var distanceX = transform.position.x - TargetPos.x;
            if (distanceX < 0) RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            PlayerManager.s_Instance.IncreaseCoin(Variant.Coin);
            if (m_currentChair) m_currentChair.Customer = null;
            m_currentChair = null;

            m_animRealCharacter.SetBool("IsEat", false);
            m_animRealCharacter.SetBool("IsWalk", true);
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

            m_animDummyCharacter.SetBool("IsWalk", false);
            m_animRealCharacter.SetBool("IsEat", false);
            m_animRealCharacter.SetBool("IsWait", false);
            m_animRealCharacter.SetBool("IsWalk", false);
        }
        public void Move()
        {

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(TargetPos.x, TargetPos.y), Variant.Speed * Time.deltaTime);
        }
        public void DummyWalking()
        {
            m_animDummyCharacter ??= DummyCharacter.GetComponent<Animator>();
            m_animDummyCharacter.SetBool("IsWalk", true);
        }


        public void OnTouch(GameObject obj)
        {

            if (obj == DummyCharacter || obj == RealCharacter)
            {



                if (m_touchCount == 0 && CurrentState == CustomerState.WALK)
                {
                    m_touchCount++;
                    CurrentState = CustomerState.IDLE;
                    m_touchSlider.gameObject.SetActive(true);

                    m_animDummyCharacter.speed = 0;

                }
                else if (m_touchCount == 1 && CurrentState == CustomerState.IDLE || m_touchCount == 1 && CurrentState == CustomerState.WALK)
                {
                    m_touchSlider.gameObject.SetActive(true);
                    if (IsChairAvailable())
                    {
                        m_touchSlider = RealCharacter.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
                        m_noticeImage = RealCharacter.transform.GetChild(0).GetChild(1).GetComponent<Image>();

                        m_animDummyCharacter.speed = 1;

                        CurrentState = CustomerState.TOCHAIR;

                        DefaultPos = TargetPos;
                        TargetPos = m_currentChair.transform.position;
                        m_touchCount = 0;
                        m_noticeImage.gameObject.SetActive(false);
                        m_touchSlider.gameObject.SetActive(false);
                        OnToChair?.Invoke();


                        DummyCharacter.SetActive(false);
                        RealCharacter.SetActive(true);
                        var distanceX = transform.position.x - TargetPos.x;
                        m_animRealCharacter.SetBool("IsEat", false);
                        m_animRealCharacter.SetBool("IsWait", false);
                        m_animRealCharacter.SetBool("IsWalk", true);
                        if (distanceX < 0) RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
                        else RealCharacter.transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

                    }
                    else
                    {
                        m_touchCount++;
                        CurrentState = CustomerState.WAITCHAIRAVAILABLE;

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
    }
}


