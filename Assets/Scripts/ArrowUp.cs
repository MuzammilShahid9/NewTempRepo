using DG.Tweening;
using UnityEngine;

public class ArrowUp : MonoBehaviour
{
	private Vector3 startPosition;

	private void Start()
	{
		startPosition = base.transform.localPosition;
		base.transform.DOLocalMoveY(startPosition.y + 30f, 1f).SetLoops(-1, LoopType.Yoyo);
	}

	private void Update()
	{
	}
}
