using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDChapterManager : MonoBehaviour
	{
		private static CDChapterManager instance;

		public GameObject chapterText;

		public GameObject chapterTextPanel;

		public VerticalLayoutGroup textContent;

		public Button btnPrevChapter;

		public Button btnNextChapter;

		public Button btnLoadChapter;

		public Button btnSaveChapter;

		public InputField chapterNumInput;

		public InputField taskNumInput;

		public InputField subTaskNumInput;

		public int currentChapter;

		public int chapterTotal;

		public List<Chapter> chapterList;

		private List<Text> _chapterTextList = new List<Text>();

		private string _path;

		private XmlDocument _xml;

		private XmlNode _xmlRoot;

		public static CDChapterManager Instance
		{
			get
			{
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			chapterList = new List<Chapter>();
		}

		private void Start()
		{
			_path = Application.dataPath + "/Resources/Config/Plot/";
			if (Directory.Exists(_path))
			{
				FileInfo[] files = new DirectoryInfo(_path).GetFiles("*", SearchOption.AllDirectories);
				DebugUtils.Log(DebugType.Other, files.Length);
				for (int i = 0; i < files.Length; i++)
				{
					if (!files[i].Name.EndsWith(".meta") && files[i].Name.Contains("Chapter"))
					{
						DebugUtils.Log(DebugType.Other, "Name:" + files[i].Name.TrimEnd());
						AddText(files[i].Name.Split('.')[0]);
						Chapter chapter = CreateChapter();
						chapter.fileName = files[i].Name;
						chapterList.Add(chapter);
					}
				}
				string xmlFile = _path + chapterList[0].fileName;
				CDTaskManager.Instance.LoadChapter(xmlFile, 0, 0, 0);
			}
			currentChapter = 0;
			SetTextColor(currentChapter, Color.red);
			chapterNumInput.text = "1";
			taskNumInput.text = "1";
			subTaskNumInput.text = "0";
			StartCoroutine(resizeContent());
		}

		private void OnEnable()
		{
			StartCoroutine(resizeContent());
		}

		public Chapter CreateChapter()
		{
			return new Chapter
			{
				taskList = new List<Task>()
			};
		}

		public void RefreshInfo()
		{
		}

		public void BtnPrevChapterClicked()
		{
			if (currentChapter > 0)
			{
				ResetColor();
				currentChapter--;
				SetTextColor(currentChapter, Color.red);
				string xmlFile = _path + chapterList[currentChapter].fileName;
				CDTaskManager.Instance.LoadChapter(xmlFile, currentChapter, 0, 0);
			}
			StartCoroutine(resizeContent());
		}

		public void BtnNextChapterClicked()
		{
			if (currentChapter < chapterList.Count - 1)
			{
				ResetColor();
				currentChapter++;
				SetTextColor(currentChapter, Color.red);
				string xmlFile = _path + chapterList[currentChapter].fileName;
				CDTaskManager.Instance.LoadChapter(xmlFile, currentChapter, 0, 0);
			}
			StartCoroutine(resizeContent());
		}

		private IEnumerator resizeContent()
		{
			yield return null;
			textContent.GetComponent<RectTransform>().sizeDelta = new Vector2(textContent.GetComponent<RectTransform>().sizeDelta.x, textContent.preferredHeight);
		}

		private void AddText(string str)
		{
			GameObject obj = Object.Instantiate(chapterText);
			Text component = obj.GetComponent<Text>();
			obj.transform.SetParent(chapterTextPanel.transform);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			component.text = str;
			_chapterTextList.Add(component);
		}

		private void InsertText(string str, int idx)
		{
		}

		private void DelText(int idx)
		{
			Object.Destroy(_chapterTextList[idx].gameObject);
			_chapterTextList.RemoveAt(idx);
		}

		private void ClearText()
		{
			_chapterTextList.Clear();
			for (int i = 0; i < chapterTextPanel.transform.childCount; i++)
			{
				Object.Destroy(chapterTextPanel.transform.GetChild(i).gameObject);
			}
		}

		private void ResetColor()
		{
			foreach (Text chapterText in _chapterTextList)
			{
				chapterText.color = Color.black;
			}
		}

		private void SetTextColor(int idx, Color color)
		{
			ResetColor();
			_chapterTextList[idx].color = color;
		}

		public void BtnSaveChapterClicked()
		{
			CDTaskManager.Instance.SaveTask(_path + chapterList[currentChapter].fileName);
		}

		public void BtnLoadChapterClicked()
		{
			int num = int.Parse(chapterNumInput.text);
			int taskID = int.Parse(taskNumInput.text);
			int subTaskID = int.Parse(subTaskNumInput.text);
			if (num < chapterList.Count)
			{
				string xmlFile = _path + chapterList[num].fileName;
				CDTaskManager.Instance.LoadChapter(xmlFile, num, taskID, subTaskID);
			}
			else if (num - 1 == chapterList.Count)
			{
				AddText("Chapter" + num);
				Chapter chapter = CreateChapter();
				chapter.fileName = "Chapter" + num + ".xml";
				chapterList.Add(chapter);
				currentChapter = num - 1;
				SetTextColor(currentChapter, Color.red);
				CDTaskManager.Instance.CreateChapter(num, _path + "Chapter" + num + ".xml");
			}
			else
			{
				CinemaDirector.Instance.ShowText("Must create Chapter In Order!");
			}
		}

		public int GetCurrentChapter()
		{
			return currentChapter;
		}
	}
}
