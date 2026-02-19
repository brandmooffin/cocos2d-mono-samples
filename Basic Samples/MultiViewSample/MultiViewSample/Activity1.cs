using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Cocos2D;
using Microsoft.Xna.Framework;

namespace MultiViewSample
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Game",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Landscape | ScreenOrientation.ReverseLandscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class Activity1 : AndroidGameActivity
    {
        private CCGameView _gameView;
        private FrameLayout _gameContainer;
        private Button _btnPause;
        private Button _btnResume;
        private Button _btnSwitch;
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
            _btnSwitch = FindViewById<Button>(Resource.Id.btnSwitch);
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

            // Toggle split-screen mode - shows two scenes side by side
            _btnSwitch.Click += (s, e) =>
            {
                if (_gameView != null)
                {
                    _gameView.SplitScreenEnabled = !_gameView.SplitScreenEnabled;
                    if (_gameView.SplitScreenEnabled)
                    {
                        _txtInfo.Text = "Split-screen: IntroLayer (Blue) | SecondLayer (Green)";
                    }
                    else
                    {
                        _txtInfo.Text = "Single view: IntroLayer (Blue)";
                    }
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

            _txtInfo.Text = "Split-screen: IntroLayer (Blue) | SecondLayer (Green)";

            // Run the game loop (this will block and pump the game update/draw)
            _gameView.Run();
        }

        private void OnViewCreated(object sender, System.EventArgs e)
        {
            CCLog.Log("ViewCreated fired - starting scenes");

            // Set up the primary scene (left side in split-screen)
            var primaryScene = IntroLayer.Scene;
            _gameView.RunWithScene(primaryScene, useViewScene: true);

            // Set up the split-screen scene (right side in split-screen)
            var secondaryScene = SecondLayer.Scene;
            _gameView.SetSplitScreenScene(secondaryScene);

            // Start in split-screen mode to show both scenes
            _gameView.SplitScreenEnabled = true;

            CCLog.Log("Split-screen mode enabled with two scenes");
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
