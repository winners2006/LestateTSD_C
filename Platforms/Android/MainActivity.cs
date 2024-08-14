using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace TSDLestate
{
	[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
	public class MainActivity : MauiAppCompatActivity
	{
		private static string _scanActionName = "com.xcheng.scanner.action.BARCODE_DECODING_BROADCAST";
		BroadcastReceiverTest receiver;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			receiver = new BroadcastReceiverTest();
		}

		protected override void OnResume()
		{
			base.OnResume();
			RegisterReceiver(receiver, new IntentFilter(_scanActionName));
		}

		protected override void OnPause()
		{
			UnregisterReceiver(receiver);
			base.OnPause();
		}
	}

	public class BroadcastReceiverTest : BroadcastReceiver
	{
		public static event Action<string> OnScanReceived;

		public override void OnReceive(Context context, Intent intent)
		{
			var code = intent.GetStringExtra("EXTRA_BARCODE_DECODING_DATA");
			OnScanReceived?.Invoke(code);
		}
	}
}
