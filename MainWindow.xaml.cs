using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Security.Principal;

namespace OFGB
{
    /// <summary>
    /// MainWindow.xaml için etkileşim mantığı
    /// </summary>

    public partial class MainWindow : Window
    {
        [LibraryImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        internal static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, [In] int[] attrValue, int attrSize);

        const string cur_ver = "Software\\Microsoft\\Windows\\CurrentVersion\\";

        public MainWindow()
        {
            InitializeComponent();
            InitializeKeys();

            DwmSetWindowAttribute(new WindowInteropHelper(Application.Current.MainWindow).EnsureHandle(), 33, [2], sizeof(int));
        }

        private void InitializeKeys()
        {
            // Dosya Gezgini'nde sağlayıcı bildirimlerini senkronize et
            bool key1 = CreateKey(cur_ver + "Explorer\\Advanced", "ShowSyncProviderNotifications");
            cb1.IsChecked = key1;

            // Kilit ekranında eğlenceli gerçekler, ipuçları, püf noktaları ve daha fazlasını al
            bool key2 = CreateKey(cur_ver + "ContentDeliveryManager", "RotatingLockScreenOverlayEnabled");
            bool key3 = CreateKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-338387Enabled");
            cb2.IsChecked = key2 && key3;

            // Ayarlar uygulamasında önerilen içerikleri göster
            bool key4 = CreateKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-338393Enabled");
            bool key5 = CreateKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-353694Enabled");
            bool key6 = CreateKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-353696Enabled");
            cb3.IsChecked = key4 && key5 && key6;

            // Windows kullanırken ipuçları ve öneriler al
            bool key7 = CreateKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-338389Enabled");
            cb4.IsChecked = key7;

            // Windows'un en iyi şekilde kullanılması ve bu cihazı kurmanın bitirilmesi için öneriler sun
            bool key8 = CreateKey(cur_ver + "UserProfileEngagement", "ScoobeSystemSettingEnabled");
            cb5.IsChecked = key8;

            // Güncellemelerden sonra ve bazen giriş yaparken yeni ve önerilen özellikleri vurgulamak için Windows karşılama deneyimini göster
            bool key9 = CreateKey(cur_ver + "ContentDeliveryManager", "SubscribedContent-310093Enabled");
            cb6.IsChecked = key9;

            // Uygulamaların kişiselleştirilmiş reklamlar göstermesine izin vermek için reklam kimliğimi kullan
            bool key10 = CreateKey(cur_ver + "AdvertisingInfo", "Enabled");
            cb7.IsChecked = key10;

            // Uyarlanmış deneyimler
            bool key11 = CreateKey(cur_ver + "Privacy", "TailoredExperiencesWithDiagnosticDataEnabled");
            cb8.IsChecked = key11;

            // Başlangıçta "Başlat" menüsünde ipuçları, kısayollar, yeni uygulamalar ve daha fazlasını göster
            bool key12 = CreateKey(cur_ver + "Explorer\\Advanced", "Start_IrisRecommendations");
            cb9.IsChecked = key12;

            // "<Uygulama> bildirimlerini kapatmak istiyor musunuz? Bunları bir süredir açmadığınızı fark ettik."
            bool key13 = CreateKey(cur_ver + "Notifications\\Settings\\Windows.ActionCenter.SmartOptOut", "Enabled");
            cb10.IsChecked = key13;

            // Bu, Yönetici olarak çalıştırılması gerekenler
            if (IsRunningAsAdministrator())
            {
                // Windows Aramasında Bing Sonuçlarını Göster (Ters, 1 == Devre Dışı)
                bool key14 = CreateKey("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions");
                bool key15 = CreateKey(cur_ver + "Search", "BingSearchEnabled");
                cb11.IsChecked = !key14 && key15;

                // Edge masaüstü arama widget çubuğunu devre dışı bırak
                bool key16 = CreateKey("Software\\Policies\\Microsoft\\Edge", "WebWidgetAllowed");
                cb12.IsChecked = key16;
            }
            else
            {
                cb11.IsEnabled = false;
                cb12.IsEnabled = false;
            }
        }

        private static bool CreateKey(string loc, string key)
        {
            RegistryKey? keyRef;
            int value;

            if (Registry.CurrentUser.OpenSubKey(loc, true) is not null)
            {
                keyRef = Registry.CurrentUser.OpenSubKey(loc, true);
            }
            else
            {
                keyRef = Registry.CurrentUser.CreateSubKey(loc);
                keyRef.SetValue(key, 0);
            }

            if (keyRef is null)
            {
                MessageBox.Show("Başlatma sırasında bir kayıt defteri alt anahtarı oluşturma başarısız oldu!", "OFGB: Ölümcül Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new InvalidOperationException("OFGB: Başlatma sırasında alt anahtar oluşturma başarısız oldu!");
            }

            value = Convert.ToInt32(keyRef.GetValue(key));
            keyRef.Close();

            return !(value != 0);
        }

        private static void ToggleOptions(string checkboxName, bool enable)
        {
            switch (checkboxName)
            {
                case "cb1":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Explorer\\Advanced\\", "ShowSyncProviderNotifications", Convert.ToInt32(!enable));
                    break;
                case "cb2":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "RotatingLockScreenOverlayEnabled", Convert.ToInt32(!enable));
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-338387Enabled", Convert.ToInt32(!enable));
                    break;
                case "cb3":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-338393Enabled", Convert.ToInt32(!enable));
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-353694Enabled", Convert.ToInt32(!enable));
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-353696Enabled", Convert.ToInt32(!enable));
                    break;
                case "cb4":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-338389Enabled", Convert.ToInt32(!enable));
                    break;
                case "cb5":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "UserProfileEngagement", "ScoobeSystemSettingEnabled", Convert.ToInt32(!enable));
                    break;
                case "cb6":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "ContentDeliveryManager", "SubscribedContent-310093Enabled", Convert.ToInt32(!enable));
                    break;
                case "cb7":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "AdvertisingInfo", "Enabled", Convert.ToInt32(!enable));
                    break;
                case "cb8":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", Convert.ToInt32(!enable));
                    break;
                case "cb9":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Explorer\\Advanced", "Start_IrisRecommendations", Convert.ToInt32(!enable));
                    break;
                case "cb10":
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Notifications\\Settings\\Windows.ActionCenter.SmartOptOut", "Enabled", Convert.ToInt32(!enable));
                    break;
                case "cb11":
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions", Convert.ToInt32(enable)); // <- Inverted
                    Registry.SetValue("HKEY_CURRENT_USER\\" + cur_ver + "Search", "BingSearchEnabled", Convert.ToInt32(!enable));
                    break;
                case "cb12":
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Policies\\Microsoft\\Edge", "WebWidgetAllowed", Convert.ToInt32(!enable));
                    break;
            }
        }

        public static bool IsRunningAsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void Checked(object sender, RoutedEventArgs e)
        {
            ToggleOptions(((CheckBox)sender).Name, true);
        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleOptions(((CheckBox)sender).Name, false);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
