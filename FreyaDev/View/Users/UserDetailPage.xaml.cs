namespace FreyaDev.View.Users;

public partial class UserDetailPage : ContentPage
{
    public UserDetailPage(UserViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}