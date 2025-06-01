using Cocos2D;
using Box2D.Dynamics;

namespace Platformer
{
    public class Platform : CCSprite
    {
        // Physics body
        private b2Body _body;

        public Platform(b2World world, float posX, float posY, float width, float height) : base("platform")
        {
            // Set position and scale to match desired dimensions
            this.Position = new CCPoint(posX, posY);
            this.ScaleX = width / this.ContentSize.Width;
            this.ScaleY = height / this.ContentSize.Height;

            // Create physics body
            _body = PhysicsHelper.CreateBoxBody(
                world,
                posX,
                posY,
                width,
                height,
                false,  // Static body
                1.0f,   // Density
                0.3f,   // Friction
                0.0f    // No bounce
            );

            // Set collision filtering
            b2Fixture fixture = _body.FixtureList;
            b2Filter filter = fixture.Filter;
            filter.categoryBits = PhysicsHelper.CATEGORY_PLATFORM;
            filter.maskBits = PhysicsHelper.CATEGORY_PLAYER;
            fixture.SetFilterData(filter);

            // Store reference to this platform
            _body.UserData = this;
        }
    }
}