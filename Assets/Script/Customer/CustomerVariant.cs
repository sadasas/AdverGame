using UnityEngine;


namespace AdverGame.Customer
{
    public enum CustomerType
    {
        COMMON,
        RARE
    }
    [CreateAssetMenu(fileName = "Variant", menuName = "Customer Variant")]
    public class CustomerVariant : ScriptableObject
    {
        [field: SerializeField]
        public int Coin { get; private set; }
        [field: SerializeField]
        public float Speed { get; private set; }
        [field: SerializeField]
        public float EatTime { get; private set; }
        [field: SerializeField]
        public int SpawnDelay { get; private set; }
        [field: SerializeField]
        public float WaitOrderMaxTime { get; private set; }
        [field: SerializeField]
        public float WaitChairAvailableTime { get; private set; }
        [field: SerializeField]
        public Sprite RealCustomerImage { get; private set; }
        [field: SerializeField]
        public Sprite DummylCustomerImage { get; private set; }
        [field: SerializeField]
        public Sprite EatImage { get; private set; }
        [field: SerializeField]
        public Sprite HappyImage { get; private set; }
        [field: SerializeField]
        public Sprite AngryImage { get; private set; }



        [field: SerializeField]
        public CustomerType Type { get; private set; }


    }
}


