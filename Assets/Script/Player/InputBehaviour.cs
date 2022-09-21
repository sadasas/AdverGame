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

		public Action<GameObject> OnLeftClick;
		public Action<Vector2> OnLeftDrag;



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


			if (m_touch.phase == TouchPhase.Began)
			{
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(m_touch.position), Vector2.zero, m_clickablerMask);
				if (hit.collider && EventSystem.current.IsPointerOverGameObject() == false)
				{

					OnLeftClick?.Invoke(hit.transform.gameObject);
				}

			}
			else if (m_touch.phase == TouchPhase.Moved && EventSystem.current.IsPointerOverGameObject() == false)
			{
				OnLeftDrag.Invoke(m_touch.deltaPosition);
			}
#endif

		}
	}
}


