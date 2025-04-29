using Microsoft.Maui.Controls;
using FreyaDev.Services;

namespace FreyaDev.View.StartingPages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(AuthViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Title = "Bejelentkezés"; 
            BindingContext = viewModel;
        }

        private async void ForgotPassword_Tapped(object sender, EventArgs e)
        {
            await DisplayAlert("Forgot Password - not implemened", "Password reset functionality not implemented yet.", "OK");
        }
    }
}