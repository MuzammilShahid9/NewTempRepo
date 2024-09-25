using UnityEngine;
using UnityEngine.UI;

public class RewardItem : MonoBehaviour
{
	public Image image;

	public GameObject unlimitImage;

	public Text detailText;

	public RewardInfo rewardInfos;

	private string[] imageNameArray = new string[8] { "Elements2/heart", "GameElements/area_bomb", "GameElements/color_bomb", "Elements2/DoubleBee", "Elements2/Spoon", "Elements2/hammer", "Elements2/glove", "Elements2/icon-01jinbi" };

	public void Enter(string awardString)
	{
		rewardInfos = new RewardInfo();
		if (!(awardString != ""))
		{
			return;
		}
		string[] array = awardString.Split(',');
		if (array[0] == "0")
		{
			float num = int.Parse(array[1]);
			rewardInfos.rewardType = (RewardType)int.Parse(array[0]);
			rewardInfos.rewardNum = int.Parse(array[1]);
			if (num < 60f)
			{
				detailText.text = num + ":00";
			}
			else
			{
				int num2 = (int)num / 60;
				int num3 = (int)num % 60;
				detailText.text = num2 + ":" + num3.ToString().PadLeft(2, '0') + ":00";
			}
			unlimitImage.SetActive(true);
			Sprite sprite = Resources.Load<GameObject>("Textures/" + imageNameArray[int.Parse(array[0])]).GetComponent<SpriteRenderer>().sprite;
			image.GetComponent<RectTransform>().sizeDelta = new Vector2(73f, 60f);
			image.sprite = sprite;
			return;
		}
		if (array[0] == "7")
		{
			detailText.text = array[1];
			detailText.fontSize = 30;
		}
		else
		{
			detailText.text = "x" + array[1];
		}
		unlimitImage.SetActive(false);
		rewardInfos.rewardType = (RewardType)int.Parse(array[0]);
		rewardInfos.rewardNum = int.Parse(array[1]);
		Sprite sprite2 = Resources.Load<GameObject>("Textures/" + imageNameArray[int.Parse(array[0])]).GetComponent<SpriteRenderer>().sprite;
		image.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite2.textureRect.width, sprite2.textureRect.height);
		if (array[0] == "7")
		{
			image.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 90f);
		}
		image.sprite = sprite2;
	}
}
