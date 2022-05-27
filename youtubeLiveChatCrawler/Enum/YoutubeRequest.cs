namespace youtubeLiveChatCrawler.Enum
{
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
}