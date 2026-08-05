using UnityEngine;
using UnityEngine.EventSystems;

namespace ToolkitByJonathan
{
    [RequireComponent(typeof(RectTransform))]
    public class UIPlaySoundOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SoundAudioSourcePair _onHoverSound;
        [SerializeField] private SoundAudioSourcePair _onExitSound;

        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioSource source = _onHoverSound.Source;
            Sound sound = _onHoverSound.Sound;

            sound.Config.ApplyTo(source);

            source.Play();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AudioSource source = _onExitSound.Source;
            Sound sound = _onExitSound.Sound;

            sound.Config.ApplyTo(source);

            source.Play();
        }
    }
}
