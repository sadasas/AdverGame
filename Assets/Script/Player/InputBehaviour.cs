using System;
using UnityEngine;
using UnityEngine.EventSystems;

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

        public bool IsDrag;
        public bool isAllowSipeOverUI = false;
        public Action<GameObject> OnLeftClick;


        public Action<float> OnLeftEndDrag;
        [SerializeField] Vector2 m_startPos;


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
                if (hit.collider)
                {
                    OnLeftClick?.Invoke(hit.transform.gameObject);
                }
            }
#elif UNITY_ANDROID
            if (Input.touchCount <= 0) return;

            m_touch = Input.GetTouch(0);

            if (EventSystem.current.IsPointerOverGameObject(m_touch.fingerId) == false && m_touch.phase == TouchPhase.Began)
            {
                m_startPos = m_touch.position;
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(m_touch.position), Vector2.zero, m_clickablerMask);

                if (hit.collider && EventSystem.current.IsPointerOverGameObject() == false)
                {
                    OnLeftClick?.Invoke(hit.transform.gameObject);

                }

                else
                {
                    OnLeftClick?.Invoke(null);
                }
            }
            else if (isAllowSipeOverUI && m_touch.phase == TouchPhase.Began)
            {
                m_startPos = m_touch.position;
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(m_touch.position), Vector2.zero, m_clickablerMask);

                if (hit.collider && EventSystem.current.IsPointerOverGameObject() == false)
                {
                    OnLeftClick?.Invoke(hit.transform.gameObject);

                }

                else
                {
                    OnLeftClick?.Invoke(null);
                }
            }

            else if (EventSystem.current.IsPointerOverGameObject(m_touch.fingerId) == false && m_touch.phase == TouchPhase.Ended)
            {

                if (!IsDrag) return;
                var distance = m_touch.position.x - m_startPos.x;


                OnLeftEndDrag?.Invoke(distance);


                IsDrag = false;

            }
            else if (isAllowSipeOverUI && m_touch.phase == TouchPhase.Ended)
            {

                if (!IsDrag) return;
                var distance = m_touch.position.x - m_startPos.x;


                OnLeftEndDrag?.Invoke(distance);


                IsDrag = false;
            }
            else if (EventSystem.current.IsPointerOverGameObject(m_touch.fingerId) == false && m_touch.phase == TouchPhase.Moved)
            {
                IsDrag = true;

            }
            else if (isAllowSipeOverUI && m_touch.phase == TouchPhase.Moved)
            {
                IsDrag = true;
            }

#endif

        }
    }
}


