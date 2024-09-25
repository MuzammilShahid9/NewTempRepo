using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class PlayBeeAudio : MonoBehaviour
{
	private void OnEnable()
	{
		if (UserDataManager.Instance.GetService().soundEnabled)
		{
			AudioSource component = GetComponent<AudioSource>();
			if (component.enabled)
			{
				component.Play();
			}
		}
	}
}
