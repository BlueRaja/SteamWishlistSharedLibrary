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
        private SteamWishlistRetriever _wishlistRetriever;
        private SteamOwnedGamesRetriever _gamesRetriever;

        private SteamGamesList _wishlist;
        private IList<SteamGamesList> _sharedGames;

        private List<Task> _awaitingTasks;

        public MainWindow()
        {
            InitializeComponent();

            WebClient webClient = new WebClient { Proxy = null }; //See http://stackoverflow.com/questions/4415443 - ugh.
            _wishlistRetriever = new SteamWishlistRetriever(webClient);
            _gamesRetriever = new SteamOwnedGamesRetriever(webClient);
            _awaitingTasks = new List<Task>();
            _sharedGames = new List<SteamGamesList>();
        }

        private async void txtMyProfile_LostFocus(object sender, RoutedEventArgs e)
        {
            Task<SteamGamesList> task = _wishlistRetriever.GetWishlist(txtMyProfile.Text);
            lblLoading.Visibility = Visibility.Visible;
            _wishlist = await KeepTrackOfTasks(task);

            RefreshGrid();
        }

        private async void txtTheirProfile_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox) sender;
            lblLoading.Visibility = Visibility.Visible;

            Task<SteamGamesList> task = _gamesRetriever.GetOwnedGames(textBox.Text);
            var games = await KeepTrackOfTasks(task);

            if (textBox.Tag != null)
            {
                _sharedGames.Remove((SteamGamesList) textBox.Tag);
            }
            textBox.Tag = games;
            _sharedGames.Add(games);

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
            if (!_wishlist.Any() || !_sharedGames.Any())
            {
                return;
            }

            var allOwnedGames = _sharedGames.SelectMany(o => o).Distinct();
            gamesGrid.GamesList = _wishlist.Intersect(allOwnedGames);
            lblSharedGames.Content = $"Shared games - {gamesGrid.GamesList.Count()} found";
        }
    }
}
