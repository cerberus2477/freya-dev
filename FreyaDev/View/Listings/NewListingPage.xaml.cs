using Microsoft.Maui.Controls;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FreyaDev.View.Listings
{
    public partial class NewListingPage : ContentPage
    {
        private List<string> uploadedImages = new List<string>();

        public NewListingPage()
        {
            InitializeComponent();
        }

        private async void OnUploadPhotosClicked(object sender, EventArgs e)
        {
            try
            {
                var results = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Select Photos",
                    FileTypes = FilePickerFileType.Images
                });

                if (results != null)
                {
                    foreach (var file in results)
                    {
                        var stream = await file.OpenReadAsync();
                        uploadedImages.Add(file.FullPath);

                        var image = new Image
                        {
                            Source = ImageSource.FromStream(() => stream),
                            HeightRequest = 100,
                            WidthRequest = 100
                        };

                        ImageContainer.Children.Add(image);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to upload photos: " + ex.Message, "OK");
            }
        }

        private async void OnOkClicked(object sender, EventArgs e)
        {
            // Get user input
            string title = TitleEntry.Text;
            string description = DescriptionEditor.Text;
            string priceText = PriceEntry.Text;
            string plant = PlantPicker.SelectedItem?.ToString();
            string type = TypePicker.SelectedItem?.ToString();
            string stage = StagePicker.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(priceText) || string.IsNullOrWhiteSpace(plant) ||
                string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(stage))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            if (!int.TryParse(priceText, out int price))
            {
                await DisplayAlert("Error", "Price must be a valid number", "OK");
                return;
            }

            if (uploadedImages.Count == 0)
            {
                await DisplayAlert("Error", "Please upload at least one photo", "OK");
                return;
            }

            // Create new listing object
            //TODO: make it trigger a put api request
            //lehet hogy úgy kell hogy a viewmodel listinglistjéhez adod hozzá somehow, egyelõre csak helyben létrehozza és nem csinál vele semmit
            var newListing = new
            {
                Title = title,
                Description = description,
                Media = uploadedImages, // List of image paths
                Price = price,
                Plant = plant,
                Type = type,
                Stage = stage
            };

            await DisplayAlert("Success", "Listing added successfully!", "OK");

            await Shell.Current.GoToAsync(".."); // Goes back to the previous page in Shell
        }


        private async void OnMyListingsClicked(object sender, EventArgs e)
        {
            // Navigate to MyListingsPage
            await Shell.Current.GoToAsync("MyListingsPage");
        }
    }
}
