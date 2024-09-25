using UnityEngine;

public class TransporterManager : MonoBehaviour
{
	public static TransporterManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	public void GetTransporterPathData()
	{
	}
}
