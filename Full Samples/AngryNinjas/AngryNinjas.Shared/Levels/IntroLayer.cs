using Cocos2D;

namespace AngryNinjas.Shared.Levels
{
    public class IntroLayer : CCLayer
    {
        CCSize _screenSize;

        protected override void AddedToScene()
        {
            base.AddedToScene();
            //get screen size
            _screenSize = CCDirector.SharedDirector.WinSize; //CCDirector::sharedDirector()->getWinSize();
            var background = new CCSprite("IntroLayer");
            background.Position = new CCPoint(_screenSize.Width / 2, _screenSize.Height / 2);

            // add the background as a child to this Layer
            AddChild(background);

            // Wait a little and then transition to the new scene
            ScheduleOnce(MakeTransition, 2);
        }

        public void MakeTransition(float dt)
        {
            CCLog.Log("Make Transition to Level");
            // CCDirector.SharedDirector.ReplaceScene(new CCTransitionFade(1, TheLevel.Scene, CCColor3B.White));
            CCDirector.SharedDirector.ReplaceScene(TheLevel.GetScene());
        }

        public static CCScene Scene
        {
            get
            {
                // 'scene' is an autorelease object.
                var scene = new CCScene();

                // 'layer' is an autorelease object.
                var layer = new IntroLayer();

                // add layer as a child to scene
                scene.AddChild(layer);

                // return the scene
                return scene;
            }

        }
    }
}
