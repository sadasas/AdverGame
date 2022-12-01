using System;
using UnityEngine;

namespace AdverGame.Player
{
    public class ItemPlate : MonoBehaviour
    {
        public float TimeCooking;
        public bool IsEmpty { get; protected set; } = true;


        public ItemSerializable Item { get; set; }
        public Action<ItemSerializable, ItemPlate> OnPutItem;

        public virtual void StartState(Sprite image)
        {
        }


        public virtual void UpdateState(float progressCooking)
        {
        }
        public virtual void FinishState(Sprite image)
        {

        }
        public virtual void UpdateProggressBar(float amount)
        {

            
        }
    }
}
