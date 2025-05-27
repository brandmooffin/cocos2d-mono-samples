using System;
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace Platformer
{
    public class Player : CCSprite
    {
        // Physics body
        private b2Body _body;

        // Movement parameters
        private const float MOVE_SPEED = 5.0f;
        private const float JUMP_FORCE = 7.0f;
        private bool _canJump = false;
        private int _jumpCount = 0;
        private const int MAX_JUMPS = 2; // Allow double jump
        private bool _isRunning = false;

        // Animation states
        private CCAnimation _idleAnimation;
        private CCAnimation _runAnimation;
        private CCAnimation _jumpAnimation;

        public Player(b2World world) : base("player_idle")
        {
            // Create physics body
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_dynamicBody;
            bodyDef.fixedRotation = true; // Prevent rotation
            bodyDef.allowSleep = false;

            _body = world.CreateBody(bodyDef);

            // Create fixture
            b2PolygonShape shape = new b2PolygonShape();
            // Make the collision box slightly smaller than the sprite
            shape.SetAsBox(
                ContentSize.Width * 0.4f / PhysicsHelper.PTM_RATIO,
                ContentSize.Height * 0.45f / PhysicsHelper.PTM_RATIO);

            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = 1.0f;
            fixtureDef.friction = 0.2f;
            fixtureDef.restitution = 0.0f;

            // Set collision filtering
            fixtureDef.filter.categoryBits = PhysicsHelper.CATEGORY_PLAYER;
            fixtureDef.filter.maskBits = PhysicsHelper.CATEGORY_PLATFORM | PhysicsHelper.CATEGORY_COLLECTIBLE;

            _body.CreateFixture(fixtureDef);

            // Add foot sensor for jump detection
            b2PolygonShape footShape = new b2PolygonShape();
            footShape.SetAsBox(
                ContentSize.Width * 0.3f / PhysicsHelper.PTM_RATIO,
                0.1f / PhysicsHelper.PTM_RATIO,
                new b2Vec2(0, -ContentSize.Height * 0.45f / PhysicsHelper.PTM_RATIO),
                0);

            b2FixtureDef footFixtureDef = new b2FixtureDef();
            footFixtureDef.shape = footShape;
            footFixtureDef.isSensor = true;

            b2Fixture footSensor = _body.CreateFixture(footFixtureDef);
            footSensor.UserData = new FootSensorUserData(this);

            // Load animations
            LoadAnimations();
        }

        private void LoadAnimations()
        {
            // In a real game, you would load animation frames
            // For this tutorial, we'll use placeholder logic

            _idleAnimation = new CCAnimation();
            // Add frames to animation
            _idleAnimation.AddSpriteFrameWithFileName("player_idle");
            _idleAnimation.DelayPerUnit = 0.2f;

            _runAnimation = new CCAnimation();
            // Add multiple frames for run animation
            _runAnimation.AddSpriteFrameWithFileName("player_run_1");
            _runAnimation.AddSpriteFrameWithFileName("player_run_2");
            _runAnimation.AddSpriteFrameWithFileName("player_run_3");
            _runAnimation.AddSpriteFrameWithFileName("player_run_4");
            _runAnimation.DelayPerUnit = 0.1f;

            _jumpAnimation = new CCAnimation();
            _jumpAnimation.AddSpriteFrameWithFileName("player_jump");
            _jumpAnimation.DelayPerUnit = 0.1f;
        }

        public void Update(float dt)
        {
            // Update sprite position based on physics body
            Position = PhysicsHelper.ToCocosVector(_body.Position);

            // Check if player fell off the screen
            if (Position.Y < -100)
            {
                // Reset position
                _body.SetTransform(new b2Vec2(100 / PhysicsHelper.PTM_RATIO, 300 / PhysicsHelper.PTM_RATIO), 0);
                _body.LinearVelocity = b2Vec2.Zero;
            }
        }

        public void MoveLeft()
        {
            _body.LinearVelocity = new b2Vec2(-MOVE_SPEED, _body.LinearVelocity.y);

            // Flip sprite to face left
            FlipX = true;

            // Play run animation if on ground
            if (_canJump && _jumpCount == 0 && !_isRunning)
            {
                _isRunning = true; // Set running state
                RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
            }
        }

        public void MoveRight()
        {
            _body.LinearVelocity = new b2Vec2(MOVE_SPEED, _body.LinearVelocity.y);

            // Flip sprite to face right
            FlipX = false;

            // Play run animation if on ground
            if (_canJump && _jumpCount == 0 && !_isRunning)
            {
                _isRunning = true; // Set running state
                RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
            }
        }

        public void StopMoving()
        {
            _body.LinearVelocity = new b2Vec2(0, _body.LinearVelocity.y);

            // Play idle animation if on ground
            if (_canJump && _jumpCount == 0)
            {
                _isRunning = false; // Stop running animation
                StopAllActions();
                RunAction(new CCRepeatForever(new CCAnimate(_idleAnimation)));
            }
        }

        public void Jump()
        {
            if (_canJump && _jumpCount < MAX_JUMPS)
            {
                _body.LinearVelocity = new b2Vec2(_body.LinearVelocity.x, JUMP_FORCE);
                _jumpCount++;
                _canJump = (_jumpCount < MAX_JUMPS);

                _isRunning = false; // Stop running animation when jumping
                // Play jump animation
                StopAllActions();
                RunAction(new CCAnimate(_jumpAnimation));

                // Play jump sound
                PlayJumpSound();
            }
        }

        public void SetCanJump(bool canJump)
        {
            if (canJump && !_canJump)
            {
                // Player just landed
                _jumpCount = 0;

                // Play landing sound
                PlayLandSound();

                // Play idle or run animation based on horizontal velocity
                StopAllActions();

                if (Math.Abs(_body.LinearVelocity.x) > 0.1f)
                {
                    RunAction(new CCRepeatForever(new CCAnimate(_runAnimation)));
                }
                else
                {
                    RunAction(new CCRepeatForever(new CCAnimate(_idleAnimation)));
                }
            }

            _canJump = canJump;
        }

        private void PlayJumpSound()
        {
            CCSimpleAudioEngine.SharedEngine.PlayEffect("jump");
        }

        private void PlayLandSound()
        {
            CCSimpleAudioEngine.SharedEngine.PlayEffect("land");
        }

        // User data for foot sensor
        public class FootSensorUserData
        {
            public Player Player { get; private set; }

            public FootSensorUserData(Player player)
            {
                Player = player;
            }
        }

    }
}