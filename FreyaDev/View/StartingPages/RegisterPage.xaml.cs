namespace FreyaDev.View.StartingPages
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(AuthViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Title = "Regisztr�ci�"; // Set title for register
            BindingContext = viewModel;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("LoginPage");
        }
    }
}
