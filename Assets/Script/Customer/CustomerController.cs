
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
        SpriteRenderer m_sprite;
        int m_touchCount = 0;
        int m_countDownMove = 0;
        float m_countDownWaitOrder = 0;
        float m_countDownIdle = 0;
        float m_eatTime = 0;
        List<ItemSerializable> m_itemsRegistered;

        [SerializeField] Slider m_touchSlider;
        [SerializeField] Image m_noticeImage;
        [SerializeField] CustomerVariant m_variant;

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
            if (m_eatTime > 0)
            {
                m_eatTime -= Time.deltaTime;
                m_noticeImage.sprite = m_variant.EatImage;
                m_noticeImage.gameObject.SetActive(true);

            }
            else if (m_eatTime <= 0)
            {
                CurrentState = CustomerState.PAY;
                m_eatTime = 0;
                m_noticeImage.gameObject.SetActive(false);
            }
        }
        void Setup()
        {
            m_sprite = GetComponent<SpriteRenderer>();
            m_itemsRegistered = AssetHelpers.GetAllItemRegistered();

            widhtOffset = GetComponent<SpriteRenderer>().bounds.size.x;
            heightOffset = GetComponent<SpriteRenderer>().bounds.size.y / 2;

            DefaultPos = transform.position;
            TargetPos = transform.position;
            TargetPos.x = -TargetPos.x;

            SpawnDelay += m_variant.SpawnDelay;
            m_eatTime += m_variant.EatTime;
            m_countDownMove = m_variant.SpawnDelay;
            m_countDownWaitOrder = m_variant.WaitOrderMaxTime;
            m_countDownIdle = m_variant.WaitChairAvailableTime;

            ChangeSprite(m_variant.DummylCustomerImage);


        }
        void ChangeSprite(Sprite image)
        {
            m_sprite.sprite = image;

            m_sprite.sortingOrder = 1;
        }
        void WaitTofindChair()
        {
            if (m_countDownIdle > 0)
            {
                m_countDownIdle -= Time.deltaTime;

            }
            else
            {
                CurrentState = CustomerState.WALK;
                m_countDownIdle = m_variant.WaitChairAvailableTime;
            }
        }
        void WaitChairAvailable()
        {
            if (m_countDownIdle > 0)
            {
                m_countDownIdle -= Time.deltaTime;
                if (IsChairAvailable())
                {

                    CurrentState = CustomerState.TOCHAIR;
                    TargetPos = m_currentChair.transform.position;
                    m_touchCount = 0;

                    ChangeSprite(m_variant.RealCustomerImage);
                    m_touchSlider.gameObject.SetActive(false);
                    OnToChair?.Invoke();
                }
            }
            else
            {
                m_noticeImage.sprite = m_variant.AngryImage;
                m_noticeImage.gameObject.SetActive(true);
                CurrentState = CustomerState.WALK;
                m_countDownIdle = m_variant.WaitChairAvailableTime;
                m_touchSlider.gameObject.SetActive(false);
                m_touchCount = 3;
            }
        }
        void Order()
        {
            var index = UnityEngine.Random.Range(0, m_itemsRegistered.Count);
            m_currentOrder = m_itemsRegistered[index];
            m_noticeImage.sprite = m_currentOrder.Content.Image;
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

                OnCancelOrder?.Invoke(m_currentOrder);
                ResetOrder();
                m_noticeImage.sprite = m_variant.AngryImage;
                m_noticeImage.gameObject.SetActive(true);
                TargetPos = DefaultPos;
                m_touchCount = 3;
                CurrentState = CustomerState.LEAVE;
                if (m_currentChair) m_currentChair.Customer = null;
                m_currentChair = null;
            }
        }
        bool IsReachDestination()
        {
            if (Vector2.Distance(transform.position, TargetPos) == 0) return true;
            return false;
        }
        public void Pay()
        {
            m_noticeImage.sprite = m_variant.HappyImage;
            m_noticeImage.gameObject.SetActive(true);
            TargetPos = DefaultPos;
            m_touchCount = 3;
            CurrentState = CustomerState.LEAVE;
            PlayerManager.s_Instance.IncreaseCoin(m_variant.Coin);
            if (m_currentChair) m_currentChair.Customer = null;
            m_currentChair = null;
        }
        public void ResetPos()
        {

            CurrentState = CustomerState.DEFAULT;

            transform.position = DefaultPos;

            m_eatTime += m_variant.EatTime;
            m_countDownMove = m_variant.SpawnDelay;
            m_countDownWaitOrder = m_variant.WaitOrderMaxTime;
            m_touchCount = 0;



            m_noticeImage.gameObject.SetActive(false);
            m_touchSlider.gameObject.SetActive(false);

            ChangeSprite(m_variant.DummylCustomerImage);


        }
        public void Move()
        {

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(TargetPos.x, TargetPos.y), m_variant.Speed * Time.deltaTime);
        }
        public void OnTouch(GameObject obj)
        {

            if (obj == this.gameObject)
            {



                if (m_touchCount == 0 && CurrentState == CustomerState.WALK)
                {
                    m_touchCount++;
                    CurrentState = CustomerState.IDLE;
                    m_touchSlider.gameObject.SetActive(true);

                }
                else if (m_touchCount == 1 && CurrentState == CustomerState.IDLE || m_touchCount == 1 && CurrentState == CustomerState.WALK)
                {
                    m_touchSlider.gameObject.SetActive(true);
                    if (IsChairAvailable())
                    {

                        CurrentState = CustomerState.TOCHAIR;


                        ChangeSprite(m_variant.RealCustomerImage);
                        DefaultPos = TargetPos;
                        TargetPos = m_currentChair.transform.position;
                        m_touchSlider.gameObject.SetActive(false);
                        m_touchCount = 0;
                        OnToChair?.Invoke();
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


