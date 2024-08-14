using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace LestateTSD
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder.UseMauiApp<App>()
				.ConfigureFonts(fonts => {
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				})
			  .ConfigureLifecycleEvents(events => {
#if ANDROID
                  events.AddAndroid(android => android
                      .OnActivityResult((activity, requestCode, resultCode, data) => LogEvent(nameof(AndroidLifecycle.OnActivityResult), requestCode.ToString()))
                      .OnStart((activity) => LogEvent(nameof(AndroidLifecycle.OnStart)))
                      .OnCreate((activity, bundle) => LogEvent(nameof(AndroidLifecycle.OnCreate)))
                      .OnBackPressed((activity) => LogEvent(nameof(AndroidLifecycle.OnBackPressed)) && false)
                      .OnStop((activity) => LogEvent(nameof(AndroidLifecycle.OnStop))));
#endif
				  static bool LogEvent(string eventName, string type = null)
				  {
					  System.Diagnostics.Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? string.Empty : $" ({type})")}");
					  return true;
				  }
			  });

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
		static void Main() { }
	}
}