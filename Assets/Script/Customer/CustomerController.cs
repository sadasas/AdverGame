
using AdverGame.Chair;
using AdverGame.Player;
using AdverGame.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdverGame.Customer
{

    enum CustomerState
    {
        IDLE,
        WALK,
        TOCHAIR,
        WAITCHAIRAVAILABLE,
        ORDER,
        WAITORDER,
        DEFAULT
    }
    public class CustomerController : MonoBehaviour, ICustomer
    {

        Vector3 m_targetPos;
        Vector2 m_defaultPos;
        float m_widhtOffset;
        float m_heightOffset;
        [SerializeField] CustomerState m_currentState = CustomerState.DEFAULT;
        ChairController m_currentChair;
        int m_touchCount = 0;
        List<ItemSerializable> m_itemsRegistered;
        int m_countDownMove = 0;
        float m_countDownWaitOrder = 0;
        float m_countDownIdle = 0;
        SpriteRenderer m_sprite;
        [SerializeField] Slider m_touchSlider;
        [SerializeField] Image m_orderImage;
        [SerializeField] CustomerVariant m_variant;


        private void Start()
        {
            StartCoroutine(Setup());

        }


        private void Update()
        {


            if (m_currentState == CustomerState.WALK) Move();
            if (m_currentState == CustomerState.WALK && IsReachDestination()) StartCoroutine(ResetPos());

            if (m_currentState == CustomerState.TOCHAIR) Move();
            if (m_currentState == CustomerState.TOCHAIR && IsReachDestination())
            {
                Order();
                m_currentState = CustomerState.WAITORDER;
            }
            if (m_currentState == CustomerState.WAITORDER) WaitOrder();

            if (m_currentState == CustomerState.WAITCHAIRAVAILABLE) WaitChairAvailable();
            if (m_currentState == CustomerState.IDLE) WaitTofindChair();

            m_touchSlider.value = m_touchCount;
        }


        IEnumerator Setup()
        {
            m_sprite = GetComponent<SpriteRenderer>();
            m_itemsRegistered = AssetHelpers.GetAllItemRegistered();

            m_widhtOffset = GetComponent<SpriteRenderer>().bounds.size.x;
            m_heightOffset = GetComponent<SpriteRenderer>().bounds.size.y / 2;

            yield return transform.position = SetRandomPos();

            m_defaultPos = transform.position;
            m_targetPos = transform.position;
            m_targetPos.x = -m_targetPos.x;

            m_countDownMove = m_variant.SpawnDelay;
            m_countDownWaitOrder = m_variant.WaitOrderMaxTime;
            m_countDownIdle = m_variant.WaitChairAvailableTime;

            ChangeSprite(m_variant.DummylCustomerImage);

            m_currentState = CustomerState.WALK;

        }

        private void ChangeSprite(Sprite image)
        {
            m_sprite.sprite = image;
            m_sprite.drawMode = SpriteDrawMode.Sliced;
            m_sprite.size = new Vector2(1, 1);
        }

        Vector2 SetRandomPos()
        {
            var stageDimension = Camera.main.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
            return new Vector2(stageDimension.x + m_widhtOffset, UnityEngine.Random.Range(-3, -(stageDimension.y - m_heightOffset)));
        }
        public void Move()
        {

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(m_targetPos.x, m_targetPos.y), m_variant.Speed * Time.deltaTime);
        }

        public void OnTouch(GameObject obj)
        {

            if (obj == this.gameObject)
            {



                if (m_touchCount == 0 && m_currentState == CustomerState.WALK)
                {
                    m_touchCount++;
                    m_currentState = CustomerState.IDLE;
                    m_touchSlider.gameObject.SetActive(true);

                }
                else if (m_touchCount == 1 && m_currentState == CustomerState.IDLE || m_touchCount == 1 && m_currentState == CustomerState.WALK)
                {
                    m_touchSlider.gameObject.SetActive(true);
                    if (IsChairAvailable())
                    {
                        m_currentState = CustomerState.TOCHAIR;

                        ChangeSprite(m_variant.RealCustomerImage);
                        m_targetPos = m_currentChair.transform.position;
                        m_touchSlider.gameObject.SetActive(false);
                        m_touchCount = 0;
                    }
                    else
                    {
                        m_touchCount++;
                        m_currentState = CustomerState.WAITCHAIRAVAILABLE;

                    }
                }




            }

        }

        void WaitTofindChair()
        {
            if (m_countDownIdle > 0)
            {
                m_countDownIdle -= Time.deltaTime;

            }
            else
            {
                m_currentState = CustomerState.WALK;
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
                    m_currentState = CustomerState.TOCHAIR;
                    m_targetPos = m_currentChair.transform.position;
                    m_touchCount = 0;

                    ChangeSprite(m_variant.RealCustomerImage);
                    m_touchSlider.gameObject.SetActive(false);
                }
            }
            else
            {
                m_currentState = CustomerState.WALK;
                m_countDownIdle = m_variant.WaitChairAvailableTime;

                m_touchCount--;
            }
        }
        void Order()
        {
            var index = UnityEngine.Random.Range(0, m_itemsRegistered.Count);
            var item = m_itemsRegistered[index];
            m_orderImage.sprite = item.Content.Image;
            m_orderImage.gameObject.SetActive(true);

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

                StartCoroutine(ResetPos());
            }
        }

        private void IncreaseCoin()
        {
            //PlayerManager.s_Instance.IncreaseCoin(m_variant.Coin);
        }

        bool IsReachDestination()
        {


            if (Vector2.Distance(transform.position, m_targetPos) == 0) return true;
            return false;
        }

        IEnumerator ResetPos()
        {
            Debug.Log("Reset");
            m_countDownMove = m_variant.SpawnDelay;
            yield return m_defaultPos = SetRandomPos();

            transform.position = m_defaultPos;
            m_targetPos = transform.position;
            m_targetPos.x = -m_targetPos.x;
            m_countDownMove = m_variant.SpawnDelay;
            m_countDownWaitOrder = m_variant.WaitOrderMaxTime;
            m_touchCount = 0;

            if (m_currentChair) m_currentChair.Customer = null;
            m_currentChair = null;

            m_touchSlider.gameObject.SetActive(false);
            m_orderImage.gameObject.SetActive(false);


            ChangeSprite(m_variant.DummylCustomerImage);
            m_currentState = CustomerState.WALK;
        }

    }
}


