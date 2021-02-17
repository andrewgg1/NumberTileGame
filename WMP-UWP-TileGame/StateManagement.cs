/*
 *	FILE				: StateManagement.xaml.cs
 *	PROJECT				: Windows and Mobile Programming PROG2121 - Assignment 7
 *	PROGRAMMER			: Andrew Gordon, Jesse Rutledge
 *	FIRST VERSION       : Dec. 4, 2020
 *  LAST UPDATE         : Dec. 14, 2020
 *	DESCRIPTION			: Contains the code for managing the state management / resume functionalty
 */

using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WMP_UWP_TileGame
{
    /* ------------------------------------------------------------------------------------
    CLASS NAME  :	StateManagement
    PURPOSE     :	The purpose of this class is to provide functionality for the state
                    management / resume

                    Contains method(s):
                    - App_Resuming()
                    - AddButtonsToArray()

    ------------------------------------------------------------------------------------ */
    class StateManagement
    {
        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public static Point EmptySquare;      // holds the coordinates of the empty square
        public static Button[] buttonArray = new Button[15];   // array of all the buttons on the grid
        public static DispatcherTimer gameTimer;
        public static int GameTime;
        public static string PlayerName;
        public static bool WasSuspended;

        /*  -- Method Header Comment
        Name	:	App_Resuming
        Purpose :	Resumes all necessary data
        Inputs	:	MainPage main   the page to assign all the retrieved values
        Outputs	:	None
        Returns	:	None
        */
        public static void App_Resuming(MainPage main)
        {
            AddButtonsToArray(main);

            var i = 1;
            //Assign the contents and coordinates of the button by iterating through the composite keys of localSettings
            foreach (var btn in buttonArray)
            {
                var composite = (ApplicationDataCompositeValue)localSettings.Values[$"button {i}"];

                if (composite != null)
                {
                    btn.Content = composite[$"{i}"];
                    var intXPos = (int) composite["xPos"];
                    var intYPos = (int) composite["yPos"];

                    var xPos = Convert.ToDouble(intXPos);
                    var yPos = Convert.ToDouble(intYPos);

                    btn.Margin = new Thickness(xPos, yPos, 0, 0);
                }

                i++;
            }

            //Assign various state related objects based off local settings
            main.NameBox.Text = PlayerName = localSettings.Values["playerName"].ToString();
            GameTime = (int) localSettings.Values["currentTime"];
            main.TimerBlock.Text = TimeSpan.FromSeconds(GameTime).ToString();
            WasSuspended = (bool) localSettings.Values["wasSuspended"];
            EmptySquare = (Point) localSettings.Values["emptySquare"];
            main.StartButton.IsEnabled = true;
            main.StartButton.Content = "Resume";

            //Purge settings now that they've been used
            localSettings.Values.Remove("playerName");
            localSettings.Values.Remove("currentTime");
            localSettings.Values.Remove("wasSuspended");
            localSettings.Values.Remove("emptySquare");

        }

        /*  -- Method Header Comment
        Name	:	AddButtonsToArray
        Purpose :	Adds all of the buttons in MainPage to an array
        Inputs	:	MainPage main   the page to assign all the retrieved values
        Outputs	:	None
        Returns	:	None
        */
        public static void AddButtonsToArray(MainPage main)
        {
            buttonArray[0] = main.Button1;
            buttonArray[1] = main.Button2;
            buttonArray[2] = main.Button3;
            buttonArray[3] = main.Button4;
            buttonArray[4] = main.Button5;
            buttonArray[5] = main.Button6;
            buttonArray[6] = main.Button7;
            buttonArray[7] = main.Button8;
            buttonArray[8] = main.Button9;
            buttonArray[9] = main.Button10;
            buttonArray[10] = main.Button11;
            buttonArray[11] = main.Button12;
            buttonArray[12] = main.Button13;
            buttonArray[13] = main.Button14;
            buttonArray[14] = main.Button15;
        }
    }
}
