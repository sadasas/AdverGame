using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Customer
{
    public class CustomerController
    {
        public CustomerController(int delaySpawner, MonoBehaviour root, List<GameObject> customerPrefab)
        {

            m_delaySpawner = delaySpawner;
            m_root = root;
            m_customerPrefab = customerPrefab;

            SpawnCustomer();
        }


        int m_delaySpawner;
        MonoBehaviour m_root;
        List<GameObject> m_customerPrefab;

        public List<ICustomer> Customers;


        void SpawnCustomer()
        {
            if (Customers == null) Customers = new();
            for (int i = 0; i < m_customerPrefab.Count; i++)
            {

                var newCust = GameObject.Instantiate(m_customerPrefab[i], SetRandomPos(m_customerPrefab[i].GetComponent<SpriteRenderer>().bounds.size.x, m_customerPrefab[i].GetComponent<SpriteRenderer>().bounds.size.y / 2), Quaternion.identity).GetComponent<ICustomer>();

                Customers.Add(newCust);

            }
        }

        Vector2 SetRandomPos(float m_widhtCustOffset, float m_heightCustOffset)
        {
            var stageDimension = Camera.main.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
            return new Vector2(stageDimension.x + m_widhtCustOffset, Random.Range(0, -(stageDimension.y - m_heightCustOffset)));
        }
    }
}

