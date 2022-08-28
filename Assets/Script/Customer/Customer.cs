
using AdverGame.Chair;
using AdverGame.Player;
using System.Collections;
using UnityEngine;



namespace AdverGame.Customer
{

    enum CustomerState
    {
        IDLE,
        ORDER,
        TOCHAIR,
        WAITORDER,
        DEFAULT
    }
    public class Customer : MonoBehaviour, ICustomer
    {

        Vector3 m_targetPos;
        Vector2 m_defaultPos;
        float m_widhtOffset;
        float m_heightOffset;
        CustomerState m_currentState = CustomerState.DEFAULT;
        ChairController m_currentChair;

        [SerializeField] int m_countDownMove = 0;
        [SerializeField] float m_countDownWaitOrder = 0;

        [SerializeField] CustomerVariant m_variant;


        private void Start()
        {
            StartCoroutine(Setup());

        }


        private void Update()
        {
            if (m_countDownMove > 0) m_countDownMove--;
            if (m_currentState == CustomerState.IDLE && m_countDownMove == 0) Move();
            if (m_currentState == CustomerState.ORDER) Move();
            if (m_currentState == CustomerState.TOCHAIR) Move();
            if (m_currentState == CustomerState.WAITORDER) WaitOrder();

            if (m_currentState == CustomerState.IDLE && IsReachDestination()) StartCoroutine(ResetPos());
            if (m_currentState == CustomerState.ORDER && IsReachDestination())
            {
                m_targetPos = m_currentChair.transform.position;
                m_currentState = CustomerState.TOCHAIR;

            }
            if (m_currentState == CustomerState.TOCHAIR && IsReachDestination())
            {
                m_currentState = CustomerState.WAITORDER;
            }

        }


        IEnumerator Setup()
        {
            m_widhtOffset = GetComponent<SpriteRenderer>().bounds.size.x;
            m_heightOffset = GetComponent<SpriteRenderer>().bounds.size.y / 2;

            yield return transform.position = SetRandomPos();

            m_defaultPos = transform.position;
            m_targetPos = transform.position;
            m_targetPos.x = -m_targetPos.x;

            m_countDownMove = m_variant.SpawnDelay;
            m_countDownWaitOrder = m_variant.WaitOrderMaxTime;

            m_currentState = CustomerState.IDLE;

        }

        Vector2 SetRandomPos()
        {
            var stageDimension = Camera.main.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
            return new Vector2(stageDimension.x + m_widhtOffset, UnityEngine.Random.Range(-2, -(stageDimension.y - m_heightOffset)));
        }
        public void Move()
        {


            transform.position = Vector3.MoveTowards(transform.position, new Vector3(m_targetPos.x, m_targetPos.y), m_variant.Speed * Time.deltaTime);
        }

        public void OnTouch()
        {
            if (m_currentState == CustomerState.IDLE)
            {
                if (IsChairAvailable())
                {
                    m_currentState = CustomerState.ORDER;
                    Order();
                }

            }
        }

        void Order()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            m_targetPos = player.transform.position;
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
            if (m_countDownWaitOrder > 0) m_countDownWaitOrder -= Time.deltaTime;
            else
            {
                IncreaseCoin();
                StartCoroutine(ResetPos());
            }
        }

        private void IncreaseCoin()
        {
            PlayerManager.s_Instance.IncreaseCoin(m_variant.Coin);
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
            if(m_currentChair)m_currentChair.Customer = null;
            m_currentChair = null;
            m_currentState = CustomerState.IDLE;
        }

    }
}


