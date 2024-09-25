using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class FPS : MonoBehaviour
{
	private float w;

	private float h;

	private Rect rect;

	private string sUserMemory;

	private string s;

	public bool OnMemoryGUI;

	private long MonoUsedM;

	private long AllMemory;

	[Range(0f, 100f)]
	public int MaxMonoUsedM = 50;

	[Range(0f, 400f)]
	public int MaxAllMemory = 200;

	public long GfxDriver;

	public long ReservedMemory;

	private StringBuilder sb = new StringBuilder();

	private float updateInterval = 0.5f;

	private float accum;

	private float frames;

	private float timeleft;

	private float fps;

	private string FPSAAA;

	[Range(0f, 150f)]
	public int MaxFPS;

	private GUIStyle style = new GUIStyle();

	private void Start()
	{
		timeleft = updateInterval;
		style.fontSize = 20;
		h = (float)Screen.height / 720f;
		w = (float)Screen.width / 1280f;
		style.fontSize = (int)(30f * w);
		GUI.color = new Color(1f, 0f, 0f);
		rect = new Rect(10f, 70f, 200f * w, 60f * h);
	}

	private void Update()
	{
		UpdateUsed();
		UpdateFPS();
	}

	private void UpdateUsed()
	{
		sb.Clear();
		MonoUsedM = Profiler.GetMonoUsedSizeLong() / 1000000;
		ReservedMemory = Profiler.GetTotalReservedMemoryLong() / 1000000;
		AllMemory = Profiler.GetTotalAllocatedMemoryLong() / 1000000;
		GfxDriver = Profiler.GetAllocatedMemoryForGraphicsDriver() / 1000000;
		sb.Append("MonoUsed:" + MonoUsedM + "M\n");
		sb.Append("GfxDriver:" + GfxDriver + "M\n");
		sb.Append("AllMemory:" + AllMemory + "M\n");
		sb.Append("ReservedMemory:" + ReservedMemory + "M\n");
		sb.Append("UnUsedReserved:" + Profiler.GetTotalUnusedReservedMemoryLong() / 1000000 + "M\n");
		sb.Append("FPS:" + FPSAAA + "\n");
	}

	private void UpdateFPS()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames += 1f;
		if ((double)timeleft <= 0.0)
		{
			fps = accum / frames;
			FPSAAA = fps.ToString("f2");
			timeleft = updateInterval;
			accum = 0f;
			frames = 0f;
		}
	}

	private void OnGUI()
	{
		if (OnMemoryGUI)
		{
			GUI.Label(rect, sb.ToString(), style);
		}
	}
}
