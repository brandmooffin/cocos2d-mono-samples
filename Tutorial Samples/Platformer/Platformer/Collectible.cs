using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace Platformer
{
    public class Collectible : CCSprite
    {
        private b2Body _body;

        public Collectible(b2World world, float x, float y) : base("coin")
        {
            Position = new CCPoint(x, y);

            // Create physics body
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_staticBody;
            bodyDef.position = new b2Vec2(x / PhysicsHelper.PTM_RATIO, y / PhysicsHelper.PTM_RATIO);

            _body = world.CreateBody(bodyDef);

            // Create circle shape for collectible
            b2CircleShape shape = new b2CircleShape();
            shape.Radius = ContentSize.Width * 0.3f / PhysicsHelper.PTM_RATIO;

            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.isSensor = true; // Make it a sensor (no collision response)
            fixtureDef.filter.categoryBits = PhysicsHelper.CATEGORY_COLLECTIBLE;
            fixtureDef.filter.maskBits = PhysicsHelper.CATEGORY_PLAYER;

            _body.CreateFixture(fixtureDef).UserData = this;

            // Add a rotation action
            RunAction(new CCRepeatForever(new CCRotateBy(2.0f, 360)));
        }

        public void Collect(GameLayer gameLayer)
        {
            // Remove from physics world
            _body.World.DestroyBody(_body);
            _body = null;

            // Play collection animation
            RunAction(new CCSequence(
                new CCScaleTo(0.2f, 1.5f),
                new CCScaleTo(0.2f, 0.0f),
                new CCCallFunc(() => RemoveFromParent())
            ));

            // Play collection sound
            CCSimpleAudioEngine.SharedEngine.PlayEffect("coin");

            // Increase score
            gameLayer.IncreaseScore(10);
        }
    }
}