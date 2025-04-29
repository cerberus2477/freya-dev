using FreyaDev.Services;
using System.Threading;

namespace FreyaDev.ViewModel;

public partial class ListingsViewModel : BaseViewModel
{
    List<Listing> allListings = new();
    int PageSize = 4;

    public ObservableRangeCollection<Listing> Listings { get; set; } = new ObservableRangeCollection<Listing>();
    private readonly ListingService listingService;
    private readonly ExceptionHandlerUtil exceptionHandlerUtil;

    [ObservableProperty]
    bool isRefreshing;
    //TODO: isbusy kell??

    [ObservableProperty]
    private string searchQuery = string.Empty;

    //when the searchtext changes, send a get request to the api to get the matching listings. load the first page into the UI.
    partial void OnSearchQueryChanged(string value)
    {
        //runs the relaycommand, respects UI command logic, disables button if busy, etc
        SearchListingsCommand.Execute(null);
    }

    public ListingsViewModel(ListingService listingService, ExceptionHandlerUtil exceptionHandlerUtil)
    {
        Title = "Hirdetések";
        this.listingService = listingService;
        this.exceptionHandlerUtil = exceptionHandlerUtil;
        //load the listings automatically when navigated to the page
        //Directly runs the SearchListingsAsync() method in a background thread.
        Task.Run(SearchListingsAsync);
    }

    //This code checks to see if the selected item is non-null
    //and then uses the built in Shell Navigation API to push a new page
    //with the listing as a parameter and then deselects the item.

    [RelayCommand]
    async Task GoToListingDetails(Listing listing)
    {
        if (listing == null)
            return;

        await Shell.Current.GoToAsync("ListingDetailsPage", true, new Dictionary<string, object>
        {
            {"Listing", listing }
        });
    }


    [RelayCommand]
    async Task SearchListingsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            allListings = await listingService.SearchListings(SearchQuery);

            if (allListings == null) return;

            Listings.Clear();

            // Add first page manually
            Listings.AddRange(allListings.Take(PageSize).ToList());
            Debug.WriteLine($"📄 Loaded first page into Listings.");
        }
        catch (Exception ex)
        {
            await exceptionHandlerUtil.HandleExceptionAsync(ex, "Hiba adódott a hirdetések lekérése során.");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    //TODO: implement filters


    [RelayCommand]
    async Task GetNextListingsAsync()
    {
        if (IsBusy) return;
      
        IsBusy = true;
        int remaining = allListings.Count - Listings.Count;

        //if first page has been loaded and there are more listings, load next page.
        if (remaining > 0 && Listings.Count > 0)
        {
            var nextPage = allListings.Skip(Listings.Count).Take(PageSize).ToList();
            Listings.AddRange(nextPage);
            Debug.WriteLine($"✅ Loaded next page of listings ({nextPage.Count} items). Total now: {Listings.Count} Remaining: {remaining}");
        }
        else
        {
            Debug.WriteLine("✅ All listings already loaded.");
        }
 
        IsBusy = false;
        IsRefreshing = false;

    }

}