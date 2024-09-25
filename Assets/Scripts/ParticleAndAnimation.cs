using UnityEngine;

public class ParticleAndAnimation : MonoBehaviour
{
	private void Start()
	{
		PlayOnce();
	}

	[ContextMenu("Play Loop")]
	public void PlayLoop()
	{
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem obj in componentsInChildren)
		{
			obj.loop = true;
			obj.Play();
		}
		Animation[] componentsInChildren2 = GetComponentsInChildren<Animation>(true);
		foreach (Animation obj2 in componentsInChildren2)
		{
			obj2.wrapMode = WrapMode.Loop;
			obj2.Play();
		}
	}

	[ContextMenu("Play Once")]
	public void PlayOnce()
	{
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem obj in componentsInChildren)
		{
			obj.loop = false;
			obj.Play();
		}
		Animation[] componentsInChildren2 = GetComponentsInChildren<Animation>(true);
		foreach (Animation obj2 in componentsInChildren2)
		{
			obj2.wrapMode = WrapMode.Once;
			obj2.Play();
		}
	}
}
