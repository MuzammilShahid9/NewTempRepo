using UnityEngine;

public class Book : MonoBehaviour
{
	public Animation BookAnimation;

	public void PlayOpen()
	{
		BookAnimation.Play("shu");
	}

	public void PlayClose()
	{
		BookAnimation.Play("shu 2");
	}
}
