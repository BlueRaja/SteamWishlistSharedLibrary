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
    /// Interaction logic for GamesGrid.xaml
    /// </summary>
    public partial class GamesGrid : UserControl
    {
        public GamesGrid()
        {
            InitializeComponent();
        }

        public IEnumerable<SteamGame> GamesList { get; set; } 
    }
}
