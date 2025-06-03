using System;
using Cocos2D;
using CocosDenshion;
using Microsoft.Xna.Framework;

namespace Platformer
{
    /// <summary>
    /// This is your extension of the main Cocos2D application object.
    /// </summary>
    internal class AppDelegate : CCApplication
    {
        public AppDelegate(Game game, GraphicsDeviceManager graphics)
            : base(game, graphics)
        {
            s_pSharedApplication = this;
            CCDrawManager.InitializeDisplay(game, graphics, DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight);
        }

        /// <summary>
        ///  Implement CCDirector and CCScene init code here.
        /// </summary>
        /// <returns>
        ///  true  Initialize success, app should continue.
        ///  false Initialize failed, app should terminate.
        /// </returns>
        public override bool ApplicationDidFinishLaunching()
        {
            CCSimpleAudioEngine.SharedEngine.SaveMediaState();

            CCDirector pDirector = null;
            try
            {
                // Set design resolution
                CCDrawManager.SetDesignResolutionSize(800, 600, CCResolutionPolicy.ShowAll);
                CCApplication.SharedApplication.GraphicsDevice.Clear(Color.Black);
                
                // Initialize director
                pDirector = CCDirector.SharedDirector;
                pDirector.SetOpenGlView();

                // Turn on display FPS (optional, for debugging)
                pDirector.DisplayStats = false;

                // Set FPS
                pDirector.AnimationInterval = 1.0 / 60;
                
                // Create and run scene
                CCScene scene = new CCScene();
                scene.AddChild(new GameLayer());
                pDirector.RunWithScene(scene);
            }
            catch (Exception ex)
            {
                CCLog.Log("ApplicationDidFinishLaunching(): Error " + ex.ToString());
            }
            return true;
        }

        public override void ApplicationDidEnterBackground()
        {
            CCDirector.SharedDirector.Pause();
            CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic();
        }

        public override void ApplicationWillEnterForeground()
        {
            CCDirector.SharedDirector.Resume();
            CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic();
        }
    }
}
