using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace EmbeddedSample
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Game",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.SensorLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize,
        Exported = true
    )]
    public class Activity1 : AndroidGameActivity
    {
        private CCGameView _gameView;
        private FrameLayout _gameContainer;
        private Button _btnPause;
        private Button _btnResume;
        private TextView _txtInfo;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Use the layout with native Android UI elements
            SetContentView(Resource.Layout.Main);

            // Get references to UI elements
            _gameContainer = FindViewById<FrameLayout>(Resource.Id.gameViewContainer);
            _btnPause = FindViewById<Button>(Resource.Id.btnPause);
            _btnResume = FindViewById<Button>(Resource.Id.btnResume);
            _txtInfo = FindViewById<TextView>(Resource.Id.txtInfo);

            // Wire up button events
            _btnPause.Click += (s, e) =>
            {
                if (_gameView != null)
                {
                    _gameView.Paused = true;
                    _txtInfo.Text = "Game paused";
                }
            };

            _btnResume.Click += (s, e) =>
            {
                if (_gameView != null)
                {
                    _gameView.Paused = false;
                    _txtInfo.Text = "Game resumed";
                }
            };

            // Create the CCGameView - size will be determined by the container
            _gameView = new CCGameView(this);
            _gameView.DesignResolution = new CCSize(1024, 768);
            _gameView.ResolutionPolicy = CCViewResolutionPolicy.ShowAll;

            // Subscribe to ViewCreated to know when the game is ready to run scenes
            _gameView.ViewCreated += OnViewCreated;

            // Start the game - this creates the internal MonoGame Game and makes
            // the Android View available (but doesn't start the game loop yet)
            _gameView.StartGame();

            // Add the game view to our container in the layout
            _gameContainer.AddView(_gameView.AndroidView);

            _txtInfo.Text = "Game view embedded in layout - use buttons to control";

            // Run the game loop (this will block and pump the game update/draw)
            _gameView.Run();
        }

        private void OnViewCreated(object sender, System.EventArgs e)
        {
            CCLog.Log("ViewCreated fired - starting IntroLayer scene");

            var scene = IntroLayer.Scene;
            _gameView.RunWithScene(scene);
            CCLog.Log("Scene started");
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (_gameView != null)
            {
                _gameView.Paused = true;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (_gameView != null)
            {
                _gameView.Paused = false;
            }
        }

        protected override void OnDestroy()
        {
            if (_gameView != null)
            {
                _gameView.Dispose();
                _gameView = null;
            }
            base.OnDestroy();
        }
    }
}
