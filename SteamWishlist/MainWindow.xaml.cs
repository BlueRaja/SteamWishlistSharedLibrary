using System;
using System.Collections.Generic;
using System.Linq;
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

        private IEnumerable<SteamGame> _wishlist;
        private IList<IEnumerable<SteamGame>> _games;

        private List<Task> _awaitingTasks;

        public MainWindow()
        {
            InitializeComponent();
            _wishlistRetriever = new SteamWishlistRetriever();
            _gamesRetriever = new SteamOwnedGamesRetriever();
            _awaitingTasks = new List<Task>();
            _games = new List<IEnumerable<SteamGame>>();
        }

        private async void txtMyProfile_LostFocus(object sender, RoutedEventArgs e)
        {
            Task<IEnumerable<SteamGame>> task = _wishlistRetriever.GetWishlist(txtMyProfile.Text);
            lblLoading.Visibility = Visibility.Visible;
            _awaitingTasks.Add(task);
            _wishlist = (await task).ToList();
            _awaitingTasks.Remove(task);

            RefreshGrid();
        }

        private async void txtTheirProfile1_LostFocus(object sender, RoutedEventArgs e)
        {
            //TODO: Remove games list when a URL is overwritten
            //TODO: Generalize for other textboxes
            Task<IEnumerable<SteamGame>> task = _gamesRetriever.GetOwnedGames(txtTheirProfile1.Text);
            lblLoading.Visibility = Visibility.Visible;
            _awaitingTasks.Add(task);
            var games = (await task).ToList();
            _awaitingTasks.Remove(task);

            _games.Add(games);

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            if (_awaitingTasks.Any())
            {
                return;
            }

            lblLoading.Visibility = Visibility.Hidden;
            if (!_wishlist.Any() || !_games.Any())
            {
                return;
            }

            var allOwnedGames = _games.SelectMany(o => o).Distinct();
            gamesGrid.GamesList = _wishlist.Intersect(allOwnedGames); ;
        }
    }
}
