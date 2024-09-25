using UnityEngine;

public class SendEmail
{
	public static void SendEmails(string info)
	{
		DebugUtils.Log(DebugType.Other, "Capture Screenshot");
		ScreenCapture.CaptureScreenshot("screen.png");
	}
}
