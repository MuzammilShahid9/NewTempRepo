using System.Collections;
using System.IO;
using UnityEngine;

namespace taecg.tools
{
	public class ImageExporterController : MonoBehaviour
	{
		[HideInInspector]
		public Camera cam;

		[HideInInspector]
		public string imageFormat;

		[HideInInspector]
		public bool isEnabledAlpha;

		[HideInInspector]
		public Vector2 resolution;

		[HideInInspector]
		public int frameCount;

		[HideInInspector]
		public string fileName;

		[HideInInspector]
		public string filePath;

		[HideInInspector]
		public int rangeStart;

		[HideInInspector]
		public int rangeEnd;

		private void Awake()
		{
			if ((bool)(cam = null))
			{
				cam = Camera.main;
			}
		}

		private void Start()
		{
			Time.captureFramerate = frameCount;
		}

		private void Update()
		{
		}

		public void TakeSequenceScreenShot()
		{
			StartCoroutine(WaitTakeSequenceScreenShot());
		}

		private IEnumerator WaitTakeSequenceScreenShot()
		{
			yield return new WaitForEndOfFrame();
			int num = (int)resolution.x;
			int num2 = (int)resolution.y;
			RenderTexture renderTexture = new RenderTexture(num, num2, 24);
			cam.targetTexture = renderTexture;
			TextureFormat textureFormat = ((!isEnabledAlpha) ? TextureFormat.RGB24 : TextureFormat.ARGB32);
			Texture2D texture2D = new Texture2D(num, num2, textureFormat, false);
			cam.Render();
			RenderTexture.active = renderTexture;
			texture2D.ReadPixels(new Rect(0f, 0f, num, num2), 0, 0);
			texture2D.Apply();
			cam.targetTexture = null;
			RenderTexture.active = null;
			if (!isEnabledAlpha)
			{
				Object.Destroy(renderTexture);
			}
			string text = imageFormat;
			File.WriteAllBytes(bytes: (text == ".png") ? texture2D.EncodeToPNG() : ((!(text == ".jpg")) ? texture2D.EncodeToPNG() : texture2D.EncodeToJPG()), path: filePath + "/" + fileName + "_" + Time.frameCount + imageFormat);
		}
	}
}
