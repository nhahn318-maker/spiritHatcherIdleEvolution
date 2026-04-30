using UnityEngine;
using UnityEngine.UI;

namespace SpiritHatchers.UI
{
    [RequireComponent(typeof(Image))]
    public class CreatureSpriteAnimationUI : MonoBehaviour
    {
        [SerializeField] private Image targetImage;
        [SerializeField] private Sprite[] frames;
        [SerializeField, Min(1f)] private float framesPerSecond = 8f;
        [SerializeField] private bool playOnEnable;

        private int frameIndex;
        private float timer;
        private bool isPlaying;

        public bool HasFrames => frames != null && frames.Length > 0;

        private void Awake()
        {
            FindImageIfNeeded();
        }

        private void OnEnable()
        {
            if (playOnEnable && HasFrames)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            isPlaying = false;
        }

        private void Update()
        {
            if (!isPlaying || !HasFrames)
            {
                return;
            }

            timer += Time.unscaledDeltaTime;
            float frameDuration = 1f / framesPerSecond;

            while (timer >= frameDuration)
            {
                timer -= frameDuration;
                frameIndex = (frameIndex + 1) % frames.Length;
                ApplyFrame();
            }
        }

        public void Play()
        {
            FindImageIfNeeded();

            if (targetImage == null || !HasFrames)
            {
                return;
            }

            isPlaying = true;
            frameIndex = Mathf.Clamp(frameIndex, 0, frames.Length - 1);
            ApplyFrame();
        }

        public void Play(Sprite[] animationFrames, Sprite fallbackSprite)
        {
            frames = animationFrames;
            frameIndex = 0;
            timer = 0f;

            if (HasFrames)
            {
                Play();
            }
            else
            {
                Stop(fallbackSprite);
            }
        }

        public void Stop(Sprite fallbackSprite)
        {
            FindImageIfNeeded();
            isPlaying = false;
            frameIndex = 0;
            timer = 0f;

            if (targetImage == null)
            {
                return;
            }

            targetImage.sprite = fallbackSprite;
            targetImage.enabled = fallbackSprite != null;
            targetImage.preserveAspect = true;
        }

        private void ApplyFrame()
        {
            if (targetImage == null || !HasFrames)
            {
                return;
            }

            targetImage.sprite = frames[frameIndex];
            targetImage.enabled = frames[frameIndex] != null;
            targetImage.preserveAspect = true;
        }

        private void FindImageIfNeeded()
        {
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }
        }
    }
}
