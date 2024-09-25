using System.Collections;
using UnityEngine;

public class Timer
{
	public delegate void Task();

	private static MonoBehaviour behaviour;

	public static void Schedule(MonoBehaviour _behaviour, float delay, Task task)
	{
		behaviour = _behaviour;
		behaviour.StartCoroutine(DoTask(task, delay));
	}

	private static IEnumerator DoTask(Task task, float delay)
	{
		yield return new WaitForSeconds(delay);
		task();
	}
}
