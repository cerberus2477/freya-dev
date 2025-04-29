namespace FreyaDev.View.StartingPages
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(AuthViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Title = "Regisztráció"; // Set title for register
            BindingContext = viewModel;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("LoginPage");
        }
    }
}
