using UnityEngine;


namespace AdverGame.Customer
{
    [CreateAssetMenu(fileName = "Variant", menuName = "Customer Variant")]
    public class CustomerVariant : ScriptableObject
    {
        public float Speed;
        public int Coin;
    }
}


