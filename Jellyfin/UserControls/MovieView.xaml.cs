using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Jellyfin.Models;
using Jellyfin.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Jellyfin.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MovieView
    {
        public MovieView()
        {
            this.InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //.Navigate(typeof(MovieDetailView)
        }

        private void MovieGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            (DataContext as MovieViewModel).NavigateToMovie();
        }
    }
}
