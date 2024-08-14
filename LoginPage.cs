namespace LestateTSD
{
	public partial class LoginPage : ContentPage
	{
		private readonly Entry _usernameEntry = new Entry { Placeholder = "Введите Ваш логин 1С" };
		private readonly Entry _passwordEntry = new Entry { Placeholder = "Введите Ваш пароль 1С", IsPassword = true };

		public LoginPage()
		{

			Title = "Lestate TSD";
			Content = new StackLayout
			{
				Padding = new Thickness(16),
				Children =
				{
					new Label { Text = "Авторизация", FontSize = 30 },
					new Image { Source = "logo.png", HeightRequest = 32 },
					new BoxView { HeightRequest = 20 },
					_usernameEntry,
					new BoxView { HeightRequest = 20 },
					_passwordEntry,
					new BoxView { HeightRequest = 20 },
					new Button
					{
						Text = "Авторизация",
						Command = new Command(OnLoginClicked)
					},
					new Label { Text = "v1.0.0", FontSize = 12, HorizontalOptions = LayoutOptions.End }
				}
			};
		}

		private async void OnLoginClicked()
		{
			HttpClientService.Username = _usernameEntry.Text;
			HttpClientService.Password = _passwordEntry.Text;

			await ShowLoadingDialog();
		}

		private async Task ShowLoadingDialog()
		{
			await DisplayAlert("Получение данных...", null, "OK");

			bool success = await ProcessJsonFile();
			if (success)
			{
				await Navigation.PushAsync(new ScanningPage());
			}
			else
			{
				await ShowErrorDialog();
			}
		}

		private async Task<bool> ProcessJsonFile()
		{
			List<Goods> goods = await HttpClientService.GetGoods();
			return goods.Count > 0;
		}

		private async Task ShowErrorDialog()
		{
			string errorMessage = !string.IsNullOrEmpty(HttpClientService.Error) ? HttpClientService.Error : "Неверный логин или пароль.";
			await DisplayAlert("Ошибка авторизации", errorMessage, "OK");
		}
	}
}