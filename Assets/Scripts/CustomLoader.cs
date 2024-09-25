using System.Text;
using UnityEngine;
using XLua;

public class CustomLoader : MonoBehaviour
{
	private LuaEnv luaenv;

	private void Start()
	{
		luaenv = new LuaEnv();
		luaenv.AddLoader(delegate(ref string filename)
		{
			if (filename == "InMemory")
			{
				string s = "return {ccc = 9999}";
				return Encoding.UTF8.GetBytes(s);
			}
			return null;
		});
		luaenv.DoString("print('InMemory.ccc=', require('InMemory').ccc)");
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
