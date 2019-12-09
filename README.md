# Jellyfin UWP Client
This is a wrapper around Jellyfin's web interface (see https://github.com/jellyfin/jellyfin-web) for UWP devices (Windows 10, Windows Phone 10, Xbox One, Windows IOT, etc.). It may or may not become a full native client in the future.

# Installing from .appxbundle
1. Ensure your computer is in **Developer Mode** (Settings > Update & Security > For Developers; select Developer Mode (Sideload Apps might work too)
2. Download both the .appxbundle and .cer files from the latest [release](https://github.com/chaosinnovator/jellyfin-uwp/releases)
3. Right-click the downloaded .cer file and choose **Install Certificate**. Follow prompts to install the certificate on the **Local Machine** in the **Trusted Root Certification Authorities** Certificate store: Local Machine > Next > Place all certificates in the following store > Browse... > Trusted Root Certification Authorities > Ok > next > Finish
4. Open the .appxbundle file and click **Install**

# Building
1. Clone the repository
2. Open the .sln file with Visual Studio
3. Build and deploy the solution (F5)

# Usage
1. Fill in the URL (i.e. http://192.168.1.xxx:8096 or https://jellyfin.url.com) and press Done
