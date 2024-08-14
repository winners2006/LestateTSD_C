using System.Text.Json;

namespace LestateTSD
{
	public class HttpClientService
	{
		public static string Username { get; set; }
		public static string Password { get; set; }
		public static string Error { get; set; }
		public static bool Result { get; set; }

		public static async Task<List<Goods>> GetGoods()
		{
			string basicAuth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{Username}:{Password}"));
			List<Goods> goods = new List<Goods>();

			try
			{
				using (var client = new System.Net.Http.HttpClient())
				{
					client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basicAuth);
					var response = await client.GetAsync("http://1c.sportpoint.ru:5055/retail/hs/apitsd/barcodes");

					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsByteArrayAsync();
						var directory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
						var filePath = System.IO.Path.Combine(directory, "barcodes.zip");

						System.IO.File.WriteAllBytes(filePath, content);

						using (var zip = System.IO.Compression.ZipFile.OpenRead(filePath))
						{
							foreach (var entry in zip.Entries)
							{
								if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
								{
									using (var stream = entry.Open())
									using (var reader = new System.IO.StreamReader(stream))
									{
										var jsonContent = await reader.ReadToEndAsync();
										goods = JsonSerializer.Deserialize<List<Goods>>(jsonContent);
									}
								}
							}
						}
					}
					else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
					{
						Error = "Неверный логин или пароль";
					}
					else
					{
						Error = "Ошибка сервера: " + response.StatusCode.ToString();
					}
				}
			}
			catch (System.Net.Http.HttpRequestException ex)
			{
				Error = "Ошибка сети: " + ex.Message;
			}
			catch (Exception ex)
			{
				Error = "Неизвестная ошибка: " + ex.Message;
			}

			return goods;
		}

		public static async Task SetMovementosGoods(string to1c)
		{
			string basicAuth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{Username}:{Password}"));

			try
			{
				using (var client = new System.Net.Http.HttpClient())
				{
					client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", basicAuth);
					var content = new System.Net.Http.StringContent(to1c, System.Text.Encoding.UTF8, "application/json");
					var response = await client.PostAsync("http://1c.sportpoint.ru:5055/retail/hs/apitsd/data", content);

					Result = response.IsSuccessStatusCode;

					if (!response.IsSuccessStatusCode)
					{
						Error = "Ошибка сервера при отправке данных: " + response.StatusCode.ToString();
					}
				}
			}
			catch (System.Net.Http.HttpRequestException ex)
			{
				Error = "Ошибка сети при отправке данных: " + ex.Message;
			}
			catch (Exception ex)
			{
				Error = "Неизвестная ошибка при отправке данных: " + ex.Message;
			}
		}
	}
}
