
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

    /// <summary>
    /// TODO: when  customers each other overlap make customer closest to the camera is in front
    /// </summary>
    public class CustomerController : MonoBehaviour
    {
        ItemSerializable m_currentOrder;



        List<ItemSerializable> m_itemsRegistered;

        Animator m_animCharacter;


        [SerializeField] Image m_noticeImage;
        [SerializeField] Transform m_assPos;

        public float widhtOffset { get; private set; }
        public float heightOffset { get; private set; }


        [HideInInspector] public Vector3 TargetPos;
        [HideInInspector] public Vector2 DefaultPos;
        public CustomerState CurrentState { get; set; } = CustomerState.DEFAULT;
        public Action<CustomerController, ItemSerializable> OnCreateOrder;
        public Action<ItemSerializable> OnCancelOrder;


        public CustomerVariant Variant;
        public bool isValid = true;
        public Coroutine CurrentCoro;
        public ChairController m_currentChair;

        private void Start()
        {

            Setup();
        }


        void Setup()
        {

            m_animCharacter = GetComponent<Animator>();



            m_itemsRegistered = AssetHelpers.GetAllItemRegistered();

            widhtOffset = GetComponent<SpriteRenderer>().bounds.size.x;
            heightOffset = GetComponent<SpriteRenderer>().bounds.size.y / 2;



        }

        void Order()
        {

            m_animCharacter.SetBool("IsWait", true);
            m_animCharacter.SetBool("IsWalk", false);

            if (Variant.Type != CustomerType.OJOL)
            {

                var diff = Vector2.Distance(m_assPos.position, m_currentChair.CushionPos.position);
                var sitPos = new Vector2(transform.position.x, transform.position.y + diff);
                transform.position = sitPos;
            }

            var index = UnityEngine.Random.Range(0, m_itemsRegistered.Count);
            m_currentOrder = m_itemsRegistered[index];
            m_noticeImage.sprite = m_currentOrder.Content.Image;
            m_noticeImage.preserveAspect = true;
            m_noticeImage.gameObject.SetActive(true);

            OnCreateOrder?.Invoke(this, m_currentOrder);

            CurrentState = CustomerState.WAITORDER;
            CurrentCoro = StartCoroutine(WaitOrder());
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

            CurrentState = CustomerState.LEAVE;

            var distanceX = transform.position.x - TargetPos.x;
            if (distanceX < 0) transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            PlayerManager.s_Instance.IncreaseCoin(Variant.Coin);
            PlayerManager.s_Instance.IncreaseExp(Variant.Exp);
            if (m_currentChair) m_currentChair.Customer = null;
            m_currentChair = null;

            StopCoroutine(CurrentCoro);
            if (Variant.Type != CustomerType.OJOL) m_animCharacter.SetBool("IsEat", false);
            else m_animCharacter.SetBool("IsWait", false);
            m_animCharacter.SetBool("IsWalk", true);
            CurrentCoro = StartCoroutine(Leaving());
        }
        public void ResetPos()
        {

            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            CurrentState = CustomerState.DEFAULT;

            transform.position = DefaultPos;



            m_noticeImage.gameObject.SetActive(false);



            if (Variant.Type != CustomerType.OJOL) m_animCharacter.SetBool("IsEat", false);
            m_animCharacter.SetBool("IsWait", false);
            m_animCharacter.SetBool("IsWalk", false);
        }
        public void Move()
        {

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(TargetPos.x, TargetPos.y), Variant.Speed * Time.deltaTime);
        }
        public void OnTouch(GameObject obj)
        {

            if (obj == this.gameObject)
            {
                if (CurrentState == CustomerState.WAITORDER)
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



        public IEnumerator ToChair()
        {
            m_animCharacter ??= GetComponent<Animator>();
            var distanceX = transform.position.x - TargetPos.x;

            if (distanceX < 0) transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            m_animCharacter.SetBool("IsWalk", true);
            while (!IsReachDestination())
            {
                Move();
                yield return null;
            }
            if (m_currentChair.IsLeft) transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            Order();

        }

        public IEnumerator WaitOrder()
        {
            var m_countDownWaitOrder = Variant.WaitOrderMaxTime;
            while (m_countDownWaitOrder > 0)
            {
                m_countDownWaitOrder -= Time.deltaTime;

                yield return null;
            }
            m_animCharacter.SetBool("IsWait", false);
            m_animCharacter.SetBool("IsWalk", true);

            OnCancelOrder?.Invoke(m_currentOrder);
            ResetOrder();
            m_noticeImage.sprite = Variant.AngryImage;
            m_noticeImage.gameObject.SetActive(true);
            TargetPos = DefaultPos;

            CurrentState = CustomerState.LEAVE;


            var distanceX = transform.position.x - TargetPos.x;

            if (distanceX < 0) transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            CurrentCoro = StartCoroutine(Leaving());

        }

        public IEnumerator Leaving()
        {
            if (m_currentChair) m_currentChair.Customer = null;
            m_currentChair = null;

            while (!IsReachDestination())
            {
                Move();
                yield return null;
            }
            ResetPos();


        }

        public IEnumerator Eating()
        {
            m_animCharacter.speed = 1;
            m_animCharacter.SetBool("IsEat", true);
            m_animCharacter.SetBool("IsWait", false);

            m_noticeImage.sprite = Variant.EatImage;
            m_noticeImage.gameObject.SetActive(true);

            var m_eatTime = Variant.EatTime;
            while (m_eatTime > 0)
            {

                m_eatTime -= Time.deltaTime;

                yield return null;
            }
            m_animCharacter.SetBool("IsEat", false);

            CurrentState = CustomerState.PAY;

            m_eatTime = 0;
            m_noticeImage.gameObject.SetActive(false);
            Pay();



        }


    }
}


