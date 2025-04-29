namespace FreyaDev.View.Listings;

public partial class MyListingsPage : ContentPage
{
    public MyListingsPage(MyListingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}