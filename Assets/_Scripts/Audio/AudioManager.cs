using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
	public enum EMusicClip
	{
		None,
		Music01_GotToBe,
		Music02_DistantOrbits,
		Music03_SixYearAgo,
		Music04_KeysOne,
		Music05_TisztaSzivvel,
		Music06_YouNeverToldMe,
		Music07_One,
		Music08_HungoroHai,
		Music09_EmersonPond,
	}

	[Header("Prefabs")]
    public AudioChannel audioChannelPrefab;

    [Header("AudioClips")]
    public AudioClip audioTick;
    public AudioClip audioBeep;
    public AudioClip audioTimeIsUp;
    public AudioClip cash;

	[Header("MusicAudioClips")]
	[Tooltip("NOTE: Leave the first item empty")]
	public List<AudioClip> musicClips;

	[Header("SFX Channels")]
	public float sfxVolume = 1.0f;
    public List<AudioChannel> channelsInUse = new List<AudioChannel>();
    public List<AudioChannel> channelsAvailable = new List<AudioChannel>();
	[Header("Music Channels")]
	public float musicVolume = 0.5f;
	public AudioChannel musicChannel1;
	public AudioChannel musicChannel2;

	private AudioChannel currentMusicChannel;

	private AudioChannel selectedAudioChannel = null;

    private AudioChannel audioTickChannel;

    public void PlayTick()
    {
        audioTickChannel = FadeInPlay(audioTick, 0, true);
    }

    public void StopTick()
    {
        if(audioTickChannel != null)
        {
            audioTickChannel.FadeOutStop();
        }
    }

    public void PlayCash(float volume = 1)
    {
        PlayOneShot(cash, volume);
    }

    public void PlayBeep(float volume = 1)
    {
        PlayOneShot(audioBeep, volume);
    }

    public void PlayTimeIsUp(float volume = 1)
    {
        PlayOneShot(audioTimeIsUp, volume);
    }

	public bool isMusicPlaying()
	{
		return currentMusicChannel != null && currentMusicChannel.audioSource != null && currentMusicChannel.audioSource.isPlaying;
	}

	public void PlayMusic(EMusicClip musicEnum)
	{
		if (musicClips.Count > 0 && (int)musicEnum < musicClips.Count)
		{
			AudioClip curClip = musicClips[(int)musicEnum];
			if (currentMusicChannel == null || (currentMusicChannel != null && currentMusicChannel.audioSource.clip != curClip))
			{
				PlayMusic(curClip);
			}
		}
	}

	public void PlayMusic(AudioClip musicClip)
	{
		if (currentMusicChannel != null)
		{
			AudioChannel newChannel = currentMusicChannel == musicChannel1 ? musicChannel2 : musicChannel1;
			currentMusicChannel.FadeOutStop(1);
			MusicFadeInPlay(newChannel, musicClip);
			currentMusicChannel = newChannel;
		}
		else
		{
			currentMusicChannel = musicChannel1;
			MusicFadeInPlay(currentMusicChannel, musicClip);
		}
	}

	private void PlayOneShot(AudioClip clip, float volume = 1)
    {
        if(clip != null)
        {            
            selectedAudioChannel = GetAvailableAudioChannel();
            if (selectedAudioChannel != null)
            {
                selectedAudioChannel.SetVolume(volume);
                selectedAudioChannel.PlayOneShot(clip);
            }
        }
    }

    private void Play(AudioClip clip, ulong delay = 0, bool loop = false)
    {
        if (clip != null)
        {
            selectedAudioChannel = GetAvailableAudioChannel();
            if (selectedAudioChannel != null)
            {
                selectedAudioChannel.Play(clip, delay, loop);
            }
        }
    }

	private AudioChannel MusicFadeInPlay(AudioChannel channel, AudioClip clip, ulong delay = 0, bool loop = true)
	{
		if (clip != null)
		{
			if (channel != null)
			{
				channel.FadeInPlay(clip, musicVolume, 1, delay, loop);
			}
		}

		return selectedAudioChannel;
	}

	private AudioChannel FadeInPlay(AudioClip clip, ulong delay = 0, bool loop = false)
    {
        if (clip != null)
        {
            selectedAudioChannel = GetAvailableAudioChannel();
            if (selectedAudioChannel != null)
            {
                selectedAudioChannel.FadeInPlay(clip, sfxVolume, 1f, delay, loop);
            }
        }

        return selectedAudioChannel;
    }

    private AudioChannel GetAvailableAudioChannel()
    {
        AudioChannel channel = null;

        if (channelsAvailable != null)
        {
            if (channelsAvailable.Count > 0)
            {
                channel = channelsAvailable[0];
            }
            else if (audioChannelPrefab != null)
            {
                channel = ObjectFactoryManager.Instance.CreateInstance<AudioChannel>(audioChannelPrefab.gameObject, transform);
                channelsAvailable.Add(channel);
            }
        }

        if(channel != null)
        {
            channel.RegisterOnStartPlaying(SetAudioChannelInUse);
            channel.RegisterOnStopPlaying(SetAudioChannelAvailable);
            channel.gameObject.SetActive(true);
        }

        return channel;
    }

    private void SetAudioChannelInUse(AudioChannel audioChannel)
    {
        if (channelsInUse != null && audioChannel != null)
        {
            channelsAvailable.Remove(audioChannel);
            channelsInUse.Add(audioChannel);
        }
    }

    private void SetAudioChannelAvailable(AudioChannel audioChannel)
    {
        if (channelsInUse != null && audioChannel != null)
        {
            channelsInUse.Remove(audioChannel);
            channelsAvailable.Add(audioChannel);
            audioChannel.gameObject.SetActive(false);
        }
    }
}