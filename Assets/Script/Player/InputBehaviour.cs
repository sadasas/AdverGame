using UnityEngine;

class InputBehaviour :MonoBehaviour 
{
    Touch m_touch;
    LayerMask m_customerMask;


    public InputBehaviour(LayerMask customerMask)
    {
        m_customerMask = customerMask;
    }

    public void Update()
    {
        DetecTouchTriggered();
    }

    private void DetecTouchTriggered()
    {
#if DEBUG
        if (Input.touchCount <= 0) return;
   
        m_touch = Input.GetTouch(0);
        Debug.Log("toc");
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(m_touch.position), Vector2.down, 2f, m_customerMask);
        if (hit.collider.CompareTag("Customer"))
        {
            hit.transform.GetComponent<ICustomer>().OnTouch();
        }

#endif
    }
}
