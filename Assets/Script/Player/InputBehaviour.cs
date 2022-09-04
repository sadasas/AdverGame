using System;
using UnityEngine;

namespace AdverGame.Player
{
    public class InputBehaviour
    {
        public InputBehaviour(LayerMask customerMask)
        {
            m_clickablerMask = customerMask;
        }

        Touch m_touch;
        LayerMask m_clickablerMask;

        public Action<GameObject> OnLeftClick;



        public void Update()
        {
            DetecTouchTriggered();
        }
        private void DetecTouchTriggered()
        {


            if (Input.touchCount <= 0) return;

            m_touch = Input.GetTouch(0);

            if (m_touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(m_touch.position), Vector2.zero, m_clickablerMask);
                if (hit.collider)
                {
                    OnLeftClick?.Invoke(hit.transform.gameObject);
                }

            }


        }
    }
}


