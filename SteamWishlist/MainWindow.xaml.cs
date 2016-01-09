using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamWishlist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: Handle exceptions thrown by WebBrowser
        //TODO: Save values entered
        private SteamGamesList _wishlist;

        private readonly IList<SteamGamesList> _sharedGames;
        private readonly List<Task> _awaitingTasks;
        private readonly SteamWishlistRetriever _wishlistRetriever;
        private readonly SteamOwnedGamesRetriever _gamesRetriever;
        private readonly IEnumerable<TextBox> _theirProfileTextboxes;
        private readonly SteamProfileUrlValidator _profileUrlValidator;

        private const string DefaultTextMySteamProfile = "http://www.steamcommunity.com/id/MYID";
        private const string DefaultTextTheirSteamProfile = "http://www.steamcommunity.com/id/FRIEND";

        public MainWindow()
        {
            InitializeComponent();

            WebClient webClient = new WebClient { Proxy = null }; //See http://stackoverflow.com/questions/4415443 - ugh.
            _wishlistRetriever = new SteamWishlistRetriever(webClient);
            _gamesRetriever = new SteamOwnedGamesRetriever(webClient);
            _profileUrlValidator = new SteamProfileUrlValidator();
            _awaitingTasks = new List<Task>();
            _sharedGames = new List<SteamGamesList>();
            _theirProfileTextboxes = new[] {txtTheirProfile1, txtTheirProfile2, txtTheirProfile3, txtTheirProfile3, txtTheirProfile4, txtTheirProfile5};

            txtMyProfile.Text = DefaultTextMySteamProfile;
            foreach (TextBox txtTheirProfile in _theirProfileTextboxes)
            {
                txtTheirProfile.Text = DefaultTextTheirSteamProfile;
            }
        }

        private void txtMyProfile_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMyProfile.Text == DefaultTextMySteamProfile)
            {
                txtMyProfile.Clear();
            }
        }

        private async void txtMyProfile_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            _wishlist = null;

            if (_profileUrlValidator.IsValidSteamProfileUrl(txtMyProfile.Text))
            {
                lblLoading.Visibility = Visibility.Visible;
                Task<SteamGamesList> task = _wishlistRetriever.GetWishlist(txtMyProfile.Text);
                _wishlist = await KeepTrackOfTasks(task);
            }

            RefreshGrid();
        }

        private void txtTheirProfile_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            foreach(TextBox txtTheirProfile in _theirProfileTextboxes)
            {
                if (txtTheirProfile.Text == DefaultTextTheirSteamProfile)
                {
                    txtTheirProfile.Clear();
                }
            }
        }

        private async void txtTheirProfile_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            if(textBox.Tag != null)
            {
                _sharedGames.Remove((SteamGamesList)textBox.Tag);
                textBox.Tag = null;
            }

            if (_profileUrlValidator.IsValidSteamProfileUrl(textBox.Text))
            {
                lblLoading.Visibility = Visibility.Visible;

                Task<SteamGamesList> task = _gamesRetriever.GetOwnedGames(textBox.Text);
                var games = await KeepTrackOfTasks(task);

                textBox.Tag = games;
                _sharedGames.Add(games);
            }

            RefreshGrid();
        }

        private async Task<T> KeepTrackOfTasks<T>(Task<T> task)
        {
            _awaitingTasks.Add(task);
            var returnVal = await task;
            _awaitingTasks.Remove(task);

            return returnVal;
        }

        private void RefreshGrid()
        {
            if (_awaitingTasks.Any())
            {
                return;
            }

            lblLoading.Visibility = Visibility.Hidden;
            if (_wishlist == null || !_wishlist.Any() || !_sharedGames.Any())
            {
                gamesGrid.Clear();
                return;
            }

            var allOwnedGames = _sharedGames.SelectMany(o => o).Distinct();
            gamesGrid.GamesList = _wishlist.Intersect(allOwnedGames);
            lblSharedGames.Content = $"Shared games - {gamesGrid.GamesList.Count()} found";
        }
    }
}
