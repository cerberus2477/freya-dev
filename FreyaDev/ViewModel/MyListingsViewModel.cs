using FreyaDev.Services;

namespace FreyaDev.ViewModel;

public partial class MyListingsViewModel : BaseViewModel
{
    public ObservableRangeCollection<Listing> MyListings { get; set; } = new ObservableRangeCollection<Listing>();
    private readonly ListingService listingService;
    private readonly ExceptionHandlerUtil exceptionHandlerUtil;
    private readonly UserSessionService userSessionService;

    [ObservableProperty]
    bool isRefreshing;
    //TODO: isbusy kell??

    [ObservableProperty]
    private string searchQuery = string.Empty;

    string username;

    //when the searchtext changes, send a get request to the api to get the matching listings. load the first page into the UI.
    partial void OnSearchQueryChanged(string value)
    {
        SearchMyListingsCommand.Execute(null);
    }

    public MyListingsViewModel(ListingService listingService, ExceptionHandlerUtil exceptionHandlerUtil, UserSessionService userSessionService)
    {
        Title = "Saját hirdetések";
        this.listingService = listingService;
        this.exceptionHandlerUtil = exceptionHandlerUtil;
        this.userSessionService = userSessionService;
        username = userSessionService.GetCurrentUsername();
        //load the listings automatically when navigated to the page
        Task.Run(SearchMyListingsAsync);
    }

    //This code checks to see if the selected item is non-null
    //and then uses the built in Shell Navigation API to push a new page
    //with the listing as a parameter and then deselects the item.

    [RelayCommand]
    async Task GoToUpdateListing(Listing listing)
    {
        if (listing == null)
            return;

        await Shell.Current.GoToAsync("UpdateListingPage", true, new Dictionary<string, object>
        {
            {"Listing", listing }
        });
    }


    [RelayCommand]
    async Task SearchMyListingsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            var listings = await listingService.SearchListings(SearchQuery, username);

            MyListings.Clear();

            MyListings.AddRange(listings);
            Debug.WriteLine($"📄 Loaded mylistings.");
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hiba adódott a saját hirdetések lekérése során.");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

}