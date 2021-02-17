/*
* File: Leaderboard.xaml.cs
* Project: WMP-UWP-TileGame
* Programmer: Jesse Rutledge
* First Version: Dec. 4, 2020
* Description: This file contains all of the code behind for the Leaderboard page that appears after the user has won the sliding puzzle
*/

using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace WMP_UWP_TileGame
{
    /// <summary>
    /// This class is used to populate and display the leaderboard with scores
    /// </summary>
    public sealed partial class Leaderboard : Page
    {
        public static List<string> ScoreTimes;
        public static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Scores.txt");

        public Leaderboard()
        {
            InitializeComponent();
            DisplayScore();
        }

        /// <summary>
        /// This method is used to write the scores to a file in appdata, if the file doesn't already exist it creates it
        /// </summary>
        /// <param name="playerName">This is the player's name</param>
        /// <param name="gametime">This is the amount of time it took for the player to solve the game </param>
        public static void WriteScore(string playerName, TimeSpan gametime)
        {
            ScoreTimes = new List<string>();

            //Check to ensure file exists, if it doesn't create the file
            if (!File.Exists(path)) File.Create(path).Close();

            //Populate ScoreTimes with current list
            ReadScore();

            //Add the newest score and sort the list
            ScoreTimes.Add($"{playerName} - {gametime:mm\\:ss}");
            ScoreTimes.Sort();

            //Write the scores to the file
            using (var streamWriter = new StreamWriter(path, false))
            {
                for (var i = 0; i < ScoreTimes.Count && i < 10; i++) streamWriter.WriteLine(ScoreTimes[i]);
            }
        }

        /// <summary>
        /// This method is used to read the scores from the Scores.txt file
        /// </summary>
        public static void ReadScore()
        {
            string scorestring;

            //Read contents of the file and add them to the scorestring
            using (var streamReader = new StreamReader(path))
            {
                scorestring = streamReader.ReadToEnd();
            }

            //Parse scores from scorestring
            var scores = scorestring.Split("\r\n");

            //Populate ScoreTimes list
            foreach (var line in scores)
            {
                if (line != "")
                {
                    ScoreTimes.Add(line);
                }
            }
        }

        /// <summary>
        /// This method is used to update the LeaderboardBlock text with the scores in ScoreTimes
        /// </summary>
        public void DisplayScore()
        {
            var i = 1;
            LeaderboardBlock.Text = "";
            foreach (var score in ScoreTimes)
            {
                LeaderboardBlock.Text += $"{i}. {score}\n";
                i++;
            }
        }
    }
}