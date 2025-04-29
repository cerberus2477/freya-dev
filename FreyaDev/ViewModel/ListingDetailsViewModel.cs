namespace FreyaDev.ViewModel;

[QueryProperty(nameof(Listing), "Listing")]
public partial class ListingDetailsViewModel : BaseViewModel
{
    public ListingDetailsViewModel()
    {
    }

    [ObservableProperty]
    Listing listing;
}