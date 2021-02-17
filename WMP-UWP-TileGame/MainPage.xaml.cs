/*
 *	FILE				: MainPage.xaml.cs
 *	PROJECT				: Windows and Mobile Programming PROG2121 - Assignment 7
 *	PROGRAMMER			: Andrew Gordon, Jesse Rutledge
 *	FIRST VERSION       : Dec. 4, 2020
 *  LAST UPDATE         : Dec. 14, 2020
 *	DESCRIPTION			: Contains the code behind for the Main Game Page
 */

using System;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using SuspendingEventArgs = Windows.ApplicationModel.SuspendingEventArgs;
using SuspendingEventHandler = Windows.UI.Xaml.SuspendingEventHandler;

namespace WMP_UWP_TileGame
{
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;     // local settings
        private bool _firstGame = true;     // flag if the current game is the first one to be played

        public MainPage()
        {
            InitializeComponent();
            // set suspending method
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

            // setup the game timer
            StateManagement.gameTimer = new DispatcherTimer();
            StateManagement.gameTimer.Tick += GameTimer_Tick;
            StateManagement.gameTimer.Interval = TimeSpan.FromSeconds(1);
        }

        /*  -- Method Header Comment
        Name	:	App_Suspending
        Purpose :	Suspends all necessary data
        Inputs	:	object sender, SuspendingEventArgs e
        Outputs	:	None
        Returns	:	None
        */
        private void App_Suspending(object sender, SuspendingEventArgs e)
        {
            // loop through each button in the array
            foreach (var btn in StateManagement.buttonArray)
            {
                // save the button content and x, y positions
                var btnNum = (int) btn.Content;
                var xPos = (int) btn.Margin.Left;
                var yPos = (int) btn.Margin.Top;

                // create a new composite store
                var composite = new ApplicationDataCompositeValue
                {
                    ["xPos"] = xPos, ["yPos"] = yPos, [btnNum.ToString()] = btnNum
                };
                // save to the composite
                localSettings.Values[$"button {btnNum}"] = composite;
            }

            // save required values to localsettings
            localSettings.Values["playerName"] = StateManagement.PlayerName;
            localSettings.Values["currentTime"] = StateManagement.GameTime;
            StateManagement.gameTimer.Stop();
            localSettings.Values["wasSuspended"] = true;
            localSettings.Values["emptySquare"] = StateManagement.EmptySquare;
        }

        /*  -- Method Header Comment
        Name	:	GameTimer_Tick
        Purpose :	Increments the game timer and redisplays it
        Inputs	:	object sender, SuspendingEventArgs e
        Outputs	:	None
        Returns	:	None
        */
        private void GameTimer_Tick(object sender, object e)
        {
            // increment timer and display
            StateManagement.GameTime++;
            TimerBlock.Text = TimeSpan.FromSeconds(StateManagement.GameTime).ToString();
        }

        /*  -- Method Header Comment
        Name	:	StartButton_Click
        Purpose :	Called when the start button is clicked
        Inputs	:	object sender, SuspendingEventArgs e
        Outputs	:	None
        Returns	:	None
        */
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!StateManagement.WasSuspended)
            {
                if(_firstGame)
                {
                    // add all the buttons into the array
                    StateManagement.AddButtonsToArray(this);
                    // initalize the coordinates of the empty square, which will always start in the bottom right corner
                    StateManagement.EmptySquare.X = 600;
                    StateManagement.EmptySquare.Y = 600;
                }

                // assign a randomized number to each button
                AssignButtonValues();

                // assign the player name and init game time
                StateManagement.PlayerName = NameBox.Text;
                StateManagement.GameTime = 0;
            }
            else
            {
                // reset suspended flag after the game has resumed
                StateManagement.WasSuspended = false;
            }
            // enable buttons when the start button is clicked
            foreach (var b in StateManagement.buttonArray)
            {
                b.IsEnabled = true;
            }
            // reset firstGame flag
            _firstGame = false;
            // display the timer and start it
            TimerBlock.Text = TimeSpan.FromSeconds(StateManagement.GameTime).ToString();
            StateManagement.gameTimer.Start();
            // modify button contents
            StartButton.Content = "Restart";
        }

        /*  -- Method Header Comment
        Name	:	AssignButtonValues
        Purpose :	Assigns randomized values to the buttons
        Inputs	:	None
        Outputs	:	None
        Returns	:	None
        */
        private static void AssignButtonValues()
        {
            // generate an array of values to assign to the buttons, assuring it is solvable
            var randArray = GenerateRandomSolveableArray();

            // assign a number value to each button
            var i = 0;
            foreach (var b in StateManagement.buttonArray)
            {
                b.Content = randArray[i++];
            }
        }

        /*  -- Method Header Comment
        Name	:	GenerateRandomSolveableArray
        Purpose :	Generates a random arrray of values that is solvable
        Inputs	:	None
        Outputs	:	int[]   array of randomized values
        Returns	:	None
        */
        private static int[] GenerateRandomSolveableArray()
        {
            var solveable = false;     // flag if the array has been confirmed to be solvable
            int[] randArray = null;     // the randomized array of button values

            // loop until a solveable array is found
            while (!solveable)
            {
                // default values to be solved
                int[] numArray = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 };
                var rnd = new Random();
                // randomize the array
                randArray = numArray.OrderBy(x => rnd.Next()).ToArray();

                // set the last value of the array to 0, indicating the initial empty square on the board
                var index = Array.FindIndex(randArray, e => e == 0);
                randArray[index] = randArray[randArray.Length - 1];
                randArray[randArray.Length - 1] = 0;

                // check the array is solvable
                solveable = CheckArrayIsSolvable(randArray);
            }

            return randArray;
        }

        // this function slightly modified taken from
        // https://www.geeksforgeeks.org/csharp-program-for-count-inversions-in-an-array-set-1-using-merge-sort/
        /*  -- Method Header Comment
        Name	:	CheckArrayIsSolvable
        Purpose :	Checks the passed array contains is a solvable game
        Inputs	:	int[] arr
        Outputs	:	None
        Returns	:	bool true if the array is solvable
                         false if the array is not solvable
        */
        private static bool CheckArrayIsSolvable(int[] arr)
        {
            // checks the number of inversions present in the array
            var inv_count = 0;
            var n = arr.Length;
            for (var i = 0; i < n - 1; i++)
                for (var j = i + 1; j < n; j++)
                    if ((arr[j] != 0) && (arr[i] != 0) && (arr[i] > arr[j]))
                        inv_count++;
            // if there is an even number of inversions, the array is solvable
            return inv_count % 2 == 0;
        }

        /*  -- Method Header Comment
        Name	:	Button_Click
        Purpose :	Called when a game button is clicked
        Inputs	:	object sender, RoutedEventArgs e
        Outputs	:	None
        Returns	:	None
        */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonMove((Button)sender);
        }

        /*  -- Method Header Comment
        Name	:	ButtonMove
        Purpose :	Attempts to move the button to a the empty square
        Inputs	:	Button clickedBtn   the button that has been clicked
        Outputs	:	None
        Returns	:	None
        */
        private void ButtonMove(Button clickedBtn)
        {
            // get the x and y coordinates of the selected button
            var xPos = (int)clickedBtn.Margin.Left;
            var yPos = (int)clickedBtn.Margin.Top;

            // if the button is to the left or right of the empty square
            if (((xPos == StateManagement.EmptySquare.X - 200 || xPos == StateManagement.EmptySquare.X + 200) && yPos == StateManagement.EmptySquare.Y) ||
                // if the button is above or below the empty square
                (yPos == StateManagement.EmptySquare.Y - 200 || yPos == StateManagement.EmptySquare.Y + 200) && xPos == StateManagement.EmptySquare.X)
            {
                // save the button's current coordinates
                Point swap;
                swap.X = xPos;
                swap.Y = yPos;

                // set the button's new location to where the Empty Square was
                clickedBtn.Margin = new Thickness(StateManagement.EmptySquare.X, StateManagement.EmptySquare.Y, 0, 0);

                // set the empty square to where the button was previously
                StateManagement.EmptySquare = swap;
            }
            // if the bottom right square is empty, check if the buttons are in the correct order
            if (StateManagement.EmptySquare.X == 600 && StateManagement.EmptySquare.Y == 600)
            {
                CheckForWin();
            }

        }

        /*  -- Method Header Comment
        Name	:	CheckForWin
        Purpose :	Checks if the player has won based on the locations of all the buttons
        Inputs	:	None
        Outputs	:	None
        Returns	:	None
        */
        private void CheckForWin()
        {
            var count = 0;      // counter to determine how many buttons are in the correct position

            foreach (var btn in StateManagement.buttonArray)
            {
                // get button content (number value seen on screen)
                var btnNum = (int)btn.Content;

                // get the button coordinates
                var xPos = (int)btn.Margin.Left;
                var yPos = (int)btn.Margin.Top;

                // get the index of the button that is supposed to be in the square
                var index = (yPos / 200) * 4 + xPos / 200;      // index of the button
                // if the index matches the button value -1, then the button is in the correct position
                if (index == btnNum - 1)
                {
                    // increase counter
                    count++;
                }
            }
            // if the counted correct buttons match the total number of buttons
            if (count != StateManagement.buttonArray.Length) return;
            StateManagement.gameTimer.Stop();
            Leaderboard.WriteScore(StateManagement.PlayerName, TimeSpan.FromSeconds(StateManagement.GameTime));
            // display a popup dialog message
            WinMessage();
            ViewLeaderboard();
            StartButton.Content = "Play Again?";
        }

        /*  -- Method Header Comment
        Name	:	WinMessage
        Purpose :	Displays a "You Won" message when the user wins the game
        Inputs	:	Button clickedBtn   the button that has been clicked
        Outputs	:	MessageDialog notifying the user they won
        Returns	:	None
        */
        private static async void WinMessage()
        {
            // set and display win message
            var messageDialog = new MessageDialog("You Won !!");
            await messageDialog.ShowAsync();
        }

        /*  -- Method Header Comment
        Name	:	ViewLeaderboard
        Purpose :	Displays the Leaderboard on a new window
        Inputs	:	None
        Outputs	:	The Leaderboard in a new window
        Returns	:	None
        */
        private static async void ViewLeaderboard()
        {
            var leaderboardWindow = await AppWindow.TryCreateAsync();
            var appWindowContentFrame = new Frame();
            appWindowContentFrame.Navigate(typeof(Leaderboard));
            ElementCompositionPreview.SetAppWindowContent(leaderboardWindow, appWindowContentFrame);

            await leaderboardWindow.TryShowAsync();

            leaderboardWindow.Closed += delegate 
            { 
                appWindowContentFrame.Content = null;
                leaderboardWindow = null;
            };
        }

        /*  -- Method Header Comment
        Name	:	NameBox_OnTextChanged
        Purpose :	Once the user enters some text, the Start Button will become active
        Inputs	:	bject sender, TextChangedEventArgs e
        Outputs	:	None
        Returns	:	None
        */
        private void NameBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            StartButton.IsEnabled = true;
        }
    }
}
