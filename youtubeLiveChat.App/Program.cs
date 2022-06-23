using youtubeLiveChat.Lib.Module;

namespace youtubeLiveChat.App
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please input youtube video id, like 'CxNm-3EedyA'");
                return;
            }

            var videoId = args[0];
            var youtubeModule = new YoutubeModule(new HttpClient());
            youtubeModule.GetChatMessages(videoId, true);
        }
    }
}