using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace SoccerStats
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);

            //var files = directory.GetFiles("*.txt");

            //foreach (var file in files)
            //{
            //    Console.WriteLine(file.Name);
            //}
            //var fileName = Path.Combine(directory.FullName, "data.txt");
            //var file = new FileInfo(fileName);
            //if (file.Exists)
            //{
            //    using (var reader = new StreamReader(file.FullName))
            //    {
            //        Console.SetIn(reader);
            //        Console.WriteLine(Console.ReadLine());
            //    }

            //    Console.ReadLine();
            //}

            var fileName = Path.Combine(directory.FullName, "SoccerGameResults.csv");
            //var fileContents = ReadFile(fileName);
            //string[] fileLines = fileContents.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //foreach (var line in fileLines)
            //{
            //    Console.WriteLine(line);
            //}
            var fileContents = ReadSoccerResults(fileName);

            var playerFileName = Path.Combine(directory.FullName, "players.json");
            var players = DeserializePlayers(playerFileName);

            var topTenPlayers = GetTopTenPlayer(players);

            //foreach (var player in players)
            //{
            //    Console.WriteLine(player.FirstName);
            //}

            foreach (var player in topTenPlayers)
            {
                Console.WriteLine("Name : " + player.FirstName + " PPG : " + player.PointsPerGame);
            }

            fileName = Path.Combine(directory.FullName, "topten.json");
            SerializePlayersToFile(topTenPlayers, fileName);

            Console.WriteLine(GetGoogleHomePage());

        }

        public static string ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        public static List<GameResult> ReadSoccerResults(string fileName)
        {
            var soccerResults = new List<GameResult>();

            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                {
                    var gameResult = new GameResult();
                    string[] values = line.Split(',');
                    
                    DateTime gameDate;
                    if (DateTime.TryParse(values[0], out gameDate))
                    {
                        gameResult.GameDate = gameDate;
                    }
                    gameResult.TeamName = values[1];
                    HomeOrAway homeOrAway;
                    if (Enum.TryParse(values[2], out homeOrAway))
                    {
                        gameResult.HomeOrAway = homeOrAway;
                    }
                    int parseInt;
                    if (int.TryParse(values[3], out parseInt))
                    {
                        gameResult.Goals = parseInt;
                    }
                    if (int.TryParse(values[4], out parseInt))
                    {
                        gameResult.GoalAttempts = parseInt;
                    }
                    if (int.TryParse(values[5], out parseInt))
                    {
                        gameResult.ShotsOnGoal = parseInt;
                    }
                    if (int.TryParse(values[6], out parseInt))
                    {
                        gameResult.ShotsOffGoal = parseInt;
                    }

                    double possesionPercent;
                    if (double.TryParse(values[7], out possesionPercent))
                    {
                        gameResult.PossesionPercent = possesionPercent;
                    }

                    soccerResults.Add(gameResult);
                }
            }

            return soccerResults;
        }

        public static List<Player> DeserializePlayers(string fileName)
        {
            var players = new List<Player>();

            var serializer = new JsonSerializer();

            using (var reader = new StreamReader(fileName))
            using (var jsonReader = new JsonTextReader(reader))
            {
                players = serializer.Deserialize<List<Player>>(jsonReader);
            }


            return players;
        }

        public static List<Player> GetTopTenPlayer(List<Player> players)
        {
            var topTenPlayers = new List<Player>();
            players.Sort(new PlayerComparer());
            int counter = 0;

            foreach (var player in players)
            {
                topTenPlayers.Add(player);
                counter++;

                if (counter == 10)
                    break;
            }

            return topTenPlayers;
        }

        public static void SerializePlayersToFile(List<Player> players,string fileName)
        {
            var serializer = new JsonSerializer();

            using (var writer = new StreamWriter(fileName))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(jsonWriter, players);
            }

        }

        public static string GetGoogleHomePage()
        {
            var webClient = new WebClient();
            byte[] googleHome = webClient.DownloadData("https://www.google.com");

            using (var stream = new MemoryStream(googleHome))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetNewsForPlayer(string playerName)
        {
            var webClient = new WebClient();
            webClient.Headers.Add("key","value");
            byte[] searchResults = webClient.DownloadData(string.Format("https://bingapis.azure-api.net/api/v5/news/search?q={0}&mkt=en-us",playerName));

            using (var stream = new MemoryStream(searchResults))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


    }
}
