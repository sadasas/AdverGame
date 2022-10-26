

using AdverGame.Customer;
using UnityEngine;

namespace AdverGame.Chair
{
    public enum ChairType
    {
        CUSTOMERS,
        DRIVERS
    }
    public class ChairController:MonoBehaviour
    {
        public ChairType Type;
        public Transform CushionPos;
        public CustomerController Customer = null;
        public bool IsLeft;
        public ChairAnchor Anchor;
      
    }
}
