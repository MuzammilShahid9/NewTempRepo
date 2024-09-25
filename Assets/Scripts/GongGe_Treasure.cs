using UnityEngine;

public class GongGe_Treasure : MonoBehaviour
{
	public Transform tr;

	private void Open()
	{
		tr.GetComponent<StandByTreasureItem>().Play("open1", false);
		tr.GetComponent<StandByTreasureItem>().ShowPearl();
	}

	private void Close()
	{
		tr.GetComponent<StandByTreasureItem>().Play("close2", false);
		tr.GetComponent<StandByTreasureItem>().Peral.gameObject.SetActive(false);
	}
}
