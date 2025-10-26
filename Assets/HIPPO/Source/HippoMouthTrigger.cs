using System.Collections;
using UnityEngine;

namespace HIPPO
{
    public class HippoMouthTrigger : MonoBehaviour
    {
        [SerializeField] private HippoHead _head;
        [SerializeField] private Transform _attachParent;
        [SerializeField, Min(0f)] private float _consumeDelay = 1f;
        
        private void OnTriggerEnter(Collider other)
        {
            var food = other.GetComponent<FoodItem>();

            if (food.IsHeld) 
                food.Drop();
            
            food.PickUp(_attachParent);
            StartCoroutine(Consume(food));
        }

        private IEnumerator Consume(FoodItem food)
        {
            yield return new WaitForSeconds(_consumeDelay);
            if (_head) _head.ForceCloseMouth();
            if (food) food.Consume();
        }
    }
}

