using UnityEngine;

namespace AdverGame.Player
{
    [CreateAssetMenu(fileName = "Item Content", menuName = "Item Content")]
    public class ItemContent : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        public Sprite Image { get; private set; }
        [field: SerializeField]
        public GameObject ItemPrefab { get; private set; }

    }


}
