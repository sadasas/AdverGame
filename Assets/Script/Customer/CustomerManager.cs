using System.Collections.Generic;
using UnityEngine;

namespace AdverGame.Customer
{
    public class CustomerManager : MonoBehaviour
    {
        public static CustomerManager s_Instance;

        [SerializeField] List<GameObject> m_customerPrefab;

        public List<Customer> Customers;

        private void Awake()
        {
            if (s_Instance != null) Destroy(s_Instance.gameObject);
            s_Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SpawnCustomer();
        }

        void SpawnCustomer()
        {
            Customers ??= new();
            for (int i = 0; i < m_customerPrefab.Count; i++)
            {

                var newCust = GameObject.Instantiate(m_customerPrefab[i]).GetComponent<Customer>();
                Customers.Add(newCust);

            }
        }


    }
}

