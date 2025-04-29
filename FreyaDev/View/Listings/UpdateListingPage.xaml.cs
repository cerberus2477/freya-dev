namespace FreyaDev.View.Listings;

public partial class UpdateListingPage : ContentPage
{
	public UpdateListingPage(UpdateListingViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

}