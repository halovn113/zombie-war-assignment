using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace KHCore
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSourcePrefab;
        [SerializeField] private AudioSource singleSFXSource;
        [SerializeField] private AudioSource bgmLayerSource;

        [Header("Audio SO")]
        [SerializeField] private AudioContainer bgmSOSources;
        [SerializeField] private AudioContainer sfxSOSources;

        [Header("Settings")]
        [SerializeField] private float fadeDuration = 1.0f;
        [SerializeField] private int maxSFXSources = 10;
        [SerializeField] private AudioMixer audioMixer;

        private Dictionary<string, AudioClip> _bgmTracks = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> _sfxTracks = new Dictionary<string, AudioClip>();
        private List<AudioSource> _sfxSources = new List<AudioSource>();
        void Awake()
        {
            ServiceLocator.Register(this);
            InitializeSFXPool();
            LoadAudioTracks();
        }

        private void LoadAudioTracks()
        {
            foreach (var track in bgmSOSources.audioInfos)
            {
                _bgmTracks.Add(track.audioName, track.audio);
                // Debug.Log($"Loaded BGM track: {track.audioName}");
            }
            foreach (var track in sfxSOSources.audioInfos)
            {
                _sfxTracks.Add(track.audioName, track.audio);
                // Debug.Log($"Loaded BGM track: {track.audioName}");
            }
        }

        public void PlayMusic(string trackName, bool loop = false)
        {
            if (!_bgmTracks.TryGetValue(trackName, out AudioClip clip))
            {
                // If not found in SOAudio, load from Resources
                clip = Resources.Load<AudioClip>($"Audio/{trackName}");
                if (clip == null)
                {
                    Debug.LogWarning("Music " + trackName + " not found!");
                    return;
                }
            }

            StartCoroutine(SwitchBGM(clip, loop));
        }

        private IEnumerator SwitchBGM(AudioClip newClip, bool loop = false)
        {
            if (bgmSource.isPlaying)
            {
                yield return StartCoroutine(FadeOut(bgmSource, fadeDuration));
            }

            bgmSource.clip = newClip;
            bgmSource.loop = loop;
            bgmSource.Play();
            yield return StartCoroutine(FadeIn(bgmSource, fadeDuration));
        }

        public void PlaySFX(string sfxName)
        {
            if (!_sfxTracks.TryGetValue(sfxName, out AudioClip clip))
            {
                // If not found in SOAudio, load from Resources
                clip = Resources.Load<AudioClip>($"Audio/{sfxName}");
                if (clip == null)
                {
                    Debug.LogWarning($"SFX {sfxName} not found!");
                    return;
                }
            }

            AudioSource availableSource = GetAvailableSFXSource();
            if (availableSource != null)
            {
                availableSource.gameObject.SetActive(true);
                availableSource.PlayOneShot(clip);
                StartCoroutine(DisableSFXSource(availableSource, clip.length));
            }
        }

        private void InitializeSFXPool()
        {
            for (int i = 0; i < maxSFXSources; i++)
            {
                AudioSource newSource = Instantiate(sfxSourcePrefab, transform);
                newSource.gameObject.SetActive(false);
                _sfxSources.Add(newSource);
            }
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (var source in _sfxSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            return null;
        }

        private IEnumerator DisableSFXSource(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            source.gameObject.SetActive(false);
        }

        private IEnumerator FadeOut(AudioSource audioSource, float duration)
        {
            float startVolume = audioSource.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }

            audioSource.volume = 0;
            audioSource.Stop();
        }

        private IEnumerator FadeIn(AudioSource audioSource, float duration)
        {
            audioSource.volume = 0;
            float targetVolume = 1.0f;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
                yield return null;
            }

            audioSource.volume = targetVolume;
        }

        public void PlaySingleEffect(string effectName)
        {
            if (!_sfxTracks.TryGetValue(effectName, out AudioClip clip))
            {
                Debug.LogWarning($"SFX {effectName} not found.");
                return;
            }

            singleSFXSource.Stop();
            singleSFXSource.clip = clip;
            singleSFXSource.Play();
        }

        public void SetVolume(string channel, float volume)
        {
            audioMixer.SetFloat(channel, Mathf.Log10(volume) * 20); // Convert volume to decibel
        }

        public void LoadVolumeSettings()
        {
            SetVolume("Master", PlayerPrefs.GetFloat("MasterVolume", 1.0f));
            SetVolume("BGM", PlayerPrefs.GetFloat("BGMVolume", 1.0f));
            SetVolume("SFX", PlayerPrefs.GetFloat("SFXVolume", 1.0f));
        }

        public void PlayLayeredMusic(string baseTrack, string layerTrack)
        {
            PlayMusic(baseTrack);
            bgmLayerSource.clip = _bgmTracks[layerTrack];
            bgmLayerSource.Play();
        }

        public void PlayEffectWithCallback(string effectName, System.Action callback)
        {
            if (!_sfxTracks.TryGetValue(effectName, out AudioClip clip))
                return;

            AudioSource source = GetAvailableSFXSource();
            source.PlayOneShot(clip);
            StartCoroutine(InvokeAfterSeconds(clip.length, callback));
        }

        private IEnumerator InvokeAfterSeconds(float delay, System.Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

        public void StartDucking()
        {
            audioMixer.SetFloat("BGM", -10f); // Reduce volume of BGM for event
        }

        public void StopDucking()
        {
            audioMixer.SetFloat("BGM", 0f); // Restore volume of BGM
        }

        public AudioClip GetSFX(string sfxName)
        {
            return _sfxTracks[sfxName];
        }

        public AudioClip GetBGM(string bgmName)
        {
            return _bgmTracks[bgmName];
        }

        public void StopBGM()
        {
            bgmLayerSource.Stop();
        }
    }

}
