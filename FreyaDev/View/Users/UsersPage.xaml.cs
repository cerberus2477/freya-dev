namespace FreyaDev.View.Users;

public partial class UsersPage : ContentPage
{
    public UsersPage(UserViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}