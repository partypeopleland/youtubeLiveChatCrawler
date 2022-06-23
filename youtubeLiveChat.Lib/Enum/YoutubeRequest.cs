namespace youtubeLiveChat.Lib.Enum;

/// <summary>
/// 發送給 Youtube 請求類型
/// </summary>
public enum YoutubeRequest
{
    /// <summary>
    /// 請求初始資訊
    /// </summary>
    Initial,

    /// <summary>
    /// 第一次請求聊天室內容
    /// </summary>
    Begin,

    /// <summary>
    /// 後續請求聊天室內容
    /// </summary>
    FollowUp
}

/// <summary>
/// Youtube 資源路徑
/// </summary>
public struct YoutubeUri
{
    /// <summary>
    /// 首頁
    /// </summary>
    private const string Home = "https://www.youtube.com";

    /// <summary>
    /// 影片路徑
    /// </summary>
    public const string VideoUrl = Home + "/watch?v={0}";

    /// <summary>
    /// 第一次請求聊天室網址路徑
    /// </summary>
    public const string UrlFirstTime = Home + "/live_chat_replay?continuation={0}";

    /// <summary>
    /// 後續請求聊天室網址路徑
    /// </summary>
    public const string UrlFollowUp = Home + "/youtubei/v1/live_chat/get_live_chat_replay?key={0}";
}