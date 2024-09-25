using UnityEngine;
using UnityEngine.Rendering;

public class ChangeSortingIndex : MonoBehaviour
{
	public int sortIndexOffset;

	public bool isSortingGroup;

	public bool isChangeSingle;

	public SpriteRenderer targetGameObject;

	public SortingGroup changeTarget;

	private void Start()
	{
		ChangeLayer();
	}

	public void ChangeLayer()
	{
		if (targetGameObject != null)
		{
			ChangeLayerWithTargetObject();
		}
		else if (changeTarget != null)
		{
			float y = CameraControl.Instance.defaultCamera.WorldToScreenPoint(base.transform.position).y;
			int num = base.transform.GetComponent<ChangeSortingIndex>().sortIndexOffset;
			changeTarget.sortingOrder = 10000 - (int)(y * 10f) + (num + sortIndexOffset) * CameraControl.Instance.defaultCamera.pixelHeight / 720;
		}
		else
		{
			ChangeLayerWithTransform(base.transform);
		}
	}

	public void ChangeLayerWithTargetObject()
	{
		float y = CameraControl.Instance.defaultCamera.WorldToScreenPoint(targetGameObject.transform.position).y;
		int num = targetGameObject.GetComponent<ChangeSortingIndex>().sortIndexOffset;
		SpriteRenderer component = base.transform.GetComponent<SpriteRenderer>();
		if (component != null)
		{
			component.sortingOrder = 10000 - (int)(y * 10f) + (num + sortIndexOffset) * CameraControl.Instance.defaultCamera.pixelHeight / 720;
		}
		else
		{
			base.transform.GetComponent<SortingGroup>().sortingOrder = 10000 - (int)(y * 10f) + (num + sortIndexOffset) * CameraControl.Instance.defaultCamera.pixelHeight / 720;
		}
	}

	public void ChangeLayerWithTransform(Transform trans)
	{
		if (isSortingGroup)
		{
			float y = CameraControl.Instance.defaultCamera.WorldToScreenPoint(base.transform.position).y;
			trans.GetComponent<SortingGroup>();
			trans.GetComponent<SortingGroup>().sortingOrder = 10000 - (int)(y * 10f) + sortIndexOffset * CameraControl.Instance.defaultCamera.pixelHeight / 720;
		}
		SpriteRenderer component = trans.GetComponent<SpriteRenderer>();
		if (component != null)
		{
			float y2 = CameraControl.Instance.defaultCamera.WorldToScreenPoint(component.transform.position).y;
			component.sortingOrder = 10000 - (int)(y2 * 10f) + sortIndexOffset * CameraControl.Instance.defaultCamera.pixelHeight / 720;
		}
		if (isChangeSingle)
		{
			return;
		}
		foreach (Transform tran in trans)
		{
			if (tran != base.transform)
			{
				ChangeLayerWithTransform(tran);
			}
		}
	}
}
