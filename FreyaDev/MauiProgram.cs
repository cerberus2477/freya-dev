using Microsoft.Extensions.Logging;

namespace FreyaDev;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        //view model is Transient so a new view model is created each time the page is navigated to

        //ViewModels
        //Listings
        builder.Services.AddTransient<ListingsViewModel>(); //TODO: can this be a singleton?
        builder.Services.AddTransient<ListingDetailsViewModel>();
        builder.Services.AddTransient<MyListingsViewModel>();
        builder.Services.AddTransient<UpdateListingViewModel>();


        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<AuthViewModel>();
        builder.Services.AddTransient<UserViewModel>();

        //Services
        builder.Services.AddSingleton<ListingService>();
        builder.Services.AddSingleton<ProfileService>();
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<UserSessionService>();
        builder.Services.AddSingleton<StageService>();
        builder.Services.AddSingleton<PlantService>();
        builder.Services.AddSingleton<UserService>();


        //Utils
        builder.Services.AddSingleton<ExceptionHandlerUtil>();


        return builder.Build();
    }
}
