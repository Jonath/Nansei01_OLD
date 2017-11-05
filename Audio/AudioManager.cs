using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance = null;     // Singleton

    public GameObject audio_prefab;

    private AudioSource musicSource;                 // Reference to the audio source which will play the music.
    private List<AudioSource> efxSources;             // Reference to the audio source which will play the music.

    void Awake()
    {
        // Set instance to this object
        if (instance == null) { 
            instance = this;
        }

        // Enforce there can be only one instance
        else if (instance != this) { 
            Destroy(gameObject);
        }

        // Set AudioManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

        if (audio_prefab != null) {
            Instantiate(audio_prefab, transform.position, transform.rotation);
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        efxSources = new List<AudioSource>();
    }

    public void Destroy()
    {
        instance = null;
        Destroy(gameObject);
    }

    public AudioSource GetMusicSource()
    {
        return musicSource;
    }

    public void PlaySingle(AudioClip clip)
    {
        if(clip == null) {
            return;
        }

        int found_it = -1;
        int i = 0;

        foreach(AudioSource source in efxSources)
        {
            if(source.clip == clip) {
                found_it = i;
            }
            i++;
        }

        AudioSource efxSource = null;
        // If not found
        if (found_it == -1) {
            //GameObject channel = Instantiate(audio_prefab, transform.position, transform.rotation) as GameObject;
            efxSource = gameObject.AddComponent<AudioSource>();
            efxSources.Add(efxSource);
        } else {
            efxSource = efxSources[found_it];
        }

        // Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;
        // Play the clip.
        efxSource.Play();
    }

    public void PlayMusic(AudioClip clip)
    {
        // Set the clip of our musicSource audio source to the clip passed in as a parameter.
        musicSource.clip = clip;
        musicSource.loop = true;

        // Play the clip.
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    IEnumerator CR_CountTime(AudioSource source)
    {
        float t = 0;

        while(t < source.clip.length) {
            t += Time.deltaTime;
        }
        source.Pause();

        yield break;
    }
}
