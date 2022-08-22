using AdverGame.Customer;
using UnityEngine;

namespace AdverGame.Player
{
    public class InputBehaviour
    {
        Touch m_touch;
        LayerMask m_clickablerMask;


        public InputBehaviour(LayerMask customerMask)
        {
            m_clickablerMask = customerMask;
        }

        public void Update()
        {
            DetecTouchTriggered();
        }

        private void DetecTouchTriggered()
        {

#if UNITY_STANDALONE_WIN

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.down, 2f, m_clickablerMask);

                if (hit.collider && hit.collider.CompareTag("Customer"))
                {
                    hit.transform.GetComponent<ICustomer>().OnTouch();
                }
            }

#elif UNITY_ANDROID

        if (Input.touchCount <= 0) return;

        m_touch = Input.GetTouch(0);
        Debug.Log("toc");
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(m_touch.position), Vector2.down, 2f, m_clickablerMask);
        if (hit.collider && hit.collider.CompareTag("Customer"))
        {
            hit.transform.GetComponent<ICustomer>().OnTouch();
        }
#endif
        }
    }
}


