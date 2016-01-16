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
        private const string DefaultTextMySteamProfile = "ex: http://www.steamcommunity.com/id/MYID";
        private const string DefaultTextTheirSteamProfile = "ex: http://www.steamcommunity.com/id/FRIEND";

        private SteamGamesList _wishlist;
        private IList<SteamGamesList> _sharedGames;
        private bool _isLoading;
        private bool _isInitializing;

        private readonly SteamWishlistRetriever _wishlistRetriever;
        private readonly SteamOwnedGamesRetriever _gamesRetriever;
        private readonly IList<TextBox> _theirProfileTextboxes;
        private readonly SteamProfileUrlValidator _profileUrlValidator;

        public MainWindow()
        {
            _isInitializing = true;
            InitializeComponent();

            _wishlistRetriever = new SteamWishlistRetriever();
            _gamesRetriever = new SteamOwnedGamesRetriever();
            _profileUrlValidator = new SteamProfileUrlValidator();
            _sharedGames = new List<SteamGamesList>();
            _theirProfileTextboxes = new[] {txtTheirProfile1, txtTheirProfile2, txtTheirProfile3, txtTheirProfile3, txtTheirProfile4, txtTheirProfile5};

            InitializeTextbox(txtMyProfile, UrlSaver.MySteamProfile, DefaultTextMySteamProfile);
            for (int i = 0; i < _theirProfileTextboxes.Count; i++)
            {
                string savedValue = UrlSaver.TheirSteamProfiles.Skip(i).FirstOrDefault();
                InitializeTextbox(_theirProfileTextboxes[i], savedValue, DefaultTextTheirSteamProfile);
            }
            _isInitializing = false;

            RefreshGamesLists();
        }

        private void InitializeTextbox(TextBox textbox, string value, string defaultValue)
        {
            textbox.Text = String.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        private void txtMyProfile_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtMyProfile.Text == DefaultTextMySteamProfile)
            {
                txtMyProfile.Clear();
            }
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

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitializing)
            {
                UrlSaver.MySteamProfile = txtMyProfile.Text;
                UrlSaver.TheirSteamProfiles = _theirProfileTextboxes.Select(o => o.Text);
            }

            SetLoadButtonEnabled();
        }

        private bool IsReadyToLoad()
        {
            return _profileUrlValidator.IsValidSteamProfileUrl(txtMyProfile.Text) &&
                   _theirProfileTextboxes.Any(o => _profileUrlValidator.IsValidSteamProfileUrl(o.Text));
        }

        private void SetLoadButtonEnabled()
        {
            lblLoading.Visibility = (_isLoading ? Visibility.Visible : Visibility.Hidden);
            btnLoad.IsEnabled = IsReadyToLoad() && !_isLoading;
        }

        private async Task RefreshGamesLists()
        {
            if (!IsReadyToLoad())
            {
                return;
            }

            _isLoading = true;
            SetLoadButtonEnabled();

            Task<SteamGamesList> wishlistTask = _wishlistRetriever.GetWishlist(txtMyProfile.Text);
            IEnumerable<Task<SteamGamesList>> sharedGamesTasks = _theirProfileTextboxes.Select(o => o.Text)
                .Where(_profileUrlValidator.IsValidSteamProfileUrl)
                .Select(_gamesRetriever.GetOwnedGames);

            _wishlist = await wishlistTask;
            _sharedGames = await Task.WhenAll(sharedGamesTasks);
            RefreshGrid();

            _isLoading = false;
            SetLoadButtonEnabled();
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if(!IsReadyToLoad())
            {
                throw new Exception("This shouldn't happen, btnLoad_Click() should never be called when IsReadyToLoad() is false!");
            }

            await RefreshGamesLists();
        }

        private void RefreshGrid()
        {
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
