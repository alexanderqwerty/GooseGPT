using System;
using GooseShared;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using OpenAI_API;


namespace GooseGPT
{
    public class ModMain : IMod
    {
        const string OpenAIApiKey = "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";
        static readonly string path = Directory.GetCurrentDirectory() + @"\Assets\Text\NotepadMessages";

        const string prompt =
            "Desktop Goose is a performance reducing program created by independent developer Samperson. It imitates a cute goose on your desktop, but it's not cute at all. The goose will constantly wreak havoc on your computer by chasing your cursor, shifting your windows, and launching the Notepad app while you're playing a game. Inspired by Untitled Goose Game and Skatebird, Desktop Goose is another app designed to boost your productivity. Write a phrase that a goose could say";

        public void Init()
        {
            InjectionPoints.PostTickEvent += PreTick;
        }

        private static bool _flag = true;

        private static async void updateText()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var openAI = new OpenAIAPI(new APIAuthentication(OpenAIApiKey));
            var conversation = openAI.Chat.CreateConversation();
            conversation.AppendUserInput(prompt);
            var response = await conversation.GetResponseFromChatbotAsync();
            using var writer = new StreamWriter(path + @"\messagegpt.txt");
            await writer.WriteLineAsync(response);
            writer.Close();
        }

        private static void PreTick(GooseEntity goose)
        {
            if (_flag && goose.currentTask == 3)
            {
                _flag = false;
            }

            if (!_flag && goose.currentTask == 0)
            {
                Console.WriteLine("Start");
                Task.Run(updateText);
                _flag = true;
            }
        }
    }
}