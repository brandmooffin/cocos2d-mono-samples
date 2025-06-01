using System;
using System.Collections.Generic;
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Dynamics.Contacts;
using Box2D.Collision;
using Microsoft.Xna.Framework.Input;
using CocosDenshion;

namespace Platformer
{
    public class GameLayer : CCLayer
    {
        // Box2D world
        private b2World _world;

        // Game objects
        private Player _player;
        private List<Platform> _platforms = new List<Platform>();

        // Input state
        private bool _isLeftPressed;
        private bool _isRightPressed;
        private bool _isJumpPressed;
        
        private int _score = 0;
        private CCLabelTTF _scoreLabel;
        private CCMenuItemLabel _restartButton;
        
        private ContactListener contactListener;

        public GameLayer()
        {
            // Initialize physics world with gravity
            _world = new b2World(new b2Vec2(0, -10.0f));
            
            // Set up contact listener for collisions
            contactListener = new ContactListener();
            _world.SetContactListener(contactListener);

            // Create level
            CreateLevel();

            // Load sounds
            CCSimpleAudioEngine.SharedEngine.PreloadEffect("jump");
            CCSimpleAudioEngine.SharedEngine.PreloadEffect("land");

            ScheduleUpdate();
        }

        private void CreateLevel()
        {
            // Get visible area size
            CCSize visibleSize = CCDirector.SharedDirector.WinSize;

            // Create background
            CCSprite background = new CCSprite("background");
            background.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height / 2);
            background.Scale = Math.Max(visibleSize.Width / background.ContentSize.Width,
                                       visibleSize.Height / background.ContentSize.Height);
            AddChild(background, -1);

            // Create player
            _player = new Player(_world);
            _player.Position = new CCPoint(100, 300);
            AddChild(_player);

            // Create score label
            _scoreLabel = new CCLabelTTF($"Score: {_score}", "MarkerFelt", 22);
            _scoreLabel.Position = new CCPoint(100, visibleSize.Height - 30);
            _scoreLabel.Color = CCColor3B.Black;
            AddChild(_scoreLabel, 10);

            // Create restart button
            CCLabelTTF restartLabel = new CCLabelTTF("Restart", "MarkerFelt", 22);
            restartLabel.Color = CCColor3B.Black;
            _restartButton = new CCMenuItemLabel(restartLabel, RestartGame);
            _restartButton.Position = new CCPoint(visibleSize.Width - 100, visibleSize.Height - 30);

            CCMenu menu = new CCMenu(_restartButton);
            menu.Position = CCPoint.Zero;
            AddChild(menu, 10);
            
            // Create floor platform
            Platform floor = new Platform(_world, visibleSize.Width / 2, 32, visibleSize.Width, 64);
            _platforms.Add(floor);
            AddChild(floor);

            // Create some platforms
            Platform platform1 = new Platform(_world, 200, 100, 200, 32);
            _platforms.Add(platform1);
            AddChild(platform1);

            Platform platform2 = new Platform(_world, 600, 100, 200, 32);
            _platforms.Add(platform2);
            AddChild(platform2);
        }

        private void RestartGame(object sender)
        {
            // Reset score
            _score = 0;

            RemoveAllChildren();
            CreateLevel();
        }

        public void IncreaseScore(int points)
        {
            _score += points;
            _scoreLabel.Text = $"Score: {_score}";
        }

        public override void Update(float dt)
        {
            // Update physics world
            _world.Step(dt, 8, 3);

            // Update player movement based on input
            if (_isLeftPressed)
                _player.MoveLeft();
            else if (_isRightPressed)
                _player.MoveRight();
            else
                _player.StopMoving();

            if (_isJumpPressed)
                _player.Jump();

            // Update all game objects
            _player.Update(dt);

            base.Update(dt);

            // Handle keyboard state every frame            
            HandleInput();
        }

        public void HandleInput()
        {
            // Reset input state
            _isLeftPressed = false;
            _isRightPressed = false;
            _isJumpPressed = false;

            // Handle keyboard input
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Left))
                _isLeftPressed = true;
            if (state.IsKeyDown(Keys.Right))
                _isRightPressed = true;
            if (state.IsKeyDown(Keys.Space))
                _isJumpPressed = true;
        }
    }
}