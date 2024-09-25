using System;
using UnityEngine;

[Serializable]
public class DemoConfig : ScriptableObject
{
	[Header("下落速度")]
	[Header("------速度调整区域------")]
	public float DropSpeed;

	[Header("元素消除后下落延迟时间")]
	public float DropdelayTime;

	[Header("爆炸元素消除后下落延迟时间")]
	public float BombDropdelayTime;

	[Header("炸弹合成速度")]
	public float CreateBombSpeed;

	[Header("步数转换炸弹速度")]
	public float StepToBoomSpeed;

	[Header("蜜蜂飞行加速度")]
	public float BeeBoomFlySpeed;

	[Header("技能条增长速度")]
	public float powerValueSpeed;

	[Header("普通炸弹消除行列数")]
	[Header("------炸弹参数调整区域------")]
	public int AreaBombNum;

	[Header("合成炸弹消除行列数")]
	public int CombineAreaBombNum;

	[Header("单个蜜蜂道具攻击目标数量")]
	public int BeeBombNum;

	[Header("两个蜜蜂合成后攻击目标数量")]
	public int CombineBeeBombNum;

	[Header("单个金币代表金币数量")]
	[Header("------结算参数调整区域------")]
	public int OneCoinNum;

	[Header("道具触发收集金币速度")]
	public float CoinFlySpeed;

	[Header("胜利界面收集单个金币时间")]
	public float WinCoinFlyTime;

	[Header("胜利界面收集单个卷轴时间")]
	public float WinScrollFlyTime;

	[Header("普通难度下通关金币数量")]
	public int normalCoinNum;

	[Header("高等难度下通关金币数量")]
	public int hardCoinNum;

	[Header("普通难度下通关卷轴数量")]
	public int normalBookNum;

	[Header("高等难度下通关卷轴数量")]
	public int hardBookNum;

	[Header("彩球金币数")]
	[Header("------金币参数调整区域------")]
	public int colorBombCoin;

	[Header("横消金币数")]
	public int VBombCoin;

	[Header("竖消金币数")]
	public int HBombCoin;

	[Header("炸弹金币数")]
	public int ABombCoin;

	[Header("蜜蜂金币数")]
	public int flyBombCoin;

	[Header("收集目标放大")]
	[Header("------收集目标调整区域------")]
	public float rate;

	[Header("普通收集目标放大")]
	public float normalRate;

	[Header("Awesome匹配消除元素数量")]
	[Header("------评语触发数量调整区域------")]
	public int awesomeRemoveNum;

	[Header("Amazing匹配消除元素数量")]
	public int amazingRemoveNum;

	[Header("Terrific匹配消除元素数量")]
	public int terrificRemoveNum;

	[Header("Magnificent匹配消除元素数量")]
	public int magnificentRemoveNum;

	[Header("Awesome匹配Combor数量")]
	public int awesomeComborNum;

	[Header("Amazing匹配Combor数量")]
	public int amazingComborNum;

	[Header("Terrific匹配Combor数量")]
	public int terrificComborNum;

	[Header("Magnificent匹配Combor数量")]
	public int magnificentComborNum;

	[Header("促销展示时间（秒）")]
	[Header("------促销相关时间设置------")]
	public long SaleTime;

	[Header("高价值用户多少秒后出现促销")]
	public long HighUserCheckTime;

	[Header("促销开启关卡")]
	public int SaleActiveLevel;

	[Header("破冰促销方案")]
	[Range(1f, 3f)]
	public int FirstSaleType;

	[Header("促销间隔秒数")]
	public long SaleCDTime;

	[Header("银行间隔秒数")]
	public long BankSaleCDTime;

	[Header("银行开启关卡")]
	public int BankSaleActiveLevel;

	[Header("银行展示时间（秒）")]
	public long BankSaleTime;

	[Header("银行收集倍数")]
	public int BankSaleMultiple;

	[Header("Combo开启关卡")]
	public int ComboActiveLevel;

	[Header("Combo展示时间（秒）")]
	public int ComboTime;

	[Header("Combo间隔秒数")]
	public int ComboCDTime;

	[Header("每天游戏胜利可观看视频次数")]
	[Header("------视频奖励------")]
	public int MaxTimeToWinWatchVideo;

	[Header("每天任务面板可观看视频次数")]
	public int TaskVideoLimit;

	[Header("游戏胜利开启视频奖励Level")]
	public int ActvieWatchVideoInGameWin;

	[Header("任务面板开启视频奖励Level")]
	public int ActvieWatchVideoInTask;
}
