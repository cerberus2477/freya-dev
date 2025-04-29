using System.Xml.Linq;

namespace FreyaDev.View.Listings;
//[QueryProperty(nameof(Id), "Id")]
public partial class ListingDetailsPage : ContentPage
{
	public ListingDetailsPage(ListingDetailsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}