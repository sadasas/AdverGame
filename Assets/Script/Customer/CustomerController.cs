
using AdverGame.Chair;
using AdverGame.Player;
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
        Collider2D m_collider;
        SpriteRenderer m_spriteRenderer;

        [SerializeField] Image m_noticeImage;
        [SerializeField] Canvas m_canvas;
        [SerializeField] Transform m_assPos;

        public float WidhtOffset { get; private set; }
        public float HeightOffset { get; private set; }


        [HideInInspector] public Vector3 TargetPos;
        [HideInInspector] public Vector2 DefaultPos;
        public CustomerState CurrentState = CustomerState.DEFAULT;
        public Action<CustomerController, ItemSerializable> OnCreateOrder;
        public Action<ItemSerializable, CustomerController> OnCancelOrder;
        public Action<ItemSerializable, CustomerController> OnSeeOrder;
        public Action<CustomerController> OnReset;


        public CustomerVariant Variant;
        public bool IsValid = true;
        public Coroutine CurrentCoro;
        public ChairController CurrentChair;
        public float CountDownWaitOrder;
        private void Start()
        {
            Setup();
        }


        private void OnTriggerStay2D(Collider2D collider)
        {
            ResolveOverlapOtherObject(collider);
        }

        void ResolveOverlapOtherObject(Collider2D collider)
        {
            if (CurrentState == CustomerState.DEFAULT) return;

            if (collider.CompareTag("Customer") && CurrentState != CustomerState.WAITORDER && CurrentState != CustomerState.ORDER)
            {

                var posOther = collider.transform.position.y;
                var pos = this.transform.position.y;

                var layer = pos < posOther ?
                     m_spriteRenderer.sortingOrder > collider.GetComponent<SpriteRenderer>().sortingOrder ? m_spriteRenderer.sortingOrder : collider.GetComponent<SpriteRenderer>().sortingOrder + 1 : collider.GetComponent<SpriteRenderer>().sortingOrder - 1;

                m_spriteRenderer.sortingOrder = layer;
                m_canvas.sortingOrder = layer;

            }
            if (collider.CompareTag("Dummy"))
            {

                var posOther = collider.transform.position.y;
                var pos = this.transform.position.y;

                var layer = pos < posOther ?
                     m_spriteRenderer.sortingOrder > collider.GetComponent<SpriteRenderer>().sortingOrder ? m_spriteRenderer.sortingOrder : collider.GetComponent<SpriteRenderer>().sortingOrder + 1 : collider.GetComponent<SpriteRenderer>().sortingOrder - 1;

                m_spriteRenderer.sortingOrder = layer;
                m_canvas.sortingOrder = layer;
            }
            if (collider.CompareTag("Chair") || collider.CompareTag(tag: "Table"))
            {


                if (collider.transform.position == TargetPos)
                {
                    m_spriteRenderer.sortingOrder = collider.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    m_canvas.sortingOrder = collider.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    return;
                }

                if (CurrentState == CustomerState.ORDER || CurrentState == CustomerState.WAITORDER || CurrentState == CustomerState.EAT)
                {

                    return;
                }
                var posOther = collider.transform.parent.TransformPoint(collider.bounds.center).y - collider.transform.parent.TransformPoint(collider.bounds.extents).y;
                var pos = m_collider.bounds.center.y - m_collider.bounds.extents.y;
                var layer = pos < posOther ?
                   m_spriteRenderer.sortingOrder > collider.GetComponent<SpriteRenderer>().sortingOrder ? m_spriteRenderer.sortingOrder : collider.GetComponent<SpriteRenderer>().sortingOrder + 1 : collider.GetComponent<SpriteRenderer>().sortingOrder - 1;

                m_spriteRenderer.sortingOrder = layer;
                m_canvas.sortingOrder = layer;
            }

        }
        void Setup()
        {

            m_animCharacter = GetComponent<Animator>();
            m_collider = GetComponent<Collider2D>();
            m_spriteRenderer = GetComponent<SpriteRenderer>();
            m_itemsRegistered = AssetHelpers.GetAllItemRegistered();
            WidhtOffset = GetComponent<SpriteRenderer>().bounds.size.x;
            HeightOffset = GetComponent<SpriteRenderer>().bounds.size.y / 2;



        }

        void Order()
        {

            m_animCharacter.SetBool("IsWait", true);
            m_animCharacter.SetBool("IsWalk", false);

            if (Variant.Type != CustomerType.OJOL)
            {

                var diff = Vector2.Distance(m_assPos.position, CurrentChair.CushionPos.position);
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
            if (CurrentChair) CurrentChair.Customer = null;
            CurrentChair = null;

            StopCoroutine(CurrentCoro);
            if (Variant.Type != CustomerType.OJOL) m_animCharacter.SetBool("IsEat", false);
            else m_animCharacter.SetBool("IsWait", false);
            m_animCharacter.SetBool("IsWalk", true);
            CurrentCoro = StartCoroutine(Leaving());
        }
        public void ResetPos()
        {
            m_spriteRenderer.sortingOrder = 2;
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            CurrentState = CustomerState.DEFAULT;

            transform.position = DefaultPos;

            OnReset?.Invoke(this);

            m_noticeImage.gameObject.SetActive(false);

            CountDownWaitOrder = 0;
            if (CurrentCoro != null) StopCoroutine(CurrentCoro);
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

                    OnSeeOrder.Invoke(m_currentOrder, this);
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
            CurrentState = CustomerState.WALK;
            if (distanceX < 0) transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            m_animCharacter.SetBool("IsWalk", true);
            while (!IsReachDestination())
            {
                Move();
                yield return null;
            }
            if (CurrentChair.IsLeft) transform.rotation = Quaternion.Euler(transform.rotation.x, -180, transform.rotation.z);
            else transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);

            Order();

        }

        public IEnumerator WaitOrder()
        {
            m_spriteRenderer.sortingOrder = CurrentChair.GetComponent<SpriteRenderer>().sortingOrder + 1;
            CountDownWaitOrder += Variant.WaitOrderMaxTime;
            while (CountDownWaitOrder > 0)
            {
                CountDownWaitOrder -= Time.deltaTime;

                yield return null;
            }
            m_animCharacter.SetBool("IsWait", false);
            m_animCharacter.SetBool("IsWalk", true);

            OnCancelOrder?.Invoke(m_currentOrder, this);
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
            if (CurrentChair) CurrentChair.Customer = null;
            CurrentChair = null;

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


