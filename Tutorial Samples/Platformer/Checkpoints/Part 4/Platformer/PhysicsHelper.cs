using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using Cocos2D;

namespace Platformer
{
    public static class PhysicsHelper
    {
        // Pixels to meters conversion ratio
        // Box2D works best with objects between 0.1 and 10 meters
        public const float PTM_RATIO = 32.0f;
        
        // Categories for collision filtering
        public const ushort CATEGORY_PLAYER = 0x0001;
        public const ushort CATEGORY_PLATFORM = 0x0002;
        public const ushort CATEGORY_COLLECTIBLE = 0x0004;
        
        // Convert from cocos2d coordinates to Box2D coordinates
        public static b2Vec2 ToPhysicsVector(CCPoint point)
        {
            return new b2Vec2(point.X / PTM_RATIO, point.Y / PTM_RATIO);
        }
        
        // Convert from Box2D coordinates to cocos2d coordinates
        public static CCPoint ToCocosVector(b2Vec2 vector)
        {
            return new CCPoint(vector.x * PTM_RATIO, vector.y * PTM_RATIO);
        }
        
        /// <summary>
        /// Convert pixels to meters for Box2D
        /// </summary>
        public static float PixelsToMeters(float pixels)
        {
            return pixels / PTM_RATIO;
        }
        
        /// <summary>
        /// Convert meters to pixels for cocos2d
        /// </summary>
        public static float MetersToPixels(float meters)
        {
            return meters * PTM_RATIO;
        }
        
        /// <summary>
        /// Convert cocos2d point to Box2D vector
        /// </summary>
        public static b2Vec2 PointToVector(CCPoint point)
        {
            return new b2Vec2(PixelsToMeters(point.X), PixelsToMeters(point.Y));
        }
        
        /// <summary>
        /// Convert Box2D vector to cocos2d point
        /// </summary>
        public static CCPoint VectorToPoint(b2Vec2 vector)
        {
            return new CCPoint(MetersToPixels(vector.x), MetersToPixels(vector.y));
        }
        
        /// <summary>
        /// Create a Box2D world with standard gravity
        /// </summary>
        public static b2World CreateWorld()
        {
            b2Vec2 gravity = new b2Vec2(0.0f, -9.8f); // Standard Earth gravity
            return new b2World(gravity);
        }
        
        /// <summary>
        /// Create a static ground body for the world boundaries
        /// </summary>
        public static b2Body CreateWorldBoundaries(b2World world, CCSize worldSize)
        {
            // Create body definition
            b2BodyDef groundBodyDef = new b2BodyDef();
            groundBodyDef.position.Set(0, 0);
            
            // Create the ground body
            b2Body groundBody = world.CreateBody(groundBodyDef);
            
            // Convert world size to meters
            float worldWidth = PixelsToMeters(worldSize.Width);
            float worldHeight = PixelsToMeters(worldSize.Height);
            
            // Create ground edge (bottom)
            b2EdgeShape groundEdge = new b2EdgeShape();
            groundEdge.Set(new b2Vec2(0, 0), new b2Vec2(worldWidth, 0));
            groundBody.CreateFixture(groundEdge, 0.0f);
            
            // Create ceiling edge (top) - optional
            b2EdgeShape ceilingEdge = new b2EdgeShape();
            ceilingEdge.Set(new b2Vec2(0, worldHeight), new b2Vec2(worldWidth, worldHeight));
            groundBody.CreateFixture(ceilingEdge, 0.0f);
            
            // Create left wall
            b2EdgeShape leftWall = new b2EdgeShape();
            leftWall.Set(new b2Vec2(0, 0), new b2Vec2(0, worldHeight));
            groundBody.CreateFixture(leftWall, 0.0f);
            
            // Create right wall
            b2EdgeShape rightWall = new b2EdgeShape();
            rightWall.Set(new b2Vec2(worldWidth, 0), new b2Vec2(worldWidth, worldHeight));
            groundBody.CreateFixture(rightWall, 0.0f);
            
            return groundBody;
        }
        
        /// <summary>
        /// Create a dynamic Box2D body for game objects
        /// </summary>
        public static b2Body CreateDynamicBody(b2World world, CCPoint position, CCSize size, float density = 1.0f)
        {
            // Create body definition
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_dynamicBody;
            bodyDef.position = PointToVector(position);
            
            // Create the body
            b2Body body = world.CreateBody(bodyDef);
            
            // Create box shape
            b2PolygonShape shape = new b2PolygonShape();
            shape.SetAsBox(PixelsToMeters(size.Width / 2), PixelsToMeters(size.Height / 2));
            
            // Create fixture
            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = density;
            fixtureDef.friction = 0.3f;
            fixtureDef.restitution = 0.0f; // No bouncing by default
            
            body.CreateFixture(fixtureDef);
            
            return body;
        }
        
        /// <summary>
        /// Create a static Box2D body for platforms
        /// </summary>
        public static b2Body CreateStaticBody(b2World world, CCPoint position, CCSize size)
        {
            // Create body definition
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_staticBody;
            bodyDef.position = PointToVector(position);
            
            // Create the body
            b2Body body = world.CreateBody(bodyDef);
            
            // Create box shape
            b2PolygonShape shape = new b2PolygonShape();
            shape.SetAsBox(PixelsToMeters(size.Width / 2), PixelsToMeters(size.Height / 2));
            
            // Create fixture
            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.friction = 0.7f; // More friction for platforms
            
            body.CreateFixture(fixtureDef);
            
            return body;
        }
    }
}