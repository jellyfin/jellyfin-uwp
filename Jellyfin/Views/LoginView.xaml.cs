using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Jellyfin.ViewModels;

namespace Jellyfin.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView
    {
        public LoginView()
        {
            InitializeComponent();
            Loaded += OnBoarding_Loaded;
        }

        private async void FocusNextItem(object sender, KeyRoutedEventArgs e)
        {
            List<Control> visualItems = new List<Control> { txtUrl, txtName, password, btnConnect };
            Control visualItem = (sender as Control);

            if (e.Key == VirtualKey.Enter)
            {
                int selectedItemIndex = visualItems.IndexOf(visualItem);
                if (selectedItemIndex > -1 && selectedItemIndex < visualItems.Count)
                {
                    Control newItem = visualItems[selectedItemIndex+1];
                    newItem.Focus(FocusState.Programmatic);
                } else if (selectedItemIndex == visualItems.Count)
                {
                    //(DataContext as LoginViewModel).LoginAsync();
                }
            }
        }
        
        private void OnBoarding_Loaded(object sender, RoutedEventArgs e)
        {
            txtUrl.Focus(FocusState.Programmatic);
        }
    }
}
