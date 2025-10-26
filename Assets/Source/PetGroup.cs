using UnityEngine;

namespace Source
{
    public class PetGroup : MonoBehaviour
    {
        [SerializeField] private Animator[] _petsAnimators;

        public Animator CurrentPet { get; private set; }

        private int _currentPetIndex;

        private void Awake()
        {
            if (_petsAnimators.Length == 0)
                return;
        
            CurrentPet = _petsAnimators[_currentPetIndex];
            CurrentPet.gameObject.SetActive(true);
        }

        public void SelectNextPet()
        {
            _currentPetIndex++;
        
            if (_currentPetIndex >= _petsAnimators.Length)
                _currentPetIndex = 0;
        
            CurrentPet?.gameObject.SetActive(false);
            CurrentPet = _petsAnimators[_currentPetIndex];
            CurrentPet.gameObject.SetActive(true);
        }
    }
}
