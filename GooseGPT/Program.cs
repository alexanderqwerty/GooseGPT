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
        static readonly string Path = Directory.GetCurrentDirectory() + @"\Assets\Text\NotepadMessages";

        const string prompt =
            "Напиши несколько фраз на бодобие \"Хорошей работы\" \"я делаяю это специально\" Напитши один вариант";

        const string model = "text-davinci-003";


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
            conversation.AppendUserInput("напиши только \"Да\"");
            var response = await conversation.GetResponseFromChatbotAsync();
            using var writer = new StreamWriter(Path + @"\message.txt");
            Console.WriteLine(response);
            writer.Close();
        }

        private static void PreTick(GooseEntity goose)
        {
            if (_flag && goose.currentTask == 3)
            {
                Task.Run(updateText);
                _flag = false;
            }

            if (!_flag && goose.currentTask == 0)
            {
                Console.WriteLine("Start");
                updateText();
                _flag = true;
            }
        }
    }
}