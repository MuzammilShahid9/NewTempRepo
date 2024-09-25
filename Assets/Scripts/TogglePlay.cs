using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TogglePlay : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private Toggle toggle;

	private void Start()
	{
		toggle = GetComponent<Toggle>();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left && toggle.interactable)
		{
			AudioManager.Instance.PlayAudioEffect("general_button");
		}
	}
}
