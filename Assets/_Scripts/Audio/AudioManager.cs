using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
	[Header("Prefabs")]
    public AudioChannel audioChannelPrefab;

    [Header("AudioClips")]
    public AudioClip audioBeep;
    public AudioClip cash;

	[Header("Scene Music Clip Selections")]
	public MusicChannelManager.EMusicClip titleSceneClip;
	public MusicChannelManager.EMusicClip workshopSceneClip;
	public MusicChannelManager.EMusicClip levelSelectSceneClip;
	[Tooltip("NOTE: Leave the first item empty")]
	public MusicChannelManager.EMusicClip[] tutorialLevelSceneClips;
	[Tooltip("NOTE: Leave the first item empty")]
	public MusicChannelManager.EMusicClip[] gameLevelSceneClips;
	public MusicChannelManager musicManager;


	[Header("SFX Channels")]
	public float sfxVolume = 1.0f;
    public List<AudioChannel> channelsInUse = new List<AudioChannel>();
    public List<AudioChannel> channelsAvailable = new List<AudioChannel>();

	private AudioChannel selectedAudioChannel = null;

    public void PlayCash(float volume = 1)
    {
        PlayOneShot(cash, volume);
    }

    public void PlayBeep(float volume = 1)
    {
        PlayOneShot(audioBeep, volume);
    }

	public bool isMusicPlaying()
	{
		return musicManager != null && musicManager.isMusicPlaying();
	}

	public void PlayMusicForScene(string sceneName)
	{
		if (!string.IsNullOrEmpty(sceneName) && musicManager != null)
		{
			MusicChannelManager.EMusicClip sceneClip = MusicChannelManager.EMusicClip.None;
			if (sceneName.CompareTo(GGConst.SCENE_NAME_START) == 0)
			{
				sceneClip = titleSceneClip;
			}
			else if (sceneName.CompareTo(GGConst.SCENE_NAME_WORKSHOP) == 0)
			{
				sceneClip = workshopSceneClip;
			}
			else if (sceneName.CompareTo(GGConst.SCENE_NAME_LEVEL_SELECT) == 0)
			{
				sceneClip = levelSelectSceneClip;
			}
			else if (sceneName.StartsWith(GGConst.SCENE_NAME_TUTORIAL_LEVEL_PREFIX))
			{
				sceneClip = GetSceneClipForLevel(sceneName, tutorialLevelSceneClips);
			}
			else if (sceneName.StartsWith(GGConst.SCENE_NAME_GAME_LEVEL_PREFIX))
			{
				sceneClip = GetSceneClipForLevel(sceneName, gameLevelSceneClips);
			}
			musicManager.PlayMusic(sceneClip);
		}
	}

	private MusicChannelManager.EMusicClip GetSceneClipForLevel(string sceneName, MusicChannelManager.EMusicClip[] sceneClips)
	{
		MusicChannelManager.EMusicClip sceneClip = MusicChannelManager.EMusicClip.None;
		string num = sceneName.Substring(sceneName.Length - 2);
		int index = 0;
		if (int.TryParse(num, out index))
		{
			int enumCount = Enum.GetNames(typeof(MusicChannelManager.EMusicClip)).Length;
			if (index > 0 && index < enumCount && index < sceneClips.Length)
			{
				sceneClip = sceneClips[index];
			}
		}

		return sceneClip;
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