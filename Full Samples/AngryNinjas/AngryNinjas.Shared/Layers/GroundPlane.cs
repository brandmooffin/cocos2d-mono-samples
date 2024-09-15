using AngryNinjas.Shared.Entities;
using Box2D.Collision.Shapes;
using Box2D.Common;
using Box2D.Dynamics;
using Cocos2D;
using AngryNinjas.Shared.Custom;
using AngryNinjas.Shared.Levels;

namespace AngryNinjas.Shared.Layers
{
    public class GroundPlane : BodyNode
    {
        b2World theWorld;
        string spriteImageName;
        CCPoint initialLocation;

        public GroundPlane(b2World world, CCPoint location, string spriteFileName)
        {
            InitWithWorld(world, location, spriteFileName);
        }

        private void InitWithWorld(b2World world, CCPoint location, string spriteFileName)
        {
            this.theWorld = world;
            this.initialLocation = location;
            this.spriteImageName = spriteFileName;
            CreateGround();
        }

        private void CreateGround()
        {
            // Define the dynamic body.
            var bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_staticBody; //or you could use b2_staticBody
            bodyDef.position.Set(initialLocation.X / Constants.PTM_RATIO, initialLocation.Y / Constants.PTM_RATIO);
            b2PolygonShape shape = new b2PolygonShape();

            int num = 4;
            b2Vec2[] vertices = {
                new b2Vec2(-1220.0f / Constants.PTM_RATIO, 54.0f / Constants.PTM_RATIO),
                new b2Vec2(-1220.0f / Constants.PTM_RATIO, -52.0f / Constants.PTM_RATIO),
                new b2Vec2(1019.0f / Constants.PTM_RATIO, -52.0f / Constants.PTM_RATIO),
                new b2Vec2(1019.0f / Constants.PTM_RATIO, 54.0f / Constants.PTM_RATIO)
            };

            shape.Set(vertices, num);

            // Define the dynamic body fixture.
            var fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = 1.0f;
            fixtureDef.friction = 1.0f;
            fixtureDef.restitution = 0.1f;

            CreateBodyWithSpriteAndFixture(theWorld, bodyDef, fixtureDef, spriteImageName);

            if (!TheLevel.SharedLevel.IS_RETINA)
            {
                //non retina adjustment
                sprite.ScaleX = 1.05f;

            }
            else
            {
                // retina adjustment
                sprite.ScaleX = 2.05f;
                sprite.ScaleY = 2.0f;
            }
        }
    }
}
