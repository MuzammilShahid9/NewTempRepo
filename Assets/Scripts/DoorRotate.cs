using UnityEngine;
using UnityEngine.Rendering;

public class DoorRotate : MonoBehaviour
{
	public GameObject door;

	public bool isOpening;

	public bool isOpen;

	public bool isClosing;

	public Vector3 openRotation = new Vector3(-90f, 0f, 0f);

	public Vector3 closeRotation = new Vector3(-90f, 90f, 0f);

	private float angleSpeed = 2f;

	private int enterRoleNumber;

	public int sortIndexOffset;

	private float timer;

	private void Start()
	{
		timer = 0f;
		enterRoleNumber = 0;
		isOpening = false;
		isOpen = false;
		isClosing = false;
		door.transform.localRotation = Quaternion.Euler(closeRotation);
		ChangeLayer();
	}

	public void ChangeLayer()
	{
		float y = CameraControl.Instance.defaultCamera.WorldToScreenPoint(door.transform.position).y;
		door.transform.GetComponent<SortingGroup>().sortingOrder = 10000 - (int)(y * 10f) + sortIndexOffset * CameraControl.Instance.defaultCamera.pixelHeight / 720;
	}

	private void Update()
	{
		if (isClosing)
		{
			isOpening = false;
			timer += Time.deltaTime;
			Vector3 euler = Vector3.Lerp(openRotation, closeRotation, angleSpeed * timer);
			door.transform.localRotation = Quaternion.Euler(euler);
			if (door.transform.localRotation == Quaternion.Euler(closeRotation))
			{
				door.transform.localRotation = Quaternion.Euler(closeRotation);
				isClosing = false;
				isOpen = false;
				timer = 0f;
			}
		}
		else if (isOpening)
		{
			isClosing = false;
			timer += Time.deltaTime;
			Vector3 euler2 = Vector3.Lerp(closeRotation, openRotation, angleSpeed * timer);
			door.transform.localRotation = Quaternion.Euler(euler2);
			if (door.transform.localRotation == Quaternion.Euler(openRotation))
			{
				door.transform.localRotation = Quaternion.Euler(openRotation);
				isOpening = false;
				timer = 0f;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			enterRoleNumber++;
			if (!isOpen)
			{
				isOpening = true;
				AudioManager.Instance.PlayAudioEffect("DoorOpen");
				isOpen = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			enterRoleNumber--;
			if (isOpen && enterRoleNumber <= 0)
			{
				AudioManager.Instance.PlayAudioEffect("DoorClose");
				isClosing = true;
			}
		}
	}
}
