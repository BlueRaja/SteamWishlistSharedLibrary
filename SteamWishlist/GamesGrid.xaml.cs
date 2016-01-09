using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for GamesGrid.xaml
    /// </summary>
    public partial class GamesGrid : UserControl
    {
        private IEnumerable<SteamGame> _gamesList;

        public IEnumerable<SteamGame> GamesList
        {
            get { return _gamesList; }
            set
            {
                _gamesList = value?.OrderBy(o => o.Name).ToList();
                dataGrid.ItemsSource = _gamesList;
            }
        }

        public GamesGrid()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            GamesList = null;
        }

        private void DG_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }
    }
}
