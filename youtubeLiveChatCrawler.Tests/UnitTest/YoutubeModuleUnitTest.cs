using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using youtubeLiveChatCrawler.Enum;
using youtubeLiveChatCrawler.Model;
using youtubeLiveChatCrawler.Module;
using youtubeLiveChatCrawler.Tests.FakeData;

namespace youtubeLiveChatCrawler.Tests.UnitTest
{
    [TestClass]
    public class YoutubeModuleTest
    {
        /// <summary>
        /// 被測試系統
        /// </summary>
        private IYoutubeModule _sut;

        /// <summary>
        /// 模擬 httpMessage handler
        /// </summary>
        private MockHttpMessageHandler _mockHandler;

        /// <summary>
        /// 當前 continuation
        /// </summary>
        private string _currentContinuation;

        /// <summary>
        /// ApiKey
        /// </summary>
        private string _apiKey;

        /// <summary>
        /// 影片代碼
        /// </summary>
        private string _videoId;

        /// <summary>
        /// 下一個 continuation
        /// </summary>
        private string _nextContinuation;

        private VideoInfo _videoInfo;

        [TestMethod]
        public void CheckGetInitialData()
        {
            GivenVideoId("CxNm-3EedyA");
            WhenGetInitialData();
            ThenResponseContinuationShouldBe("op2w0wRgGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCQAFyAggB");
            ThenResponseApiKeyShouldBe("AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8");
        }


        [TestMethod]
        public void CheckGetFirstTimeContinuation()
        {
            GivenFirstTimeRequestData("op2w0wRgGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCQAFyAggB");
            WhenGetFirstTimeChatMessages();
            ThenResponseContinuationShouldBe("op2w0wRxGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCKOq1sMADQABIA1IFIACwAQByAggBeAA%3D");
        }

        [TestMethod]
        public void CheckGetFollowUpContinuation()
        {
            GivenFollowUpRequestData(
                "op2w0wRxGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCKOq1sMADQABIA1IFIACwAQByAggBeAA%3D",
                "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8"
            );
            WhenGetFollowUpChatMessages();
            ThenResponseContinuationShouldBe("op2w0wRxGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCKMf5gNsKQABIA1IFIACwAQByAggBeAA%3D");
        }

        [TestMethod]
        public void CheckGetFollowUpAtLastContinuation()
        {
            GivenFollowUpRequestData(
                "op2w0wRxGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCKMy3stYOQABIA1IFIACwAQByAggBeAA%3D",
                "AIzaSyAO_FJ2SlqU8Q4STEHLGCilw_Y9_11qcW8"
            );
            WhenGetFollowUpChatMessages();
            ThenResponseContinuationShouldBe(string.Empty);
        }

        private void GivenVideoId(string videoId)
        {
            _videoId = videoId;
            _mockHandler = MockFakeHtmlResponseByVideoId(_videoId);
        }

        private void GivenFirstTimeRequestData(string continuation)
        {
            _currentContinuation = continuation;
            _mockHandler = MockFakeHtmlResponseByContinuation(this._currentContinuation);
        }

        private void GivenFollowUpRequestData(string continuation, string apiKey)
        {
            _apiKey = apiKey;
            _currentContinuation = continuation;
            _mockHandler = MockFakeJsonResponse(_apiKey, _currentContinuation);
        }

        private void WhenGetInitialData()
        {
            _sut = new YoutubeModule(_mockHandler.ToHttpClient());
            _videoInfo = _sut.GetVideoInfo(_videoId);
            _nextContinuation = _videoInfo.Continuation.Last();
            _apiKey = _videoInfo.ApiKey;
        }

        private void WhenGetFirstTimeChatMessages()
        {
            _sut = new YoutubeModule(_mockHandler.ToHttpClient());
            (_, _nextContinuation) = this._sut.GetChatMessagesFirstTime(this._currentContinuation);
        }

        private void WhenGetFollowUpChatMessages()
        {
            _sut = new YoutubeModule(_mockHandler.ToHttpClient());
            (_, _nextContinuation) = _sut.GetChatMessagesFollowUp(_apiKey, _currentContinuation);
        }

        private void ThenResponseContinuationShouldBe(string expected)
        {
            Assert.AreEqual(expected, _nextContinuation);
        }

        private void ThenResponseApiKeyShouldBe(string expected)
        {
            Assert.AreEqual(expected, _apiKey);
        }

        private MockHttpMessageHandler MockFakeHtmlResponseByVideoId(string videoId)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(string.Format(YoutubeUri.VideoUrl, videoId))
                .Respond("text/html", YoutubeFakeData.GetVideoHtmlContentByVideoId(videoId));
            return mockHttp;
        }

        private MockHttpMessageHandler MockFakeHtmlResponseByContinuation(string continuation)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(string.Format(YoutubeUri.UrlFirstTime, continuation))
                .Respond("text/html", YoutubeFakeData.GetVideoHtmlContentByContinuation());
            return mockHttp;
        }

        private MockHttpMessageHandler MockFakeJsonResponse(string apiKey, string continuation)
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp
                .When(string.Format(YoutubeUri.UrlFollowUp, apiKey))
                .Respond("application/json", YoutubeFakeData.GetVideoJsonContentByContinuation(continuation));
            return mockHttp;
        }
    }
}