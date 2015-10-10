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
        private IEnumerable<SteamGame> _games; 

        public MainWindow()
        {
            InitializeComponent();
            _wishlistRetriever = new SteamWishlistRetriever();
            _gamesRetriever = new SteamOwnedGamesRetriever();
        }

        private async void txtMyProfile_LostFocus(object sender, RoutedEventArgs e)
        {
            _wishlist = (await _wishlistRetriever.GetWishlist(txtMyProfile.Text)).ToList();
            foreach (SteamGame game in _wishlist)
            {
                Console.WriteLine(game.ToString());
            }
        }

        private async void txtTheirProfile1_LostFocus(object sender, RoutedEventArgs e)
        {
            _games = (await _gamesRetriever.GetOwnedGames(txtTheirProfile1.Text)).ToList();
            foreach(SteamGame game in _games)
            {
                Console.WriteLine(game.ToString());
            }
        }
    }
}
