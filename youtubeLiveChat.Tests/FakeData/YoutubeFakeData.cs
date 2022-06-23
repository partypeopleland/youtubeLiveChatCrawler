using System.Collections.Generic;
using System.IO;

namespace youtubeLiveChatCrawler.Tests.FakeData
{
    public static class YoutubeFakeData
    {
        private static readonly Dictionary<string, string> FileLookup = new Dictionary<string, string>()
        {
            { "op2w0wRxGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCKOq1sMADQABIA1IFIACwAQByAggBeAA%3D", "./FakeData/03.json" },
            { "op2w0wRxGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCKMf5gNsKQABIA1IFIACwAQByAggBeAA%3D", "./FakeData/04.json" },
            { "op2w0wRxGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCKPS7yNMNQABIA1IFIACwAQByAggBeAA%3D", "./FakeData/05.json" },
            { "op2w0wRxGlhDaWtxSndvWVZVTkZURGc0TnpGeFJrVmhhM0J4V1hCM1FsTnFTRTVCRWd0RGVFNXRMVE5GWldSNVFSb1Q2cWpkdVFFTkNndERlRTV0TFRORlpXUjVRU0FCKMy3stYOQABIA1IFIACwAQByAggBeAA%3D", "./FakeData/06.json" },
        };

        public static string GetVideoHtmlContentByVideoId(string videoId)
        {
            return File.ReadAllText($"./FakeData/{videoId}.html");
        }

        public static string GetVideoHtmlContentByContinuation()
        {
            return File.ReadAllText("./FakeData/first.html");
        }

        public static string GetVideoJsonContentByContinuation(string continuation)
        {
            return FileLookup.ContainsKey(continuation) 
                ? File.ReadAllText(FileLookup[continuation]) 
                : string.Empty;
        }
    }
}