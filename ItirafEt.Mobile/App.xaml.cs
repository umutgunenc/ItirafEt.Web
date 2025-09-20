using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Application = Microsoft.Maui.Controls.Application;


namespace ItirafEt.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
#if ANDROID
            // Android tarafında klavye açılınca sayfa yeniden boyutlansın
            Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
                   .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
#endif
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "ItirafEt.Mobile" };
        }
    }
}
