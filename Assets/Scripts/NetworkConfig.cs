public static class NetworkConfig
{
	public static string SaveAllDataAddress = "http://47.89.189.26:801/castlestory/save_data.php";

	public static string SubmitScoreAddress = "http://13.52.11.140:8000/castle_story/submit_level.php?id=";

	public static string GetDataAddress = "http://47.89.189.26:801/castlestory/get_data.php?id=";

	public static string GetRankList = "http://13.52.11.140:8000/castle_story/get_level_rank.php?ids=";

	public static string GetVersionURL = "http://47.89.189.26:801/castlestory/get_version.php?";

	public static string AnalyticsMoneySpendAddress = "http://analytics.playinfinity.cn:8000/analytics/castle_story_level_analysis_android_money_spend_v1.php";

	public static string AnalyticsDataAddress = "http://analytics.playinfinity.cn:8000/analytics/castle_story_level_analysis_android_v2.php";

	public static string AnalyticsCastltNameDataAddress = "http://analytics.playinfinity.cn:8000/analytics/castle_story_upload_castle_name.php";

	public static string AnalyticsCatNameDataAddress = "http://analytics.playinfinity.cn:8000/analytics/castle_story_upload_cat_name.php";

	public static string GetFriendFbApiAddress = "me/friends?fields=picture, name";

	public static string GetMyFbDataAddress = "me?fields=picture,name";

	public static string GetInvitableFriendsAddress = "me/invitable_friends";

	public const string ShareLinkURL_IOS = "";

	public const string GetPermission = "me/permissions";

	public const string bugUrl = "http://analytics.playinfinity.cn:8000/log/upload_error_log_v2.php";

	public const string bugUrl_A = "http://analytics.playinfinity.cn:8000/log/upload_error_log_v1.php";

	public const string bugUrl_I = "http://analytics.playinfinity.cn:8000/log/upload_error_log_v1.php";

	public static bool isSuccessfulGetDataFromService = false;

	public static string GetAdWeightURL = "http://www.play-infinity.cn:801/castlestory/ad_weight_android.json";
}
