using FreyaDev.View;
using FreyaDev.View.StartingPages;
using FreyaDev.View.Listings;

namespace FreyaDev;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ListingsPage), typeof(ListingsPage));
        Routing.RegisterRoute(nameof(ListingDetailsPage), typeof(ListingDetailsPage));
        Routing.RegisterRoute(nameof(MyListingsPage), typeof(MyListingsPage));
        Routing.RegisterRoute(nameof(UpdateListingPage), typeof(UpdateListingPage));

        Routing.RegisterRoute(nameof(NewListingPage), typeof(NewListingPage));

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(GreetingPage), typeof(GreetingPage));

        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));

        DetermineStartPage();
        //AdjustNavigationBar();
    }

    private void DetermineStartPage()
    {
        bool isLoggedIn = Preferences.Get("IsLoggedIn", false);
        SetFlyoutBehavior(this, isLoggedIn ? FlyoutBehavior.Locked : FlyoutBehavior.Disabled);
        GoToAsync(isLoggedIn ? "HomePage" : "GreetingPage");
    }

    private void AdjustNavigationBar()
    {
        bool isMobile = DeviceDisplay.MainDisplayInfo.Width < 800;
        Shell.SetFlyoutBehavior(this, isMobile ? FlyoutBehavior.Disabled : FlyoutBehavior.Flyout);
        Shell.SetTabBarIsVisible(this, true);
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);
        bool hideFlyout = args.Current.Location.OriginalString.Contains("GreetingPage") ||
                          args.Current.Location.OriginalString.Contains("LoginPage");

        SetFlyoutBehavior(this, hideFlyout ? FlyoutBehavior.Disabled : FlyoutBehavior.Flyout);
    }
}
