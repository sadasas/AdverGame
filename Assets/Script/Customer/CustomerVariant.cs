using UnityEngine;


namespace AdverGame.Customer
{
    public enum CustomerType : int
    {
        RARE = 5,
        OJOL = 10,
        COMMON = 40
    }
    [CreateAssetMenu(fileName = "Variant", menuName = "Customer Variant")]
    public class CustomerVariant : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public string Description { get; private set; }

        [field: SerializeField]
        public int OccurrencePercentage { get; private set; }
        [field: SerializeField]
        public int Coin { get; private set; }
        [field: SerializeField]
        public int Exp { get; private set; }
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
        public float IdleTime { get; private set; }
        [field: SerializeField]
        public GameObject CustomerPrefab { get; private set; }
        [field: SerializeField]
       
        public Sprite EatImage { get; private set; }
        [field: SerializeField]
        public Sprite HappyImage { get; private set; }

        [field: SerializeField]
        public Sprite ConfusedImage { get; private set; }

        [field: SerializeField]
        public Sprite AngryImage { get; private set; }

        [field: SerializeField]
        public Sprite Image { get; private set; }


        [field: SerializeField]
        public CustomerType Type { get; private set; }


    }
}


