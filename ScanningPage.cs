using System.Text.Json;

namespace LestateTSD
{
	public partial class ScanningPage : ContentPage
	{
		private List<Goods> _goods = new List<Goods>();
		private List<Goods> _barcodeArray = new List<Goods>();
		private List<Goods> _datamatrixArray = new List<Goods>();
		private List<Goods> _noMarkingItems = new List<Goods>();

		private readonly Entry _textMessageEntry = new Entry { IsReadOnly = true, HorizontalTextAlignment = TextAlignment.Center };
		private readonly Entry _batchEntry = new Entry();
		private readonly Entry _markingValueEntry = new Entry();

		private bool _awaitingMarkingScan = false;
		private Goods _currentMarkedItem;

		public ScanningPage()
		{
			Title = "Lestate TSD";

			var picker = new Picker
			{
				Items = { "Отправить", "Очистить" }
			};

			picker.SelectedIndexChanged += async (sender, e) =>
			{
				if (picker.SelectedIndex == 0)
				{
					await ShowSendDialog();
				}
				else
				{
					_barcodeArray.Clear();
					_datamatrixArray.Clear();
					_noMarkingItems.Clear();
				}
			};

			Content = new StackLayout
			{
				Children =
				{
					_textMessageEntry,
					new Label { Text = "Общее кол-во товаров: 0", FontSize = 12, FontAttributes = FontAttributes.Bold },
					new ListView
					{
						ItemsSource = _barcodeArray,
						ItemTemplate = new DataTemplate(() =>
						{
							var vendorCodeLabel = new Label();
							vendorCodeLabel.SetBinding(Label.TextProperty, "VendorCode");

							var batchLabel = new Label();
							batchLabel.SetBinding(Label.TextProperty, "Batch");

							return new ViewCell
							{
								View = new StackLayout
								{
									Orientation = StackOrientation.Horizontal,
									Children = { vendorCodeLabel, batchLabel }
								}
							};
						})
					},
					picker
				}
			};

			GetResult();
		}

		private async void GetResult()
		{
			_goods = await HttpClientService.GetGoods();
		}

		private async Task ShowSendDialog()
		{
			var commentEntry = new Entry { Placeholder = "Введите комментарий" };
			await DisplayAlert("Подтверждение отправки", "Общее кол-во товаров: " + _barcodeArray.Count, "Отправить");
			await OnButtonClicked(commentEntry.Text);
		}

		private async Task OnButtonClicked(string comment)
		{
			var itemStrings = new List<object>();
			foreach (var item in _barcodeArray)
			{
				itemStrings.Add(new { barcode = item.Barcode, count = item.Count });
			}

			var itemStringsMatrix = new List<object>();
			foreach (var item in _datamatrixArray)
			{
				itemStringsMatrix.Add(new { barcode = item.Barcode, datamatrix = item.DataMatrix });
			}

			var body = new
			{
				barcodeArray = itemStrings,
				datamatrixArray = itemStringsMatrix,
				comment
			};

			string combinedString = JsonSerializer.Serialize(body);

			await HttpClientService.SetMovementosGoods(combinedString);

			if (HttpClientService.Result)
			{
				_textMessageEntry.Text = "Данные отправлены!";
				_barcodeArray.Clear();
				_datamatrixArray.Clear();
				_noMarkingItems.Clear();
			}
		}
	}
}
