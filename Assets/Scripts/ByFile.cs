using UnityEngine;
using XLua;

public class ByFile : MonoBehaviour
{
	private LuaEnv luaenv;

	private void Start()
	{
		luaenv = new LuaEnv();
		luaenv.DoString("require 'byfile'");
	}

	private void Update()
	{
		if (luaenv != null)
		{
			luaenv.Tick();
		}
	}

	private void OnDestroy()
	{
		luaenv.Dispose();
	}
}
