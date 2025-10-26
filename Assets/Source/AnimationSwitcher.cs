using UnityEngine;

namespace Source
{
    public class AnimationSwitcher : MonoBehaviour
    {
        private static readonly int AnimationHash = Animator.StringToHash("AnimationIndex");
    
        [SerializeField] private PetGroup _petGroup;
    
        private int _animationIndex;
        
        public void NextAnimation()
        {
            _animationIndex++;

            if (_animationIndex > 1)
                _animationIndex = 0;
        
            _petGroup.CurrentPet.SetInteger(AnimationHash, _animationIndex);
        }
    }
}
