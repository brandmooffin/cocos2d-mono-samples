using Cocos2D;
using Box2D.Dynamics;
using System;

namespace Platformer
{
    public class GameLayer : CCLayer
    {
        // Physics world
        private b2World physicsWorld;
        private b2Body groundBody;
        
        // Physics settings
        private const int VELOCITY_ITERATIONS = 8;
        private const int POSITION_ITERATIONS = 3;
        private const float TIME_STEP = 1.0f / 60.0f;
        
        public GameLayer()
        {
            // Get visible area size
            CCSize visibleSize = CCDirector.SharedDirector.WinSize;
            
            // Initialize physics world
            InitializePhysics(visibleSize);
            
            // Create background
            CCSprite background = new CCSprite("background");
            background.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height / 2);
            
            // Scale background to fit screen
            float scaleX = visibleSize.Width / background.ContentSize.Width;
            float scaleY = visibleSize.Height / background.ContentSize.Height;
            background.Scale = Math.Max(scaleX, scaleY);
            
            AddChild(background, -1);
            
            // Create some test physics objects
            CreateTestObjects(visibleSize);
            
            // Add labels for information
            CCLabelTTF titleLabel = new CCLabelTTF("Platformer Tutorial - Part 2: Physics", "Arial", 24);
            titleLabel.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height - 30);
            titleLabel.Color = CCColor3B.White;
            AddChild(titleLabel);
            
            CCLabelTTF infoLabel = new CCLabelTTF("Physics World Active - Objects will fall!", "Arial", 16);
            infoLabel.Position = new CCPoint(visibleSize.Width / 2, visibleSize.Height - 60);
            infoLabel.Color = CCColor3B.Yellow;
            AddChild(infoLabel);
            
            // Enable updates to step the physics world
            ScheduleUpdate();
        }
        
        private void InitializePhysics(CCSize worldSize)
        {
            // Create physics world
            physicsWorld = PhysicsHelper.CreateWorld();
            
            // Create world boundaries
            groundBody = PhysicsHelper.CreateWorldBoundaries(physicsWorld, worldSize);
            
            // Set up contact listener for collision detection (we'll expand this later)
            // physicsWorld.SetContactListener(new ContactListener());
        }
        
        private void CreateTestObjects(CCSize visibleSize)
        {
            // Create some test boxes to demonstrate physics
            for (int i = 0; i < 3; i++)
            {
                // Create a visual sprite
                CCSprite testBox = new CCSprite("platform"); // Using platform texture as test
                testBox.Position = new CCPoint(200 + i * 100, 400 + i * 50);
                testBox.Color = new CCColor3B((byte)(100 + i * 50), (byte)(150 - i * 30), (byte)(200));
                AddChild(testBox, 1);
                
                // Create corresponding physics body
                b2Body physicsBody = PhysicsHelper.CreateDynamicBody(
                    physicsWorld, 
                    testBox.Position, 
                    testBox.ContentSize,
                    1.0f
                );
                
                // Store reference to sprite in physics body user data
                physicsBody.UserData = testBox;
            }
            
            // Create a static platform to catch falling objects
            CCSprite platform = new CCSprite("platform");
            platform.Position = new CCPoint(visibleSize.Width / 2, 150);
            platform.ScaleX = 4.0f; // Make it wider
            AddChild(platform, 1);
            
            // Create static physics body for platform
            b2Body platformBody = PhysicsHelper.CreateStaticBody(
                physicsWorld,
                platform.Position,
                new CCSize(platform.ContentSize.Width * platform.ScaleX, platform.ContentSize.Height)
            );
            platformBody.UserData = platform;
        }
        
        public override void Update(float dt)
        {
            base.Update(dt);
            
            // Step the physics world
            physicsWorld.Step(TIME_STEP, VELOCITY_ITERATIONS, POSITION_ITERATIONS);
            
            // Update visual positions based on physics bodies
            UpdateVisualPositions();
        }
        
        private void UpdateVisualPositions()
        {
            // Iterate through all physics bodies and update their corresponding sprites
            for (b2Body body = physicsWorld.BodyList; body != null; body = body.Next)
            {
                if (body.UserData is CCSprite sprite)
                {
                    // Convert physics position back to cocos2d coordinates
                    CCPoint newPosition = PhysicsHelper.VectorToPoint(body.Position);
                    sprite.Position = newPosition;
                    
                    // Update rotation if needed
                    sprite.Rotation = -CCMathHelper.ToDegrees(body.Angle);
                }
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Clean up physics world
                physicsWorld.Dump();
            }
            base.Dispose(disposing);
        }
    }
}