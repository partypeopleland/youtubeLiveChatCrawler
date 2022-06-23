using youtubeLiveChat.Lib.Model;

namespace youtubeLiveChat.Lib.Module;

public interface IYoutubeModule
{
    /// <summary>
    /// 取得聊天室訊息
    /// </summary>
    /// <param name="videoId">Youtube 影片代碼</param>
    /// <param name="saveFile">是否儲存聊天紀錄檔案</param>
    /// <returns></returns>
    VideoInfo GetChatMessages(string videoId, bool saveFile);

    /// <summary>
    /// 取得 continuation 及 ApiKey
    /// </summary>
    /// <param name="videoId">Youtube 影片代碼</param>
    /// <returns></returns>
    VideoInfo GetVideoInfo(string videoId);

    /// <summary>
    /// 取得初始聊天室訊息
    /// </summary>
    /// <param name="url">聊天室初始網址</param>
    /// <returns></returns>
    (List<ChatMessage> chatMessages, string nextContinuation) GetChatMessagesFirstTime(string url);

    /// <summary>
    /// 取得後續聊天室訊息
    /// </summary>
    /// <param name="apiKey">ApiKey</param>
    /// <param name="continuation">取得後續聊天內容的 token</param>
    /// <returns></returns>
    (List<ChatMessage> chatMessages, string nextContinuation) GetChatMessagesFollowUp(string apiKey, string continuation);
}