using System;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Text_Localization", 10)]
public class LocalizationText : Text
{
	public string KeyString;

	public Font EnglishFont;

	public Font ChineseFont;

	public Font KoreanFont;

	public Font JapaneseFont;

	public Font RussianFont;

	public bool IsOpenLocalize = true;

	public override string text
	{
		get
		{
			if (IsOpenLocalize)
			{
				if (string.IsNullOrEmpty(KeyString))
				{
					return m_Text;
				}
				if (!LanguageConfig.IsContainKey(KeyString))
				{
					return KeyString;
				}
				m_Text = LanguageConfig.GetString(KeyString);
				return m_Text;
			}
			return m_Text;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(m_Text))
				{
					m_Text = "";
					SetVerticesDirty();
				}
			}
			else if (m_Text != value)
			{
				m_Text = value;
				SetVerticesDirty();
				SetLayoutDirty();
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (IsOpenLocalize)
		{
			LanguageConfig.OnLocalize = (LanguageConfig.GameVoidDelegate)Delegate.Combine(LanguageConfig.OnLocalize, new LanguageConfig.GameVoidDelegate(OnLocalize));
		}
	}

	protected override void Start()
	{
		base.Start();
		ChangeFont();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (IsOpenLocalize && LanguageConfig.OnLocalize != null)
		{
			LanguageConfig.OnLocalize = (LanguageConfig.GameVoidDelegate)Delegate.Remove(LanguageConfig.OnLocalize, new LanguageConfig.GameVoidDelegate(OnLocalize));
		}
	}

	public void OnLocalize()
	{
		if (IsOpenLocalize)
		{
			ChangeFont();
			text = LanguageConfig.GetString(KeyString);
		}
	}

	private void ChangeFont()
	{
		if (LanguageConfig.GetCurrentLanguage() == SystemLanguage.Chinese)
		{
			if (ChineseFont != null)
			{
				base.font = ChineseFont;
				return;
			}
			ChineseFont = LanguageConfig.FontCollect[SystemLanguage.Chinese];
			base.font = ChineseFont;
			return;
		}
		if (LanguageConfig.GetCurrentLanguage() == SystemLanguage.Korean)
		{
			KoreanFont = LanguageConfig.FontCollect[SystemLanguage.Korean];
			base.font = KoreanFont;
			return;
		}
		if (LanguageConfig.GetCurrentLanguage() == SystemLanguage.Japanese)
		{
			if (JapaneseFont != null)
			{
				base.font = JapaneseFont;
				return;
			}
			JapaneseFont = LanguageConfig.FontCollect[SystemLanguage.Japanese];
			base.font = JapaneseFont;
			return;
		}
		if (LanguageConfig.GetCurrentLanguage() == SystemLanguage.Russian)
		{
			if (RussianFont != null)
			{
				base.font = RussianFont;
				return;
			}
			RussianFont = LanguageConfig.FontCollect[SystemLanguage.Russian];
			base.font = RussianFont;
			return;
		}
		if (EnglishFont != null)
		{
			base.font = EnglishFont;
		}
		else
		{
			EnglishFont = LanguageConfig.FontCollect[SystemLanguage.English];
			base.font = EnglishFont;
		}
		bool flag = base.font.name == "Poetsen One";
	}

	public void SetKeyString(string value)
	{
		KeyString = value;
		OnLocalize();
	}
}
