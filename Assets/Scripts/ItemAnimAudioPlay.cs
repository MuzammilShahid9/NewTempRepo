using UnityEngine;

public class ItemAnimAudioPlay : MonoBehaviour
{
	public AudioClip[] audioArray;

	public void PlayAudioClip(int index)
	{
		if (index < audioArray.Length)
		{
			string audioEffectName = audioArray[index].name;
			AudioManager.Instance.PlayAudioEffect(audioEffectName);
		}
	}
}
