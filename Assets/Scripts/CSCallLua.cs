using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class CSCallLua : MonoBehaviour
{
	public class DClass
	{
		public int f1;

		public int f2;
	}

	[CSharpCallLua]
	public interface ItfD
	{
		int f1 { get; set; }

		int f2 { get; set; }

		int add(int a, int b);
	}

	[CSharpCallLua]
	public delegate int FDelegate(int a, string b, out DClass c);

	[CSharpCallLua]
	public delegate Action GetE();

	private LuaEnv luaenv;

	private string script = "\r\n        a = 1\r\n        b = 'hello world'\r\n        c = true\r\n\r\n        d = {\r\n           f1 = 12, f2 = 34, \r\n           1, 2, 3,\r\n           add = function(self, a, b) \r\n              print('d.add called')\r\n              return a + b \r\n           end\r\n        }\r\n\r\n        function e()\r\n            print('i am e')\r\n        end\r\n\r\n        function f(a, b)\r\n            print('a', a, 'b', b)\r\n            return 1, {f1 = 1024}\r\n        end\r\n        \r\n        function ret_e()\r\n            print('ret_e called')\r\n            return e\r\n        end\r\n    ";

	private void Start()
	{
		luaenv = new LuaEnv();
		luaenv.DoString(script);
		DebugUtils.Log(DebugType.Other, "_G.a = " + luaenv.Global.Get<int>("a"));
		DebugUtils.Log(DebugType.Other, "_G.b = " + luaenv.Global.Get<string>("b"));
		DebugUtils.Log(DebugType.Other, "_G.c = " + luaenv.Global.Get<bool>("c"));
		DClass dClass = luaenv.Global.Get<DClass>("d");
		DebugUtils.Log(DebugType.Other, "_G.d = {f1=" + dClass.f1 + ", f2=" + dClass.f2 + "}");
		Dictionary<string, double> dictionary = luaenv.Global.Get<Dictionary<string, double>>("d");
		DebugUtils.Log(DebugType.Other, "_G.d = {f1=" + dictionary["f1"] + ", f2=" + dictionary["f2"] + "}, d.Count=" + dictionary.Count);
		List<double> list = luaenv.Global.Get<List<double>>("d");
		DebugUtils.Log(DebugType.Other, "_G.d.len = " + list.Count);
		ItfD itfD = luaenv.Global.Get<ItfD>("d");
		itfD.f2 = 1000;
		DebugUtils.Log(DebugType.Other, "_G.d = {f1=" + itfD.f1 + ", f2=" + itfD.f2 + "}");
		DebugUtils.Log(DebugType.Other, "_G.d:add(1, 2)=" + itfD.add(1, 2));
		LuaTable luaTable = luaenv.Global.Get<LuaTable>("d");
		DebugUtils.Log(DebugType.Other, "_G.d = {f1=" + luaTable.Get<int>("f1") + ", f2=" + luaTable.Get<int>("f2") + "}");
		luaenv.Global.Get<Action>("e")();
		DClass c;
		int num = luaenv.Global.Get<FDelegate>("f")(100, "John", out c);
		DebugUtils.Log(DebugType.Other, "ret.d = {f1=" + c.f1 + ", f2=" + c.f2 + "}, ret=" + num);
		luaenv.Global.Get<GetE>("ret_e")()();
		luaenv.Global.Get<LuaFunction>("e").Call();
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
