using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioChannel : MonoBehaviour
{
    public AudioSource audioSource;

    private float timer;
    private float duration;
    private float volumeScale;

    public bool IsInUse { get; private set; }
    public bool IsLooping { get; private set; }

    public delegate void ChannelUsageActivity(AudioChannel channel);
    public static event ChannelUsageActivity onStartPlaying;
    public static event ChannelUsageActivity onStopPlaying;
        
    private void Update()
    {
        if(IsInUse && !IsLooping)
        {
            timer += Time.deltaTime;

            if (timer >= duration)
            {
                IsInUse = false;
                OnStopPlaying();
            }
        }
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (audioSource != null && clip)
        {
            timer = 0;
            duration = clip.length;
            audioSource.PlayOneShot(clip, volumeScale);
            IsInUse = true;
            OnStartPlaying();
        }
    }

    public void Play(AudioClip clip, ulong delay = 0, bool loop = false)
    {
        if(audioSource != null && clip)
        {
            audioSource.loop = IsLooping = loop;

            if (!IsLooping)
            {
                timer = 0;
                duration = clip.length;
            }

            audioSource.clip = clip;
            audioSource.Play(delay);
            IsInUse = true;
            OnStartPlaying();
        }
    }

    public void Stop()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            IsInUse = false;
            IsLooping = false;
            OnStopPlaying();
        }
    }

    public void RegisterOnStartPlaying(ChannelUsageActivity onStartPlayingListener)
    {
        if(onStartPlayingListener != null)
        {
            onStartPlaying -= onStartPlayingListener;
            onStartPlaying += onStartPlayingListener;
        }
    }

    public void RegisterOnStopPlaying(ChannelUsageActivity onStopPlayingListener)
    {
        if (onStopPlayingListener != null)
        {
            onStopPlaying -= onStopPlayingListener;
            onStopPlaying += onStopPlayingListener;
        }
    }

    public void OnStartPlaying()
    {
        if (onStartPlaying != null)
        {
            onStartPlaying(this);
        }
    }

    public void OnStopPlaying()
    {
        if(onStopPlaying != null)
        {
            onStopPlaying(this);
        }
    }

    public void FadeOutStop(float duration = 0)
    {
        if(audioSource != null)
        {
            TweenManager.Instance.ValueTransition(audioSource.volume, 0, duration, true, null, (float v) => {
                audioSource.volume = v;
            }, Stop);
        }
    }

	public void FadeInPlay(AudioClip clip, float maxVolume = 1, float duration = 0, ulong delay = 0, bool loop = false)
    {
        if (audioSource != null)
        {            
            audioSource.volume = 0;
            Play(clip, 0, loop);
            TweenManager.Instance.ValueTransition(audioSource.volume, maxVolume, duration, true, null, (float v) => {
                audioSource.volume = v;
            });
        }
    }

    public void SetVolume(float volume)
    {
        volumeScale = volume;
    }
}