using UnityEngine;

namespace HIPPO
{
    public class HippoEffects : MonoBehaviour
    {
        [SerializeField] private HippoEat _hippoEat;
        [SerializeField] private ParticleSystem _eatParticles;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _eatClip;
        [SerializeField] private Transform _particlePoint;

        private void OnEnable()
        {
            _hippoEat.OnConsumed += PlayEatEffect;
        }

        private void OnDisable()
        {
            _hippoEat.OnConsumed -= PlayEatEffect;
        }

        private void PlayEatEffect()
        {
            if (_eatParticles != null)
                _eatParticles.Play();

            if (_eatClip != null)
                _audioSource?.PlayOneShot(_eatClip);
        }
    }
}
