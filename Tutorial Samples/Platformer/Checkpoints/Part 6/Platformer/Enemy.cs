using System;
using Cocos2D;
using Box2D.Dynamics;
using Box2D.Common;
using Box2D.Collision.Shapes;
using CocosDenshion;

namespace Platformer
{
    public class Enemy : CCSprite
    {
        // Physics body
        private b2Body _body;

        // Patrol parameters
        private const float PATROL_SPEED = 2.0f;
        private readonly float _patrolMinX;
        private readonly float _patrolMaxX;
        private int _direction = 1; // 1 = right, -1 = left

        private bool _isDefeated;

        public bool IsDefeated { get { return _isDefeated; } }

        public Enemy(b2World world, float x, float y, float patrolMinX, float patrolMaxX)
            : base("player_idle")
        {
            // Placeholder art: reuse the player sprite with a red tint.
            // In a real game, swap in dedicated enemy artwork.
            Color = new CCColor3B(230, 60, 60);

            _patrolMinX = patrolMinX;
            _patrolMaxX = patrolMaxX;

            // Create physics body (dynamic, like the player: it walks and falls)
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_dynamicBody;
            bodyDef.fixedRotation = true;
            bodyDef.allowSleep = false;
            bodyDef.position = new b2Vec2(x / PhysicsHelper.PTM_RATIO, y / PhysicsHelper.PTM_RATIO);

            _body = world.CreateBody(bodyDef);

            // Body fixture - collides with platforms and the player
            b2PolygonShape shape = new b2PolygonShape();
            shape.SetAsBox(
                ContentSize.Width * 0.4f / PhysicsHelper.PTM_RATIO,
                ContentSize.Height * 0.45f / PhysicsHelper.PTM_RATIO);

            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = 1.0f;
            fixtureDef.friction = 0.2f;
            fixtureDef.restitution = 0.0f;
            fixtureDef.filter.categoryBits = PhysicsHelper.CATEGORY_ENEMY;
            fixtureDef.filter.maskBits = PhysicsHelper.CATEGORY_PLATFORM | PhysicsHelper.CATEGORY_PLAYER;

            _body.CreateFixture(fixtureDef).UserData = this;

            // Head sensor - the player defeats the enemy by landing on this.
            // Mirrors the player's foot sensor from Part 3, sitting on TOP of
            // the body - and slightly WIDER than the body box. If it were
            // narrower, the player could land on an uncovered edge of the
            // solid body and stand there without triggering the stomp.
            b2PolygonShape headShape = new b2PolygonShape();
            headShape.SetAsBox(
                ContentSize.Width * 0.5f / PhysicsHelper.PTM_RATIO,
                3f / PhysicsHelper.PTM_RATIO,
                new b2Vec2(0, ContentSize.Height * 0.45f / PhysicsHelper.PTM_RATIO),
                0);

            b2FixtureDef headFixtureDef = new b2FixtureDef();
            headFixtureDef.shape = headShape;
            headFixtureDef.isSensor = true;

            b2Fixture headSensor = _body.CreateFixture(headFixtureDef);
            headSensor.UserData = new HeadSensorUserData(this);
        }

        public void Update(float dt)
        {
            if (_isDefeated)
            {
                // Deferred cleanup: Defeat runs inside a contact callback,
                // while the world is locked mid-step and DestroyBody is
                // silently ignored. Update runs after the step, so the
                // body can be destroyed for real here.
                RemoveFromWorld();
                return;
            }

            // Sync the sprite with the physics body
            Position = PhysicsHelper.ToCocosVector(_body.Position);

            // Patrol AI: walk in the current direction, turn around at the bounds
            if (Position.X <= _patrolMinX)
                _direction = 1;
            else if (Position.X >= _patrolMaxX)
                _direction = -1;

            _body.LinearVelocity = new b2Vec2(PATROL_SPEED * _direction, _body.LinearVelocity.y);

            // Face the direction of travel
            FlipX = _direction < 0;
        }

        public void Defeat(GameLayer gameLayer)
        {
            if (_isDefeated)
                return;

            _isDefeated = true;

            // The body is NOT destroyed here: Defeat is called from a
            // contact callback, while the physics world is locked mid-step
            // and silently ignores DestroyBody. Update destroys it on the
            // next frame, once the step has finished.

            // Squash, fade, and remove the sprite
            RunAction(new CCSequence(
                new CCScaleTo(0.15f, 1.3f, 0.4f),
                new CCFadeOut(0.25f),
                new CCCallFunc(() => RemoveFromParent())
            ));

            // Reuse the landing sound as a defeat thump; a real game would
            // use a dedicated effect here.
            CCSimpleAudioEngine.SharedEngine.PlayEffect("land");

            gameLayer.IncreaseScore(25);
        }

        public void RemoveFromWorld()
        {
            if (_body != null)
            {
                _body.World.DestroyBody(_body);
                _body = null;
            }
        }

        // User data for the head sensor - lets the contact listener recognize
        // "the player landed on an enemy" (see ContactListener.CheckEnemyContact).
        public class HeadSensorUserData
        {
            public Enemy Enemy { get; private set; }

            public HeadSensorUserData(Enemy enemy)
            {
                Enemy = enemy;
            }
        }
    }
}
