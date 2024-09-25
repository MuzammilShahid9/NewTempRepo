using System.Collections;
using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private Dictionary<string, AudioSource> soundDictionary = new Dictionary<string, AudioSource>();

	private Dictionary<string, AudioSource> musicDictionary = new Dictionary<string, AudioSource>();

	public AudioSource musicAudioSource;

	public AudioSource effectAudioSource;

	private static AudioManager instance;

	private Coroutine musicCoroutine;

	private Coroutine effectCoroutine;

	private Coroutine idleEffectCoroutine;

	private Coroutine idleMusicCoroutine;

	private Dictionary<string, float> AudioEffectDic = new Dictionary<string, float>();

	private List<string> delList = new List<string>();

	private string PlayingMusicName;

	public static AudioManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		DebugUtils.Log(DebugType.Other, "load audio listener");
		instance = this;
		LoadAllAudio();
	}

	public void LoadAllAudio()
	{
		AudioClip[] array = Resources.LoadAll<AudioClip>("Sound");
		AudioClip[] array2 = Resources.LoadAll<AudioClip>("PlotSound");
		AudioClip[] array3 = array;
		foreach (AudioClip audioClip in array3)
		{
			AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.clip = audioClip;
			soundDictionary.Add(audioClip.name, audioSource);
		}
		array3 = array2;
		foreach (AudioClip audioClip2 in array3)
		{
			AudioSource audioSource2 = base.gameObject.AddComponent<AudioSource>();
			audioSource2.playOnAwake = false;
			audioSource2.clip = audioClip2;
			soundDictionary.Add(audioClip2.name, audioSource2);
		}
		array3 = Resources.LoadAll<AudioClip>("Music");
		foreach (AudioClip audioClip3 in array3)
		{
			DebugUtils.Log(DebugType.Other, "load music " + audioClip3.name);
			AudioSource audioSource3 = base.gameObject.AddComponent<AudioSource>();
			audioSource3.clip = audioClip3;
			audioSource3.loop = true;
			audioSource3.playOnAwake = false;
			musicDictionary.Add(audioClip3.name, audioSource3);
		}
		UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (AudioEffectDic.Count > 0)
			{
				Dictionary<string, float> dictionary = new Dictionary<string, float>(AudioEffectDic);
				foreach (string key in dictionary.Keys)
				{
					if (dictionary[key] <= 0f)
					{
						delList.Add(key);
					}
					else
					{
						AudioEffectDic[key] -= duration;
					}
				}
				foreach (string del in delList)
				{
					AudioEffectDic.Remove(del);
				}
				delList.Clear();
			}
			return false;
		}));
	}

	private void Start()
	{
		SetAudioEffectMute(!UserDataManager.Instance.GetService().soundEnabled);
		SetAudioMusicMute(!UserDataManager.Instance.GetService().musicEnabled);
	}

	public void PlayAudioMusic(string musicName)
	{
		StartCoroutine(MusicFadeInOut(0.5f, musicName, 1f));
	}

	public void StopAllBackgroundMusic()
	{
		foreach (KeyValuePair<string, AudioSource> item in musicDictionary)
		{
			item.Value.Stop();
		}
		PlayingMusicName = "";
	}

	public void PlayAudioEffect(string audioEffectName, float time)
	{
		if (soundDictionary.ContainsKey(audioEffectName) && !AudioEffectDic.ContainsKey(audioEffectName))
		{
			soundDictionary[audioEffectName].PlayOneShot(soundDictionary[audioEffectName].clip);
			AudioEffectDic.Add(audioEffectName, time);
		}
	}

	public void PlayAudioEffect(string audioEffectName, bool loop = false, float speed = 1f)
	{
		if (soundDictionary.ContainsKey(audioEffectName))
		{
			if (loop)
			{
				soundDictionary[audioEffectName].loop = true;
				soundDictionary[audioEffectName].pitch = speed;
				soundDictionary[audioEffectName].Play();
			}
			else
			{
				soundDictionary[audioEffectName].PlayOneShot(soundDictionary[audioEffectName].clip);
			}
		}
	}

	public void StopAudioEffect(string audioEffectName, bool underPlotControl = false)
	{
		soundDictionary[audioEffectName].Stop();
		if (underPlotControl)
		{
			StopAllCoroutines();
		}
	}

	public void SetAudioEffectMute(bool isMute)
	{
		foreach (KeyValuePair<string, AudioSource> item in soundDictionary)
		{
			item.Value.mute = isMute;
		}
	}

	public void SetAudioMusicMute(bool isMute)
	{
		foreach (KeyValuePair<string, AudioSource> item in musicDictionary)
		{
			item.Value.mute = isMute;
		}
	}

	public void PlotPlayMusic(MusicConfigData musicData)
	{
		if (musicData.Stop == 1)
		{
			if (musicCoroutine != null)
			{
				StopCoroutine(musicCoroutine);
			}
			if (musicData.Fade == 1)
			{
				StartCoroutine(MusicFadeOut(musicData.MusicName, musicData.FadeOutTime));
			}
			else
			{
				StopAllBackgroundMusic();
			}
		}
		else if (musicData.Stop == 0)
		{
			if (musicData.Fade == 1)
			{
				musicCoroutine = StartCoroutine(MusicFadeIn(musicData.MusicName, musicData.FadeInTime, musicData.Loop, musicData.LoopDelayTime));
			}
			else
			{
				musicCoroutine = StartCoroutine(MusicPlay(musicData.MusicName, musicData.Loop, musicData.LoopDelayTime));
			}
		}
	}

	public void IdlePlayMusic(MusicConfigData musicData, RoleType roleType)
	{
		if (musicData.Stop == 1)
		{
			if (idleMusicCoroutine != null)
			{
				StopCoroutine(idleMusicCoroutine);
			}
			if (musicData.Fade == 1)
			{
				StartCoroutine(MusicFadeOut(musicData.MusicName, musicData.FadeOutTime));
			}
			else
			{
				StopAllBackgroundMusic();
			}
		}
		else if (musicData.Stop == 0)
		{
			if (musicData.Fade == 1)
			{
				idleMusicCoroutine = StartCoroutine(MusicFadeIn(musicData.MusicName, musicData.FadeInTime, musicData.Loop, musicData.LoopDelayTime));
			}
			else
			{
				idleMusicCoroutine = StartCoroutine(MusicPlay(musicData.MusicName, musicData.Loop, musicData.LoopDelayTime));
			}
		}
	}

	private IEnumerator MusicFadeOut(string musicName, float fadeOutTime)
	{
		musicDictionary[musicName].volume = 1f;
		while (musicDictionary[musicName].volume > 0f)
		{
			yield return null;
			musicDictionary[musicName].volume -= 1f / fadeOutTime * Time.deltaTime;
		}
		StopAllBackgroundMusic();
		musicDictionary[musicName].volume = 1f;
	}

	private IEnumerator MusicFadeIn(string musicName, float fadeInTime, int loop, float loopDelayTime)
	{
		musicDictionary[musicName].volume = 0f;
		musicDictionary[musicName].Play();
		musicDictionary[musicName].loop = false;
		while (musicDictionary[musicName].volume < 1f)
		{
			yield return null;
			musicDictionary[musicName].volume += 1f / fadeInTime * Time.deltaTime;
		}
		if (loop == 1)
		{
			yield return new WaitForSeconds(musicDictionary[musicName].clip.length - fadeInTime);
			while (true)
			{
				StopAllBackgroundMusic();
				yield return new WaitForSeconds(loopDelayTime);
				musicDictionary[musicName].Play();
				yield return new WaitForSeconds(musicDictionary[musicName].clip.length);
			}
		}
	}

	private IEnumerator MusicFadeInOut(float fadeOutTime, string musicName2, float fadeInTime)
	{
		float time = 0f;
		PlayingMusicName = musicName2;
		while (time < fadeOutTime)
		{
			time += Time.deltaTime;
			foreach (KeyValuePair<string, AudioSource> item in musicDictionary)
			{
				if (item.Value.volume > 0f)
				{
					item.Value.volume -= 1f / fadeOutTime * Time.deltaTime;
					if (item.Value.volume <= 0f)
					{
						item.Value.Stop();
					}
				}
			}
			yield return null;
		}
		if (!string.IsNullOrEmpty(musicName2) && musicDictionary.ContainsKey(musicName2))
		{
			musicDictionary[musicName2].volume = 0f;
			musicDictionary[musicName2].Play();
			musicDictionary[musicName2].loop = true;
			while (musicDictionary[musicName2].volume < 1f)
			{
				yield return null;
				musicDictionary[musicName2].volume += 1f / fadeInTime * Time.deltaTime;
			}
		}
	}

	private IEnumerator MusicFadeInOut(float fadeOutTime)
	{
		float time = 0f;
		while (time < fadeOutTime)
		{
			time += Time.deltaTime;
			foreach (KeyValuePair<string, AudioSource> item in musicDictionary)
			{
				if (item.Value.volume > 0f)
				{
					item.Value.volume -= 1f / fadeOutTime * Time.deltaTime;
					if (item.Value.volume <= 0f)
					{
						item.Value.Stop();
					}
				}
			}
			yield return null;
		}
	}

	private IEnumerator MusicPlay(string musicName, int loop, float loopDelayTime)
	{
		musicDictionary[musicName].volume = 1f;
		musicDictionary[musicName].Play();
		musicDictionary[musicName].loop = false;
		if (loop == 1)
		{
			yield return new WaitForSeconds(musicDictionary[musicName].clip.length);
			while (true)
			{
				StopAllBackgroundMusic();
				yield return new WaitForSeconds(loopDelayTime);
				musicDictionary[musicName].Play();
				yield return new WaitForSeconds(musicDictionary[musicName].clip.length);
			}
		}
	}

	public void PlotPlayEffect(EffectConfigData effectData)
	{
		if (effectData.Stop == 1)
		{
			if (effectCoroutine != null)
			{
				StopCoroutine(effectCoroutine);
			}
			if (effectData.Fade == 1)
			{
				StartCoroutine(EffectFadeOut(effectData.EffectName, effectData.FadeOutTime));
			}
			else
			{
				soundDictionary[effectData.EffectName].Stop();
			}
		}
		else if (effectData.Stop == 0)
		{
			if (effectData.Fade == 1)
			{
				effectCoroutine = StartCoroutine(EffectFadeIn(effectData.EffectName, effectData.FadeInTime, effectData.Loop, effectData.LoopDelayTime));
			}
			else
			{
				effectCoroutine = StartCoroutine(EffectPlay(effectData.EffectName, effectData.Loop, effectData.LoopDelayTime));
			}
		}
	}

	public void IdlePlayEffect(EffectConfigData effectData, RoleType roleType)
	{
		if (effectData.Stop == 1)
		{
			if (idleEffectCoroutine != null)
			{
				StopCoroutine(idleEffectCoroutine);
			}
			if (effectData.Fade == 1)
			{
				StartCoroutine(EffectFadeOut(effectData.EffectName, effectData.FadeOutTime));
			}
			else
			{
				soundDictionary[effectData.EffectName].Stop();
			}
		}
		else if (effectData.Stop == 0)
		{
			if (effectData.Fade == 1)
			{
				idleEffectCoroutine = StartCoroutine(EffectFadeIn(effectData.EffectName, effectData.FadeInTime, effectData.Loop, effectData.LoopDelayTime));
			}
			else
			{
				idleEffectCoroutine = StartCoroutine(IdleEffectPlay(effectData.EffectName, effectData.Loop, effectData.LoopDelayTime, roleType));
			}
		}
	}

	private IEnumerator EffectFadeOut(string effectName, float fadeOutTime)
	{
		soundDictionary[effectName].volume = 1f;
		while (soundDictionary[effectName].volume > 0f)
		{
			yield return null;
			soundDictionary[effectName].volume -= 1f / fadeOutTime * Time.deltaTime;
		}
		soundDictionary[effectName].Stop();
		soundDictionary[effectName].volume = 1f;
	}

	private IEnumerator EffectFadeIn(string effectName, float fadeInTime, int loop, float loopDelayTime)
	{
		soundDictionary[effectName].volume = 0f;
		soundDictionary[effectName].Play();
		soundDictionary[effectName].loop = false;
		while (soundDictionary[effectName].volume < 1f)
		{
			yield return null;
			soundDictionary[effectName].volume += 1f / fadeInTime * Time.deltaTime;
		}
		if (loop == 1)
		{
			yield return new WaitForSeconds(soundDictionary[effectName].clip.length - fadeInTime);
			while (true)
			{
				soundDictionary[effectName].Stop();
				yield return new WaitForSeconds(loopDelayTime);
				soundDictionary[effectName].Play();
				yield return new WaitForSeconds(soundDictionary[effectName].clip.length);
			}
		}
	}

	private IEnumerator EffectPlay(string effectName, int loop, float loopDelayTime)
	{
		soundDictionary[effectName].volume = 1f;
		soundDictionary[effectName].Play();
		soundDictionary[effectName].loop = false;
		if (loop == 1)
		{
			yield return new WaitForSeconds(soundDictionary[effectName].clip.length);
			while (true)
			{
				soundDictionary[effectName].Stop();
				yield return new WaitForSeconds(loopDelayTime);
				soundDictionary[effectName].Play();
				yield return new WaitForSeconds(soundDictionary[effectName].clip.length);
			}
		}
		yield return new WaitForSeconds(soundDictionary[effectName].clip.length);
		PlotMusicManager.Instance.FinishEffectStep();
	}

	private IEnumerator IdleEffectPlay(string effectName, int loop, float loopDelayTime, RoleType roleType)
	{
		soundDictionary[effectName].volume = 1f;
		soundDictionary[effectName].Play();
		soundDictionary[effectName].loop = false;
		if (loop == 1)
		{
			yield return new WaitForSeconds(soundDictionary[effectName].clip.length);
			while (true)
			{
				soundDictionary[effectName].Stop();
				yield return new WaitForSeconds(loopDelayTime);
				soundDictionary[effectName].Play();
				yield return new WaitForSeconds(soundDictionary[effectName].clip.length);
			}
		}
		yield return new WaitForSeconds(soundDictionary[effectName].clip.length);
		IdleMusicManager.Instance.FinishEffectStep(roleType);
	}

	public void ChangeMusicVolume(float volume)
	{
		if (PlayingMusicName != "")
		{
			musicDictionary[PlayingMusicName].volume = volume;
		}
	}

	public void ChangeMusicVolume(float volume, float time)
	{
		if (!(PlayingMusicName != ""))
		{
			return;
		}
		AudioSource audioSource = musicDictionary[PlayingMusicName];
		float startVolume = audioSource.volume;
		float currentTime = 0f;
		UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (currentTime > time)
			{
				audioSource.volume = volume;
				return true;
			}
			audioSource.volume = Mathf.Lerp(startVolume, volume, currentTime / time);
			currentTime += duration;
			return false;
		}));
	}
}
