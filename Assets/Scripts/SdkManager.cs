using Umeng;

public static class SdkManager
{
	public static void InitSDK()
	{
		DebugUtils.Log(DebugType.Other, "Init SDK");

		Analytics.StartWithAppKeyAndChannelId("5c88ccbb203657b04e000f84", "GooglePlay");

	}
}
