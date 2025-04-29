namespace FreyaDev.View.Listings;

public partial class ListingsPage : ContentPage
{
    public ListingsPage(ListingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnFilterButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.DisplayAlert("Sz�r�k", "Itt lesznek a sz�r�k!", "OK");
    }
}