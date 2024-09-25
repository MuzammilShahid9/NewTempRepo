using UnityEngine;
using XLua;

public class ByString : MonoBehaviour
{
	private LuaEnv luaenv;

	private void Start()
	{
		luaenv = new LuaEnv();
		luaenv.DoString("print('hello world')");
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
