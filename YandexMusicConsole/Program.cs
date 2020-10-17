using System;
using System.Net;
using Yandex.Music.Api;
using Yandex.Music.Api.Common;
using Yandex.Music.Api.Models.Track;

namespace YandexMusicConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string login;
            string password;
            if (args.Length > 0)
            {
                login = args[0];
                password = args[1];
            }
            else
            {
                Console.WriteLine("Введите логин");
                login = Console.ReadLine();
                Console.WriteLine("Введите пароль");
                password = Console.ReadLine();
                Console.Clear();
            }
            DebugSettings debugSettings = new DebugSettings(@"C:\yandex_music", @"C:\yandex_music\log.txt");
            debugSettings.Clear();
            AuthStorage authStorage = new AuthStorage(debugSettings);
            
            authStorage.User.Login = login;
            var api = new YandexMusicApi();
            api.User.Authorize(authStorage, authStorage.User.Login, password);
            if (authStorage.IsAuthorized)
            {
                Console.WriteLine("Успешная авторизация");
            }
            else
            {
                Console.WriteLine("Неверный логин или пароль");
                return;
            }

            Console.WriteLine("Какой трек Вы хотите прослушать?");
            string searchingQuery = Console.ReadLine();
            var search = api.Search.Track(authStorage, searchingQuery);

            if (search.Result.Tracks == null)
            {
                Console.WriteLine("По Вашему запросу ничего не найдено :(");
                return;
            }

            int count = 1;
            foreach (var item in search.Result.Tracks.Results)
            {
                Console.WriteLine($"{count}. {item.Artists[0].Name} - {item.Title}");
                count++;
            }

            Console.WriteLine("Выберите трек из списка:");
            int choice = Convert.ToInt32(Console.ReadLine());
            if (choice < 1 || choice > 20)
            {
                Console.WriteLine("Вы ввели неверное число.");
                return;
            }

            YTrack track = api.Track.Get(authStorage, search.Result.Tracks.Results[choice - 1].Id).Result[0];
            
            var fileLink = api.Track.GetFileLink(authStorage, track);
            
            using (var client = new WebClient())
            {
                client.DownloadFile(fileLink, @$"{debugSettings.OutputDir}\{track.Title}.mp3");
            }
            var audio = new AudioPlayer(authStorage, track);
            audio.Play();

            Console.ReadKey();
        }
    }
}
