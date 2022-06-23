using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using youtubeLiveChat.Lib.Module;

namespace youtubeLiveChat.Tests;

[TestClass]
public class YoutubeModuleIntergrationTest
{
    /// <summary>
    /// 測試取得聊天室訊息(全部)
    /// </summary>
    [TestMethod]
    public void CrawlLiveChatHistoryFile()
    {
        var videoId = "CxNm-3EedyA";
        var sut = new YoutubeModule(new HttpClient());

        sut.GetChatMessages(videoId, true);
    }
}