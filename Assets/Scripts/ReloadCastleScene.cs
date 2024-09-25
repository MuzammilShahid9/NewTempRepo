using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReloadCastleScene : MonoBehaviour
{
	public LocalizationText loadingText;

	public Slider loadingBar;

	public AsyncOperation asyncMainScene;

	private int _resTotal = 26;

	private int _resCount;

	private void Start()
	{
		loadingBar.value = 0.1f;
		loadingText.SetKeyString("MainLoadingText" + Random.Range(1, 6));
		StartCoroutine(LoadCastleScene());
	}

	private IEnumerator LoadCastleScene()
	{
		GlobalVariables.ComboNum = 0;
		asyncMainScene = SceneManager.LoadSceneAsync("_Scenes/CastleScene");
		while (!asyncMainScene.isDone)
		{
			loadingBar.value = 0.1f + asyncMainScene.progress;
			yield return null;
		}
	}
}
