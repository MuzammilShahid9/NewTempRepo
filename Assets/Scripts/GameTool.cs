using System.Diagnostics;
using UnityEngine;

public class GameTool : MonoBehaviour
{
	public void BtnExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_plot.py " + Application.dataPath + "/GameTools/Excel//PlotConfig.xlsx " + Application.dataPath + "/Resources/Config/Plot/";
		DebugUtils.Log(DebugType.Other, processStartInfo.ToString());
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnTaskExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_task.py " + Application.dataPath + "/GameTools/Excel//TaskConfig.xlsx " + Application.dataPath + "/Resources/Config/Task/";
		DebugUtils.Log(DebugType.Other, processStartInfo.ToString());
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnLanguageExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_language.py " + Application.dataPath + "/GameTools/Excel/LanguageConfig.xlsx " + Application.dataPath + "/Resources/Config/Language/";
		DebugUtils.Log(DebugType.Other, processStartInfo.ToString());
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnShopExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_shop.py " + Application.dataPath + "/GameTools/Excel/ShopConfig.xlsx " + Application.dataPath + "/Resources/Config/Shop/";
		DebugUtils.Log(DebugType.Other, processStartInfo.ToString());
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnSaleExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_sale.py " + Application.dataPath + "/GameTools/Excel/SaleConfig.xlsx " + Application.dataPath + "/Resources/Config/Sale/";
		DebugUtils.Log(DebugType.Other, processStartInfo.ToString());
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnItemExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_item.py " + Application.dataPath + "/GameTools/Excel/ItemConfig.xlsx " + Application.dataPath + "/Resources/Config/Item/";
		DebugUtils.Log(DebugType.Other, processStartInfo.ToString());
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnIdleActionExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_idleaction.py " + Application.dataPath + "/GameTools/Excel/IdleActionConfig.xlsx " + Application.dataPath + "/Resources/Config/IdleAction/";
		DebugUtils.Log(DebugType.Other, processStartInfo.ToString());
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnAddMovesExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_addMoves.py " + Application.dataPath + "/GameTools/Excel/AddMovesConfig.xlsx " + Application.dataPath + "/Resources/Config/AddMoves/";
		DebugUtils.Log(DebugType.Other, processStartInfo.Arguments);
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnRoleExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_role.py " + Application.dataPath + "/GameTools/Excel/RoleConfig.xlsx " + Application.dataPath + "/Resources/Config/Role/";
		DebugUtils.Log(DebugType.Other, processStartInfo.Arguments);
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}

	public void BtnEditorExcelToJsonClicked()
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		processStartInfo.FileName = "python";
		processStartInfo.UseShellExecute = false;
		processStartInfo.RedirectStandardOutput = true;
		processStartInfo.Arguments = Application.dataPath + "/GameTools/Python/generate_editor.py " + Application.dataPath + "/GameTools/Excel/EditorConfig.xlsx " + Application.dataPath + "/Resources/Config/Editor/";
		DebugUtils.Log(DebugType.Other, processStartInfo.Arguments);
		Process process = Process.Start(processStartInfo);
		string text = process.StandardOutput.ReadToEnd();
		process.WaitForExit();
		DebugUtils.Log(DebugType.Other, "command output " + text);
	}
}
