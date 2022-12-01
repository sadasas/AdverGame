using UnityEngine;

namespace AdverGame.Customer
{
    [CreateAssetMenu(fileName = "Variant", menuName = "Dummy Variant")]
    public class DummyVariant :ScriptableObject
    {
        [field: SerializeField]
        public Sprite ConfusedImage { get; private set; }

        [field: SerializeField]
        public float WalkDelay { get; private set; }


        [field: SerializeField]
        public float IdleTime { get; private set; }

        [field: SerializeField]
        public float Speed { get; private set; }

        [field: SerializeField]
        public int OccurrencePercentage { get; private set; }

    }
}


