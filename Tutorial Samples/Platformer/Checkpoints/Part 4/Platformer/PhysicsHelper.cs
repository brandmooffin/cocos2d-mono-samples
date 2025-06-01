using Cocos2D;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;
using Box2D.Common;

namespace Platformer
{
    public static class PhysicsHelper
    {
        // Physics constants
        public const float PTM_RATIO = 32.0f; // Pixels to meters ratio

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
        
        // Create a rectangular physics body
        public static b2Body CreateBoxBody(b2World world, float x, float y, float width, float height,
            bool isDynamic = false, float density = 1.0f, float friction = 0.3f, float restitution = 0.1f)
        {
            // Define body
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.position = new b2Vec2(x / PTM_RATIO, y / PTM_RATIO);
            bodyDef.type = isDynamic ? b2BodyType.b2_dynamicBody : b2BodyType.b2_staticBody;

            // Create body
            b2Body body = world.CreateBody(bodyDef);

            // Define fixture
            b2PolygonShape shape = new b2PolygonShape();
            shape.SetAsBox(width / (2 * PTM_RATIO), height / (2 * PTM_RATIO));
            
            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = density;
            fixtureDef.friction = friction;
            fixtureDef.restitution = restitution;

            // Add fixture to body
            body.CreateFixture(fixtureDef);

            return body;
        }
    }
}