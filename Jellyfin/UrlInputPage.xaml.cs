using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Jellyfin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UrlInputPage : Page
    {
        private string _uriString;

        public UrlInputPage()
        {
            this.InitializeComponent();
            this.Loaded += UrlInputPage_Loaded;
        }

        private void UrlInputPage_Loaded(object sender, RoutedEventArgs e)
        {
            UrlInputBox.Focus(FocusState.Programmatic);
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            BtnConnect.IsEnabled = false;
            TxtError.Visibility = Visibility.Collapsed;
            BtnConfirm.Visibility = Visibility.Collapsed;

            _uriString = UrlInputBox.Text.Trim(); // Trim to remove any leading/trailing whitespaces

            bool isValid = await NormalizeAndCheckUri(_uriString);
            BtnConnect.IsEnabled = true; // Re-enable the button after the check is complete

            if (!isValid)
            {
                BtnConfirm.Visibility = Visibility.Visible;
                // The error message should already be set within NormalizeAndCheckUri method
            }
            else
            {
                ProceedWithValidURL(_uriString);
            }
        }

        private async Task<bool> NormalizeAndCheckUri(string uriInput)
        {
            UriBuilder uriBuilder;

            // Attempt to normalize URL with https first.
            try
            {
                uriBuilder = new UriBuilder(uriInput)
                {
                    Scheme = Uri.UriSchemeHttps
                };
                if (uriBuilder.Port.Equals(80))
                {
                    uriBuilder.Port = -1; // -1 indicates that the default port for the scheme should be used.

                }
            }
            catch (Exception ex)
            {
                UpdateErrorUI($"Invalid URL format: {ex.Message}");
                return false;
            }

            // Try connecting with https first.
            _uriString = uriBuilder.Uri.AbsoluteUri;
            if (await CheckURLValidAsync(_uriString))
            {
                return true;
            }

            // If https fails, try with http.
            uriBuilder.Scheme = Uri.UriSchemeHttp;
            _uriString = uriBuilder.Uri.AbsoluteUri;
            if (await CheckURLValidAsync(_uriString))
            {
                return true;
            }
            UpdateErrorUI("Unable to connect to the server with both https and http.");
            return false;
        }



        private void UrlInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SubmitButton_Click(this, new RoutedEventArgs());
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming the URL is correct based on user confirmation;
            ProceedWithValidURL(_uriString);
        }

        private async Task<bool> CheckURLValidAsync(string uriString)
        {
            // Ensure the URL is well-formed
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out Uri testUri))
            {
                UpdateErrorUI("Invalid URL format.");
                return false;
            }

            // First attempt to connect with the URI provided
            HttpWebResponse response = await AttemptConnection(testUri);
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                // We have a successful https connection, no need to try http.
                bool isValidContent = await ValidateResponseContent(response);
                response.Dispose(); // Always dispose of the response object when done.
                return isValidContent;
            }

            // If we reach here, both https and http have failed
            response?.Dispose(); // Dispose if not null
            return false;
        }


        private async Task<HttpWebResponse> AttemptConnection(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = 10000; // Set a timeout period (10 seconds, for example).
            try
            {
                return (HttpWebResponse)(await request.GetResponseAsync());
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    return response; // Return the response for further status code checks
                }
                // Handle specific WebException scenarios or log for debugging
                // For example: ex.Status == WebExceptionStatus.NameResolutionFailure means DNS resolution failed
                return null; // Connection failed for a reason other than an HTTP response
            }
            catch (SocketException ex)
            {
                // Handle network layer errors.
                // Log the error message: ex.Message
                return null;
            }
            catch (TimeoutException ex)
            {
                // Handle timeout.
                // Log the error message: ex.Message
                return null;
            }
            catch (Exception ex)
            {
                // Handle other types of exceptions.
                // Log the error message: ex.Message
                return null;
            }
        }


        private async Task<bool> ValidateResponseContent(HttpWebResponse response)
        {
            var encoding = System.Text.Encoding.GetEncoding(response.CharacterSet);
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = await reader.ReadToEndAsync();
                if (!responseText.Contains("Jellyfin"))
                {
                    // Update UI with a specific error message
                    UpdateErrorUI("Response does not contain expected content.");
                    return false;
                }
            }

            return true;
        }

        private void UpdateErrorUI(string message)
        {
            TxtError.Text = message;
            TxtError.Visibility = Visibility.Visible;
        }

        private void ProceedWithValidURL(string url)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["savedUrl"] = url; // Save the URL
            Frame.Navigate(typeof(Jellyfin.MainPage), url); // Navigate to MainPage with the new URL
        }

    }
}
