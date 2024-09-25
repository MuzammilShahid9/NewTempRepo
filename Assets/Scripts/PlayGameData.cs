using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class PlayGameData : Singleton<PlayGameData>
{
	public DemoConfig gameConfig;

	public Dictionary<int, int> WeightDic = new Dictionary<int, int>
	{
		{ 1, 1 },
		{ 2, 1 },
		{ 3, 1 },
		{ 4, 1 },
		{ 5, 1 },
		{ 6, 1 },
		{ 7, 1 },
		{ 22, -151 },
		{ 23, 1 },
		{ 41, 24 },
		{ 42, 25 },
		{ 43, 26 },
		{ 13, 0 },
		{ 14, 0 },
		{ 10, 0 },
		{ 11, 0 },
		{ 12, 0 },
		{ 21, 18 },
		{ 31, 7 },
		{ 32, 8 },
		{ 33, 9 },
		{ 34, 10 },
		{ 35, 11 },
		{ 71, 12 },
		{ 72, 13 },
		{ 73, 14 },
		{ 74, 15 },
		{ 51, 22 },
		{ 52, 21 },
		{ 61, 19 },
		{ 62, 20 },
		{ 10000, 13 },
		{ 11000, 14 },
		{ 12000, 15 },
		{ 13000, 16 },
		{ 20000, 13 },
		{ 21000, 14 },
		{ 22000, 15 },
		{ 23000, 16 },
		{ 24000, 17 }
	};

	public Dictionary<int, List<Combo>> ComboReward = new Dictionary<int, List<Combo>>
	{
		{
			0,
			new List<Combo>()
		},
		{
			1,
			new List<Combo>
			{
				new Combo(ElementType.AreaBomb, 1)
			}
		},
		{
			2,
			new List<Combo>
			{
				new Combo(ElementType.AreaBomb, 1),
				new Combo(ElementType.HorizontalBomb, 1)
			}
		},
		{
			3,
			new List<Combo>
			{
				new Combo(ElementType.AreaBomb, 1),
				new Combo(ElementType.HorizontalBomb, 1),
				new Combo(ElementType.FlyBomb, 1)
			}
		},
		{
			4,
			new List<Combo>
			{
				new Combo(ElementType.AreaBomb, 1),
				new Combo(ElementType.HorizontalBomb, 1),
				new Combo(ElementType.FlyBomb, 1),
				new Combo(ElementType.ColorBomb, 1)
			}
		}
	};

	public PlayGameData()
	{
		Init();
	}

	public void Init()
	{
		gameConfig = Resources.Load<DemoConfig>("Config/GameConfig/DemoConfig");
		
	}
}
