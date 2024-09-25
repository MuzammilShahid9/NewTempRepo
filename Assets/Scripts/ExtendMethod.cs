using DG.Tweening;
using UnityEngine;

public static class ExtendMethod
{
	public static Tweener DOGameTweenMove(this Transform tran, Vector3 endValue, float duration, bool snapping = false)
	{
		Tweener tweener = tran.DOMove(endValue, duration, snapping);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameTweenLocalMove(this Transform tran, Vector3 endValue, float duration, bool snapping = false)
	{
		Tweener tweener = tran.DOLocalMove(endValue, duration, snapping);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameTweenScale(this Transform tran, Vector3 endValue, float duration)
	{
		Tweener tweener = tran.DOScale(endValue, duration);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameTweenFade(this SpriteRenderer render, float endValue, float duration)
	{
		Tweener tweener = render.DOFade(endValue, duration);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameTweenColor(this Material material, Color endValue, float duration)
	{
		Tweener tweener = material.DOColor(endValue, duration);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameTweenOffset(this Material material, Vector2 endValue, float duration)
	{
		Tweener tweener = material.DOOffset(endValue, duration);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameTweenColor(this Material material, Color endValue, string property, float duration)
	{
		Tweener tweener = material.DOColor(endValue, property, duration);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameLocalPath(this Transform transform, Vector3[] path, float duration, PathType pathType = PathType.Linear)
	{
		Tweener tweener = transform.DOPath(path, duration, pathType);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Sequence DOGameLocalJump(this Transform transform, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
	{
		Sequence sequence = transform.DOLocalJump(endValue, jumpPower, numJumps, duration, snapping);
		UpdateManager.Instance.AddSequenceToList(sequence);
		return sequence;
	}

	public static Tweener DOGameLocalPunchScale(this Transform transform, Vector3 punch, float duration, int vibrato = 10)
	{
		Tweener tweener = transform.DOPunchScale(punch, duration, vibrato);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameLocalPunchRotation(this Transform transform, Vector3 punch, float duration, int vibrato = 10)
	{
		Tweener tweener = transform.DOPunchRotation(punch, duration, vibrato);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}

	public static Tweener DOGameLocalShakeScale(this Transform transform, float duration, float strength, int vibrato = 10)
	{
		Tweener tweener = transform.DOShakeScale(duration, strength, vibrato);
		UpdateManager.Instance.AddTweenToList(tweener);
		return tweener;
	}
}
