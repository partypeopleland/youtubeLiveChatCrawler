namespace youtubeLiveChat.Lib.Model;

public class VideoInfo
{
    /// <summary>
    /// continuation token 集合
    /// </summary>
    public List<string> Continuation { get; set; }

    /// <summary>
    /// 影片標題
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// apiKey
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// 聊天訊息 mapping continuation
    /// </summary>
    public Dictionary<string, List<ChatMessage>> ChatMessages { get; } = new Dictionary<string, List<ChatMessage>>();

    /// <summary>
    /// video id
    /// </summary>
    public string VideoId { get; set; }
}