using UnityEngine;

public class DoubleDoorRotate : MonoBehaviour
{
	public GameObject leftDoor;

	public GameObject rightDoor;

	public bool isOpening;

	public bool isOpen;

	public bool isClosing;

	public Vector3 leftDoorOpenRotation = new Vector3(-90f, 0f, 0f);

	public Vector3 leftDoorCloseRotation = new Vector3(-90f, 90f, 0f);

	public Vector3 rightDoorOpenRotation = new Vector3(-90f, 0f, 0f);

	public Vector3 rightDoorCloseRotation = new Vector3(-90f, 90f, 0f);

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
		leftDoor.transform.localRotation = Quaternion.Euler(leftDoorCloseRotation);
		rightDoor.transform.localRotation = Quaternion.Euler(rightDoorCloseRotation);
	}

	public void ChangeLayer()
	{
	}

	private void Update()
	{
		if (isClosing)
		{
			isOpening = false;
			timer += Time.deltaTime;
			Vector3 euler = Vector3.Lerp(leftDoorOpenRotation, leftDoorCloseRotation, angleSpeed * timer);
			leftDoor.transform.localRotation = Quaternion.Euler(euler);
			Vector3 euler2 = Vector3.Lerp(rightDoorOpenRotation, rightDoorCloseRotation, angleSpeed * timer);
			rightDoor.transform.localRotation = Quaternion.Euler(euler2);
			if (rightDoor.transform.localRotation == Quaternion.Euler(rightDoorCloseRotation) && leftDoor.transform.localRotation == Quaternion.Euler(leftDoorCloseRotation))
			{
				rightDoor.transform.localRotation = Quaternion.Euler(rightDoorCloseRotation);
				leftDoor.transform.localRotation = Quaternion.Euler(leftDoorCloseRotation);
				isClosing = false;
				isOpen = false;
				timer = 0f;
			}
		}
		else if (isOpening)
		{
			isClosing = false;
			timer += Time.deltaTime;
			Vector3 euler3 = Vector3.Lerp(leftDoorCloseRotation, leftDoorOpenRotation, angleSpeed * timer);
			leftDoor.transform.localRotation = Quaternion.Euler(euler3);
			Vector3 euler4 = Vector3.Lerp(rightDoorCloseRotation, rightDoorOpenRotation, angleSpeed * timer);
			rightDoor.transform.localRotation = Quaternion.Euler(euler4);
			if (rightDoor.transform.localRotation == Quaternion.Euler(rightDoorOpenRotation) && leftDoor.transform.localRotation == Quaternion.Euler(leftDoorOpenRotation))
			{
				rightDoor.transform.localRotation = Quaternion.Euler(rightDoorOpenRotation);
				leftDoor.transform.localRotation = Quaternion.Euler(leftDoorOpenRotation);
				isOpening = false;
				timer = 0f;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		DebugUtils.Log(DebugType.Other, "OnTriggerEnter");
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
