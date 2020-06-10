using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChannelManager : MonoBehaviour
{
	public enum EMusicClip
	{
		None,
		Music01GotToBe,
		Music02DistantOrbits,
		Music03SixYearAgo,
		Music04KeysOne,
		Music05TisztaSzivvel,
		Music06YouNeverToldMe,
		Music07One,
		Music08HungoroHai,
		Music09EmersonPond,
	}

	[Header("Settings")]
	public float musicFadeTime = 1.0f;
	public float musicVolume = 0.5f;

	[Header("MusicAudioClips")]
	[Tooltip("NOTE: Leave the first item empty")]
	public List<AudioClip> musicClips;

	[Header("Music Channels")]
	public AudioChannel musicChannel1;
	public AudioChannel musicChannel2;

	private AudioChannel currentMusicChannel;

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
			currentMusicChannel.FadeOutStop(musicFadeTime);
			if (musicClip == null)
			{
				currentMusicChannel = null;
			}
			else
			{
				MusicFadeInPlay(newChannel, musicClip);
				currentMusicChannel = newChannel;
			}
		}
		else
		{
			if (musicClip != null)
			{
				currentMusicChannel = musicChannel1;
				MusicFadeInPlay(currentMusicChannel, musicClip);
			}
		}
	}

	private void MusicFadeInPlay(AudioChannel channel, AudioClip clip, ulong delay = 0, bool loop = true)
	{
		if (clip != null)
		{
			if (channel != null)
			{
				channel.FadeInPlay(clip, musicVolume, musicFadeTime, delay, loop);
			}
		}

	}

}