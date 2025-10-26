using System;
using System.Collections;
using UnityEngine;

namespace HIPPO
{
    public class HippoEat : MonoBehaviour
    {
        [SerializeField] private HippoHead _head;
        [SerializeField] private Transform _attachParent;
        [SerializeField, Min(0f)] private float _consumeDelay = 1f;
        public event Action OnConsumed;
        
        private void OnTriggerEnter(Collider other)
        {
            var food = other.GetComponentInParent<FoodItem>();
            if (food == null || _head == null) return;
            if (food.IsHeld) food.Drop();
            food.PickUp(_attachParent);
            StartCoroutine(Consume(food));
        }

        private IEnumerator Consume(FoodItem food)
        {
            yield return new WaitForSeconds(_consumeDelay);
            if (_head) _head.ForceCloseMouth();
            OnConsumed?.Invoke();
            if (food) food.Consume();
        }
    }
}
