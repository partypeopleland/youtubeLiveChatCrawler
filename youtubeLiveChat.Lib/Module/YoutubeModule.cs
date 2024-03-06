using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using youtubeLiveChat.Lib.Enum;
using youtubeLiveChat.Lib.Helper;
using youtubeLiveChat.Lib.Model;

namespace youtubeLiveChat.Lib.Module;

public class YoutubeModule : IYoutubeModule
{
    private readonly HttpClient _client;

    public YoutubeModule(HttpClient httpClient)
    {
        _client = httpClient;
        _client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:59.0) Gecko/20100101 Firefox/59.0");
        _client.DefaultRequestHeaders.Add("Origin", "https://www.youtube.com");
    }

    public VideoInfo GetChatMessages(string videoId, bool saveFile = false)
    {
        try
        {
            var isFirstTime = true;

            var videoInfo = GetVideoInfo(videoId);

            while (true)
            {
                List<ChatMessage> chatMessages;
                string nextContinuation;
                if (isFirstTime)
                {
                    (chatMessages, nextContinuation) = GetChatMessagesFirstTime(videoInfo.Continuation.First());
                    isFirstTime = false;
                }
                else
                {
                    (chatMessages, nextContinuation) = GetChatMessagesFollowUp(videoInfo.ApiKey, videoInfo.Continuation.Last());
                }

                videoInfo.Continuation.Add(nextContinuation);
                videoInfo.ChatMessages.Add(nextContinuation, chatMessages);

                if (chatMessages.Count == 0) break;
            }

            if (saveFile)
            {
                SaveChatMessages(videoInfo);
                SaveChatMessageNames(videoInfo);
            }

            return videoInfo;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private static string ReplaceSpecialChar(string filePath)
    {
        return filePath.Replace(":", "：")
            .Replace("?", "？")
            .Replace("/", "／")
            .Replace("\\", "＼")
            .Replace("*", "＊")
            .Replace("\"", "＂")
            .Replace("<", "＜")
            .Replace(">", "＞");
    }

    private static async void SaveChatMessageNames(VideoInfo videoInfo)
    {
        var names = new List<string>();
        var filePath = ReplaceSpecialChar($"[{videoInfo.VideoId}] {videoInfo.Title} names.txt");
        await using var file = new StreamWriter(filePath);
        foreach (var continuation in videoInfo.Continuation)
        {
            if (!videoInfo.ChatMessages.ContainsKey(continuation)) continue;

            foreach (var chatMessage in videoInfo.ChatMessages[continuation])
            {
                if (names.Contains(chatMessage.AuthorName)) continue;
                names.Add(chatMessage.AuthorName);
            }
        }

        foreach (var name in names)
        {
            await file.WriteLineAsync(name);
        }

        Console.WriteLine($"Total number of names: {names.Count}, Save file success\n" + filePath);
    }

    private static async void SaveChatMessages(VideoInfo videoInfo)
    {
        var count = 0;
        var filePath = ReplaceSpecialChar($"[{videoInfo.VideoId}] {videoInfo.Title} messages.txt");

        await using var file = new StreamWriter(filePath);
        foreach (var continuation in videoInfo.Continuation)
        {
            if (!videoInfo.ChatMessages.ContainsKey(continuation)) continue;


            foreach (var chatMessage in videoInfo.ChatMessages[continuation])
            {
                await file.WriteLineAsync(chatMessage.Line);
            }

            count += videoInfo.ChatMessages[continuation].Count;
        }

        await file.WriteLineAsync($"Total number of messages: {count}");
        Console.WriteLine($"Total number of messages: {count}, Save file success\n" + filePath);
    }

    public VideoInfo GetVideoInfo(string videoId)
    {
        try
        {
            var url = string.Format(YoutubeUri.VideoUrl, videoId);
            var htmlContent = GetHtmlContent(url);

            var ytInitialData = AnalyzeInitData(htmlContent, YoutubeRequest.Initial);
            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ytInitialData);
            var continuation = ParseContinuation(YoutubeRequest.Initial, data);
            var title = ParseTitle(data);

            var ytCfg = AnalyzeYtCfg(htmlContent);
            var dictYtCfg = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ytCfg);
            var apiKey = ParseApiKey(dictYtCfg);

            return new VideoInfo()
            {
                Continuation = new List<string>() { continuation },
                ApiKey = apiKey,
                Title = title,
                VideoId = videoId
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private string ParseTitle(Dictionary<string, object> data)
    {
        var xPath = "contents.twoColumnWatchNextResults.results.results.contents.0.videoPrimaryInfoRenderer.title.runs.0.text";
        return JsonHelper.TryGetValueByXPath(data, xPath, string.Empty).ToString();
    }

    public (List<ChatMessage> chatMessages, string nextContinuation) GetChatMessagesFirstTime(string continuation)
    {
        try
        {
            var url = string.Format(YoutubeUri.UrlFirstTime, continuation);
            var htmlContent = GetHtmlContent(url);
            var ytInitialData = AnalyzeInitData(htmlContent, YoutubeRequest.Begin);
            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(ytInitialData);

            var nextContinuation = ParseContinuation(YoutubeRequest.Begin, data);
            return (ParseChatMessages(data), nextContinuation);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            throw;
        }
    }

    public (List<ChatMessage> chatMessages, string nextContinuation) GetChatMessagesFollowUp(string apiKey, string continuation)
    {
        try
        {
            var url = string.Format(YoutubeUri.UrlFollowUp, apiKey);
            var jsonContent = GetJsonContent(url, continuation);
            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);

            var nextContinuation = ParseContinuation(YoutubeRequest.FollowUp, data);
            return (ParseChatMessages(data), nextContinuation);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            throw;
        }
    }


    private List<ChatMessage> ParseChatMessages(Dictionary<string, dynamic> data)
    {
        var result = new List<ChatMessage>();
        // 從 data 取得聊天紀錄
        JArray actions = data["continuationContents"]["liveChatContinuation"]["actions"];
        if (actions == null) return result;
        if (actions.Count > 0)
        {
            var sb = new StringBuilder();
            foreach (var action in actions)
            {
                var replayChatItemAction = action["replayChatItemAction"];

                var innerAction = replayChatItemAction["actions"][0].ToObject<JObject>();
                var originalActionType = OriginalActionType(innerAction);
                if (originalActionType != "addChatItemAction") continue;

                var originItem = innerAction[originalActionType]["item"].ToObject<JObject>();
                var originMessageType = OriginMessageType(originItem);
                if (originMessageType != "liveChatTextMessageRenderer") continue;


                var messages = originItem[originMessageType]["message"]["runs"].ToArray();
                result.Add(new ChatMessage
                {
                    AuthorName = originItem[originMessageType]["authorName"]["simpleText"].ToString(),
                    Message = CombineMessage(messages, sb),
                    TimeStampText = originItem[originMessageType]["timestampText"]["simpleText"].ToString()
                });
            }
        }

        return result;
    }

    private static string OriginMessageType(JObject originItem)
    {
        return originItem.Properties().Select(p => p.Name).FirstOrDefault();
    }

    private static string OriginalActionType(JObject innerAction)
    {
        return innerAction
            .Properties()
            .Select(p => p.Name)
            .FirstOrDefault(x => x != "clickTrackingParams");
    }

    private static string CombineMessage(JToken[] messages, StringBuilder sb)
    {
        foreach (var message in messages)
        {
            if (message["text"] == null) continue;
            sb.Append(message["text"]);
        }

        // 沒有訊息內容，表示該訊息是 icon，暫時先略過
        var currentChatMessage = sb.ToString();
        sb.Clear();
        if (string.IsNullOrEmpty(currentChatMessage)) currentChatMessage = "<icon>";
        return currentChatMessage;
    }

    private static string GetYoutubeInitDataPattern(YoutubeRequest initial)
    {
        switch (initial)
        {
            case YoutubeRequest.Initial: return "ytInitialData\\s*=\\s*({.+?})\\s*;\\s*</script>";
            case YoutubeRequest.Begin: return "window\\[\"ytInitialData\"\\] = ({.+});\\s*</script>";
            default:
                throw new ArgumentOutOfRangeException(nameof(initial), initial, null);
        }
    }

    private string GetHtmlContent(string url)
    {
        return _client.GetStringAsync(url).Result;
    }

    private string GetJsonContent(string url, string continuation)
    {
        var sendData = new
        {
            context = new
            {
                client = new
                {
                    clientName = "WEB",
                    clientVersion = "2.20220519.09.00"
                }
            },
            continuation
        };
        var content = new StringContent(JsonConvert.SerializeObject(sendData), Encoding.UTF8, "application/json");
        var respMsg = _client.PostAsync(url, content).Result;
        return respMsg.Content.ReadAsStringAsync().Result;
    }

    private static string ParseContinuation(YoutubeRequest requestType, Dictionary<string, dynamic> jsonData)
    {
        var xPath = GetContinuationPattern(requestType);
        return JsonHelper.TryGetValueByXPath(jsonData, xPath, string.Empty).ToString();
    }


    private static string ParseApiKey(Dictionary<string, dynamic> dictYtCfg)
    {
        return JsonHelper.TryGetValue(dictYtCfg, "INNERTUBE_API_KEY", string.Empty).ToString();
    }

    private string AnalyzeInitData(string htmlContent, YoutubeRequest requestType)
    {
        var pattern = GetYoutubeInitDataPattern(requestType);
        var match = Regex.Match(htmlContent, pattern, RegexOptions.Singleline);
        if (!match.Success) throw new Exception($"無法解析 HTML， pattern:{pattern}");

        return match.Groups[1].Value;
    }

    private string AnalyzeYtCfg(string liveChatHtml)
    {
        var ytcfgPattern = "ytcfg\\.set\\(({.+?})\\);";
        var match = Regex.Match(liveChatHtml, ytcfgPattern, RegexOptions.Singleline);
        if (!match.Success) throw new Exception($"無法解析 YtCfg， pattern:{ytcfgPattern}");

        return match.Groups[1].Value;
    }

    private static string GetContinuationPattern(YoutubeRequest requestType)
    {
        switch (requestType)
        {
            case YoutubeRequest.Initial:
                return
                    "contents.twoColumnWatchNextResults.conversationBar.liveChatRenderer.header.liveChatHeaderRenderer.viewSelector.sortFilterSubMenuRenderer.subMenuItems.1.continuation.reloadContinuationData.continuation";
            case YoutubeRequest.Begin:
            case YoutubeRequest.FollowUp:
                return "continuationContents.liveChatContinuation.continuations.0.liveChatReplayContinuationData.continuation";
            default:
                throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
        }
    }
}