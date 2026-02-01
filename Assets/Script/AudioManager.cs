using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM")]
    public AudioClip bgm;

    [Header("SFX")]
    public AudioClip sfxMoneySuccess;
    public AudioClip sfxOpenBook;
    public AudioClip sfxMaskAppear;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBGM();
    }

    // ---------------- BGM ----------------

    public void PlayBGM()
    {
        if (bgmSource == null || bgm == null) return;

        bgmSource.clip = bgm;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource == null) return;
        bgmSource.Stop();
    }

    public void ResumeBGM()
    {
        if (bgmSource == null) return;

        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    // ---------------- SFX ----------------

    public void PlayMoneySuccess()
    {
        PlaySFX(sfxMoneySuccess);
    }

    public void PlayOpenBook()
    {
        PlaySFX(sfxOpenBook);
    }

    public void PlayMaskAppear()
    {
        PlaySFX(sfxMaskAppear);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
