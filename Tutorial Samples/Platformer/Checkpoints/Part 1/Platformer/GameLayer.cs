using System;
using Cocos2D;

namespace Platformer
{
    public class GameLayer : CCLayer
    {
        public GameLayer()
        {
            // Get visible area size
            CCSize visibleSize = CCDirector.SharedDirector.WinSize;
            
            // Create background
            CCSprite background = new CCSprite("background");
            background.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height / 2);
            
            // Scale background to fit screen
            float scaleX = visibleSize.Width / background.ContentSize.Width;
            float scaleY = visibleSize.Height / background.ContentSize.Height;
            background.Scale = Math.Max(scaleX, scaleY);
            
            AddChild(background, -1);
            
            // Add a simple label to confirm everything is working
            CCLabelTTF label = new CCLabelTTF("Platformer Tutorial - Part 1", "MarkerFelt", 22);
            label.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height - 50);
            label.Color = CCColor3B.Blue;
            AddChild(label);
        }
    }
}