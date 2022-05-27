namespace youtubeLiveChatCrawler.Model
{
    /// <summary>
    /// 聊天訊息
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// 發言人
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// 訊息文字內容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 聊天訊息
        /// </summary>
        public string Line => $"[{TimeStampText}]{AuthorName}: {Message}";

        /// <summary>
        /// 發言時間
        /// </summary>
        public string TimeStampText { get; set; }
    }
}