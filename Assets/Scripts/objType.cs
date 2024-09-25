using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class objType : MonoBehaviour
{
	public int type = -1;

	private ParticleSystem ps;

	private Animation[] ani;

	private SpriteRenderer[] list;

	private ParticleSystem[] plist;

	private MeshRenderer[] mlist;

	public List<Material> pMaterial;

	private List<float> pAlpha;

	private List<float> spriteAlpha;

	private void Awake()
	{
		ps = GetComponentInChildren<ParticleSystem>();
		ani = GetComponentsInChildren<Animation>();
		list = GetComponentsInChildren<SpriteRenderer>();
		plist = GetComponentsInChildren<ParticleSystem>(true);
		if (plist != null)
		{
			pMaterial = new List<Material>();
			pAlpha = new List<float>();
			ParticleSystem[] array = plist;
			foreach (ParticleSystem particleSystem in array)
			{
				if (!(particleSystem != null))
				{
					continue;
				}
				Renderer[] componentsInChildren = particleSystem.GetComponentsInChildren<Renderer>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					Material[] materials = componentsInChildren[j].materials;
					for (int k = 0; k < materials.Length; k++)
					{
						pMaterial.Add(materials[k]);
						if (materials[k].HasProperty("_Color"))
						{
							pAlpha.Add(materials[k].color.a);
						}
						else if (materials[k].HasProperty("_TintColor"))
						{
							pAlpha.Add(materials[k].GetColor("_TintColor").a);
						}
						else
						{
							pAlpha.Add(-1f);
						}
					}
				}
			}
		}
		if (list == null)
		{
			return;
		}
		spriteAlpha = new List<float>();
		SpriteRenderer[] array2 = list;
		foreach (SpriteRenderer spriteRenderer in array2)
		{
			if (spriteRenderer != null)
			{
				spriteAlpha.Add(spriteRenderer.color.a);
			}
		}
	}

	public void Pause()
	{
		if (ps != null)
		{
			ps.Pause(true);
		}
		if (ani != null)
		{
			Animation[] array = ani;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}
	}

	public void Play()
	{
		if (ps != null)
		{
			ps.Play(true);
		}
		if (ani != null)
		{
			Animation[] array = ani;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = true;
			}
		}
	}

	public void Show()
	{
		mlist = GetComponentsInChildren<MeshRenderer>();
		if (mlist != null && mlist.Length != 0)
		{
			MeshRenderer[] array = mlist;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.material.HasProperty("_TintColor"))
				{
					Color color = meshRenderer.material.GetColor("_TintColor");
					if (!(color.a > 0.4f))
					{
						color.a = 0.5f;
						meshRenderer.material.SetColor("_TintColor", color);
					}
				}
				else if (meshRenderer.material.HasProperty("_Color"))
				{
					if (meshRenderer.transform.GetComponent<SkeletonAnimation>() != null)
					{
						meshRenderer.transform.GetComponent<SkeletonAnimation>().valid = false;
						Color color2 = meshRenderer.material.GetColor("_Color");
						color2.a = 1f;
						meshRenderer.material.SetColor("_Color", color2);
						meshRenderer.transform.GetComponent<SkeletonAnimation>().valid = true;
					}
					else
					{
						Color color3 = meshRenderer.material.GetColor("_Color");
						color3.a = 1f;
						meshRenderer.material.color = color3;
						meshRenderer.material.SetColor("_Color", color3);
					}
				}
			}
		}
		if (list != null)
		{
			for (int j = 0; j < list.Length; j++)
			{
				if (list[j] != null)
				{
					Color color4 = list[j].color;
					color4.a = spriteAlpha[j];
					list[j].color = color4;
				}
			}
		}
		if (pMaterial != null)
		{
			for (int k = 0; k < pMaterial.Count; k++)
			{
				if (pMaterial[k].HasProperty("_Color"))
				{
					Color color5 = pMaterial[k].color;
					color5.a = pAlpha[k];
					pMaterial[k].SetColor("_Color", color5);
				}
				else if (pMaterial[k].HasProperty("_TintColor"))
				{
					Color color6 = pMaterial[k].GetColor("_TintColor");
					color6.a = pAlpha[k];
					pMaterial[k].SetColor("_TintColor", color6);
				}
			}
		}
		if (ani != null)
		{
			Animation[] array2 = ani;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = true;
			}
		}
	}

	public void Hide()
	{
		mlist = GetComponentsInChildren<MeshRenderer>();
		if (mlist != null && mlist.Length != 0)
		{
			MeshRenderer[] array = mlist;
			foreach (MeshRenderer meshRenderer in array)
			{
				if (meshRenderer.material.HasProperty("_TintColor"))
				{
					Color color = meshRenderer.material.GetColor("_TintColor");
					if (!(color.a > 0.4f))
					{
						color.a = 0.5f;
						meshRenderer.material.SetColor("_TintColor", color);
					}
				}
				else if (meshRenderer.material.HasProperty("_Color"))
				{
					if (meshRenderer.transform.GetComponent<SkeletonAnimation>() != null)
					{
						Color color2 = meshRenderer.material.GetColor("_Color");
						color2.a = 1f;
						meshRenderer.material.SetColor("_Color", color2);
						meshRenderer.transform.GetComponent<SkeletonAnimation>().valid = true;
					}
					else
					{
						Color color3 = meshRenderer.material.GetColor("_Color");
						color3.a = 1f;
						meshRenderer.material.color = color3;
						meshRenderer.material.SetColor("_Color", color3);
					}
				}
			}
		}
		if (list != null)
		{
			SpriteRenderer[] array2 = list;
			foreach (SpriteRenderer spriteRenderer in array2)
			{
				if (spriteRenderer != null)
				{
					spriteRenderer.DOFade(0f, 0.5f);
				}
			}
		}
		if (pMaterial == null)
		{
			return;
		}
		foreach (Material item in pMaterial)
		{
			if (item != null)
			{
				if (item.HasProperty("_Color"))
				{
					Color color4 = item.color;
					color4.a = 0f;
					item.DOColor(color4, 0.3f);
				}
				else if (item.HasProperty("_TintColor"))
				{
					Color color5 = item.GetColor("_TintColor");
					color5.a = 0f;
					item.DOColor(color5, "_TintColor", 0.3f);
				}
			}
		}
	}
}
