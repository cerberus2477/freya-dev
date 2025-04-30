//namespace FreyaDev.View.Users;

//public partial class UserUpdatePage : ContentPage
//{
//    private UserSessionService userSessionService;
//    private UserViewModel viewModel;
//    public UserUpdatePage(UserViewModel viewModel, UserSessionService userSessionService)
//    {
//        InitializeComponent();
//        this.viewModel = viewModel;
//        BindingContext = viewModel;
//        this.userSessionService = userSessionService;
//    }

//    private void EditUser_Clicked(object sender, EventArgs e)
//    {
//        // Enable editing
//        UsernameEntry.IsEnabled = true;
//        EmailEntry.IsEnabled = true;
//        CityEntry.IsEnabled = true;
//        BirthdateEntry.IsEnabled = true;
//        DescriptionEntry.IsEnabled = true;

//        // Show Save button, hide Edit button
//        EditButton.IsVisible = false;
//        SaveButton.IsVisible = true;
//    }

//    private async void SaveUser_Clicked(object sender, EventArgs e)
//    {
//        // Disable editing
//        UsernameEntry.IsEnabled = false;
//        EmailEntry.IsEnabled = false;
//        CityEntry.IsEnabled = false;
//        BirthdateEntry.IsEnabled = false;
//        DescriptionEntry.IsEnabled = false;



//        // Show Edit button, hide Save button
//        EditButton.IsVisible = true;
//        SaveButton.IsVisible = false;
//    }
//}

using Microsoft.Maui.Controls;
using FreyaDev.Model;
using FreyaDev.ViewModel;
using FreyaDev.Services;

namespace FreyaDev.View.Users
{
    public partial class UserUpdatePage : ContentPage
    {
        private readonly UserSessionService _userSessionService;
        private readonly UserViewModel _viewModel;

        public UserUpdatePage(UserViewModel viewModel, UserSessionService userSessionService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            _userSessionService = userSessionService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData();
        }

        //private void LoadUserData()
        //{
        //    // Get the current route
        //    var route = Shell.Current.CurrentState.Location.OriginalString;

        //    // Check if we have query parameters
        //    if (route.Contains('?'))
        //    {
        //        var query = route.Split('?')[1];
        //        var parameters = ParseQueryString(query);

        //        if (parameters.TryGetValue("User", out var userObj) &&
        //            userObj is User user)
        //        {
        //            _viewModel.UserUsername = user.Username;
        //            _viewModel.UserEmail = user.Email;
        //            _viewModel.UserCity = user.City;
        //            _viewModel.UserBirthdate = user.Birthdate;
        //            _viewModel.UserDescription = user.Description;
        //            _viewModel.OriginalUser = user;
        //        }
        //    }
        //}
        // Replace LoadUserData with this version
        private void LoadUserData()
        {
            if (Shell.Current.CurrentState.Location.OriginalString is string route &&
                route.Contains('?'))
            {
                var query = route.Split('?')[1];
                var parameters = System.Web.HttpUtility.ParseQueryString(query);

                if (int.TryParse(parameters["userId"], out int userId))
                {
                    _viewModel.LoadUserDetailsCommand.Execute(userId);
                }
            }
        }

        private void EditUser_Clicked(object sender, EventArgs e)
        {
            UsernameEntry.IsEnabled = true;
            EmailEntry.IsEnabled = true;
            CityEntry.IsEnabled = true;
            BirthdateEntry.IsEnabled = true;
            DescriptionEntry.IsEnabled = true;

            EditButton.IsVisible = false;
            SaveButton.IsVisible = true;
        }

        private async void SaveUser_Clicked(object sender, EventArgs e)
        {
            if (_viewModel.OriginalUser != null)
            {
                await _viewModel.SaveUserCommand.ExecuteAsync(_viewModel.OriginalUser);
            }

            UsernameEntry.IsEnabled = false;
            EmailEntry.IsEnabled = false;
            CityEntry.IsEnabled = false;
            BirthdateEntry.IsEnabled = false;
            DescriptionEntry.IsEnabled = false;

            EditButton.IsVisible = true;
            SaveButton.IsVisible = false;
        }
    }
}