using UnityEngine;

// Plays all music and sound effects in the game.
// Every sound has its own volume slider in the Inspector.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;   // loops the background music
    public AudioSource sfxSource;     // plays one-shot sound effects

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    [Range(0f, 1f)] public float musicVolume = 0.4f;

    [Header("Sound Effects")]
    public AudioClip jumpClip;
    public AudioClip dashClip;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public AudioClip coinClip;
    public AudioClip buttonClip;
    public AudioClip pauseClip;
    public AudioClip resumeClip;
    public AudioClip gameOverClip;

    [Header("Footsteps (sand)")]
    public AudioClip[] stepClips;      // several single step sounds

    [Header("Volumes (0 = silent, 1 = full)")]
    [Range(0f, 1f)] public float jumpVolume = 1f;
    [Range(0f, 1f)] public float dashVolume = 1f;
    [Range(0f, 1f)] public float hitVolume = 1f;
    [Range(0f, 1f)] public float deathVolume = 1f;
    [Range(0f, 1f)] public float coinVolume = 1f;
    [Range(0f, 1f)] public float buttonVolume = 1f;
    [Range(0f, 1f)] public float pauseVolume = 1f;
    [Range(0f, 1f)] public float resumeVolume = 1f;
    [Range(0f, 1f)] public float gameOverVolume = 1f;
    [Range(0f, 1f)] public float stepVolume = 0.5f;   // steps are usually quieter

    void Awake()
    {
        Instance = this;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }

    // Lets you change the music volume with the slider while the game is running
    void OnValidate()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
    }

    // Start a music track (does nothing if it is already playing)
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Pause or resume the music (used by the pause menu)
    public void SetMusicPaused(bool paused)
    {
        if (paused) musicSource.Pause();
        else musicSource.UnPause();
    }

    // Play a sound effect once at the given volume
    void PlaySfx(AudioClip clip, float volume)
    {
        if (clip == null) return;   // a missing clip is simply skipped
        sfxSource.PlayOneShot(clip, volume);
    }

    // Plays a random footstep sound
    public void PlayStep()
    {
        if (stepClips == null || stepClips.Length == 0) return;
        AudioClip clip = stepClips[Random.Range(0, stepClips.Length)];
        sfxSource.PlayOneShot(clip, stepVolume);
    }

    // Short helper methods used by the other scripts
    public void PlayMenuMusic() { PlayMusic(menuMusic); }
    public void PlayGameMusic() { PlayMusic(gameMusic); }
    public void PlayJump() { PlaySfx(jumpClip, jumpVolume); }
    public void PlayDash() { PlaySfx(dashClip, dashVolume); }
    public void PlayHit() { PlaySfx(hitClip, hitVolume); }
    public void PlayDeath() { PlaySfx(deathClip, deathVolume); }
    public void PlayCoin() { PlaySfx(coinClip, coinVolume); }
    public void PlayButton() { PlaySfx(buttonClip, buttonVolume); }
    public void PlayPause() { PlaySfx(pauseClip, pauseVolume); }
    public void PlayResume() { PlaySfx(resumeClip, resumeVolume); }
    public void PlayGameOver() { PlaySfx(gameOverClip, gameOverVolume); }
}