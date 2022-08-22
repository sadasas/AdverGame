
using AdverGame.Player;
using UnityEngine;



namespace AdverGame.Customer
{
    public class Customer : MonoBehaviour, ICustomer
    {
        bool m_isMove = false;
        Vector3 m_targetPos;
        Vector2 m_defaultPos;

        [SerializeField] CustomerVariant m_variant;

        private void Start()
        {
            m_defaultPos = transform.position;
            m_targetPos = transform.position;
            m_targetPos.x = -m_targetPos.x;

            m_isMove = true;
        }


        private void FixedUpdate()
        {

            Move();
            if (IsReachDestination()) ResetPos();


        }
        public void Move()
        {
            if (!m_isMove) return;

            transform.position = Vector3.MoveTowards(transform.position, new Vector3(m_targetPos.x, transform.position.y), m_variant.Speed * Time.deltaTime);
        }

        public void OnTouch()
        {
            IncreaseCoin();
            ResetPos();
        }

        private void IncreaseCoin()
        {
            PlayerManager.s_Instance.IncreaseCoin(m_variant.Coin);
        }

        bool IsReachDestination()
        {
            if (!m_isMove) return false;
            if (Vector2.Distance(transform.position, m_targetPos) < 1) return true;
            return false;
        }

        void ResetPos()
        {
            Debug.Log("Reset");

            transform.position = m_defaultPos;
            //m_isMove = false;
        }

    }
}

