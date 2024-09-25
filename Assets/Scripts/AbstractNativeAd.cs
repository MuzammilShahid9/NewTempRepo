using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractNativeAd : MonoBehaviour
{
	public struct Data
	{
		public Uri MainImageUrl;

		public Uri IconImageUrl;

		public Uri ClickDestinationUrl;

		public string CallToAction;

		public string Title;

		public string Text;

		public double StarRating;

		public Uri PrivacyInformationIconClickThroughUrl;

		public Uri PrivacyInformationIconImageUrl;

		public static Uri ToUri(object value)
		{
			Uri uri = value as Uri;
			if (uri != null)
			{
				return uri;
			}
			string text = value as string;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
			{
				return new Uri(text, UriKind.Absolute);
			}
			Debug.LogError("Invalid URL: " + text);
			return null;
		}

		

		public override string ToString()
		{
			return string.Format("mainImageUrl: {0}\niconImageUrl: {1}\nclickDestinationUrl: {2}\ncallToAction: {3}\ntitle: {4}\ntext: {5}\nstarRating: {6}\nprivacyInformationIconClickThroughUrl: {7}\nprivacyInformationIconImageUrl: {8}", MainImageUrl, IconImageUrl, ClickDestinationUrl, CallToAction, Title, Text, StarRating, PrivacyInformationIconClickThroughUrl, PrivacyInformationIconImageUrl);
		}
	}

	public string AdUnitId;

	[Header("Text")]
	public Text Title;

	public Text Text;

	public Text CallToAction;

	[Header("Images")]
	public Renderer MainImage;

	public Renderer IconImage;

	public Renderer PrivacyInformationIconImage;
}
