using Microsoft.Maui.Controls;
using FreyaDev.View;
using FreyaDev.View.StartingPages;

namespace FreyaDev.View
{
    public partial class ProfilePage : ContentPage
    {
        private UserSessionService userSessionService;
        public ProfilePage(ProfileViewModel viewModel, UserSessionService userSessionService)
        {
            InitializeComponent();
            BindingContext = viewModel;
            this.userSessionService = userSessionService;
        }

        //todo: ezek jó helyen vannak itt vagy menjenek a viewmodelbe?
        private void EditProfile_Clicked(object sender, EventArgs e)
        {
            // Enable editing
            UsernameEntry.IsEnabled = true;
            EmailEntry.IsEnabled = true;
            CityEntry.IsEnabled = true;
            BirthdateEntry.IsEnabled = true;
            DescriptionEntry.IsEnabled = true;

            // Show Save button, hide Edit button
            EditButton.IsVisible = false;
            SaveButton.IsVisible = true;
        }

        private void SaveProfile_Clicked(object sender, EventArgs e)
        {
            // Disable editing
            UsernameEntry.IsEnabled = false;
            EmailEntry.IsEnabled = false;
            CityEntry.IsEnabled = false;
            BirthdateEntry.IsEnabled = false;
            DescriptionEntry.IsEnabled = false;

            

            // Show Edit button, hide Save button
            EditButton.IsVisible = true;
            SaveButton.IsVisible = false;
        }

        //todo: use usersessionservice to log out.
        //should this be here or in the viewmodel?
        private async void Logout_Clicked(object sender, EventArgs e)
        {
            try
            {

                userSessionService.Logout();
                // (TODO: make sure to (clear the navigation stack), hide the nav)
                // Navigate to the login page
                await Shell.Current.GoToAsync("/LoginPage");
            }
            catch (Exception ex)
            {
                // Log or handle errors
                Debug.WriteLine($"Logout failed: {ex.Message}");
                throw; // Or handle gracefully
            }



        }


        private async void MyListings_Clicked(object sender, EventArgs e)
        {
            // Navigate to MyListingsPage
            await Shell.Current.GoToAsync("MyListingsPage");
        }
    }
}