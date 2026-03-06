using Cocos2D;

namespace MultiViewSample
{
    public class SecondLayer : CCLayerColor
    {
        public SecondLayer()
        {
            // create and initialize a Label
            var label = new CCLabelTTF("Second View!", "arial", 22)
            {
                // position the label on the center of the screen
                Position = CCDirector.SharedDirector.WinSize.Center
            };

            // add the label as a child to this Layer
            AddChild(label);

            // setup our color for the background - use green to distinguish from IntroLayer
            Color = new CCColor3B(Microsoft.Xna.Framework.Color.Green);
            Opacity = 255;
        }

        public static CCScene Scene
        {
            get
            {
                var scene = new CCScene();
                var layer = new SecondLayer();
                scene.AddChild(layer);
                return scene;
            }
        }
    }
}
