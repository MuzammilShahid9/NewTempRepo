using System.Collections;
using UnityEngine;

public class DebugToScreen : MonoBehaviour
{
	private Rect windowRect;

	private Vector2 scrollPosition;

	private GUIStyle font = new GUIStyle();

	private static bool banShowOnScreen = false;

	private static bool isPause;

	private static ArrayList msgList = ArrayList.Synchronized(new ArrayList());

	private static DebugToScreen instance;

	private float h;

	private float w;

	public static void Init(GameObject go)
	{
		if (instance == null)
		{
			instance = go.AddComponent<DebugToScreen>();
		}
	}

	public static void PostException(string message)
	{
		if (TestConfig.isShowBugWindow && !isPause)
		{
			msgList.Add(" i = " + msgList.Count + " " + message);
		}
	}

	private void Awake()
	{
		h = (float)Screen.height / 720f;
		w = (float)Screen.width / 1280f;
		font.fontSize = (int)(30f * h);
		scrollPosition = new Vector2(50f * w, 50f * h);
		windowRect = new Rect(0f, 0f, 1280f * w, 720f * h);
		banShowOnScreen = false;
	}

	public static void RegisterHandler()
	{
	}

	private void OnGUI()
	{
		if (!banShowOnScreen && msgList != null && msgList.Count > 0)
		{
			windowRect = GUI.Window(1, windowRect, DoWindow, "Debug");
		}
	}

	private void DoWindow(int windowId)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		for (int i = 0; i < msgList.Count; i++)
		{
			GUILayout.Label(msgList[i].ToString(), font);
		}
		GUILayout.EndScrollView();
		if (GUILayout.Button("Clear", GUILayout.Height(70f * h)))
		{
			msgList.Clear();
		}
		if (GUILayout.Button("Close", GUILayout.Height(70f * h)))
		{
			msgList.Clear();
		}
		if (GUILayout.Button("Pause", GUILayout.Height(70f * h)))
		{
			isPause = !isPause;
		}
		GUI.DragWindow();
	}

	private void OnDestroy()
	{
		isPause = true;
		instance = null;
		msgList = null;
	}

	public void DebugLogCallback(byte[] data)
	{
	}
}
