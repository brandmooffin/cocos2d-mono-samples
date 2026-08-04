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

            // Start the sprite at the spawn point - otherwise the first frame
            // draws at (0,0) before Update syncs it to the physics body
            Position = new CCPoint(x, y);

            // Create physics body (dynamic, like the player: it walks and falls)
            b2BodyDef bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_dynamicBody;
            bodyDef.fixedRotation = true;
            bodyDef.allowSleep = false;
            bodyDef.position = new b2Vec2(x / PhysicsHelper.PTM_RATIO, y / PhysicsHelper.PTM_RATIO);

            _body = world.CreateBody(bodyDef);

            // Body fixture - collides with platforms and the player.
            // Sized to the VISIBLE art, not the texture: player_idle's 64x64
            // canvas is mostly transparent padding, with the ~26x31 character
            // sitting bottom-center. Boxing the whole texture would let the
            // empty air beside the enemy hit the player.
            b2PolygonShape shape = new b2PolygonShape();
            shape.SetAsBox(
                ContentSize.Width * 0.2f / PhysicsHelper.PTM_RATIO,
                ContentSize.Height * 0.25f / PhysicsHelper.PTM_RATIO,
                new b2Vec2(0, -ContentSize.Height * 0.25f / PhysicsHelper.PTM_RATIO),
                0);

            b2FixtureDef fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = 1.0f;
            fixtureDef.friction = 0.2f;
            fixtureDef.restitution = 0.0f;
            fixtureDef.filter.categoryBits = PhysicsHelper.CATEGORY_ENEMY;
            fixtureDef.filter.maskBits = PhysicsHelper.CATEGORY_PLATFORM | PhysicsHelper.CATEGORY_PLAYER;

            _body.CreateFixture(fixtureDef).UserData = this;

            // Head sensor - the player defeats the enemy by landing on this.
            // The body box is bottom-aligned, so its top face - the visible
            // head - sits at the sprite's vertical center (y = 0). The sensor
            // is WIDER than the body box: wide enough that every position
            // where the player can physically stand on the enemy registers
            // as a stomp, leaving no edge to perch on.
            b2PolygonShape headShape = new b2PolygonShape();
            headShape.SetAsBox(
                ContentSize.Width * 0.35f / PhysicsHelper.PTM_RATIO,
                3f / PhysicsHelper.PTM_RATIO,
                new b2Vec2(0, 0),
                0);

            b2FixtureDef headFixtureDef = new b2FixtureDef();
            headFixtureDef.shape = headShape;
            headFixtureDef.isSensor = true;
            // Explicit filter: an unset filter defaults to category 0x0001 -
            // the same value as CATEGORY_PLAYER - with mask 0xFFFF, so the
            // head would masquerade as the player in category checks and
            // generate contacts with platforms, coins, and other enemies.
            headFixtureDef.filter.categoryBits = PhysicsHelper.CATEGORY_ENEMY;
            headFixtureDef.filter.maskBits = PhysicsHelper.CATEGORY_PLAYER;

            b2Fixture headSensor = _body.CreateFixture(headFixtureDef);
            headSensor.UserData = new HeadSensorUserData(this);
        }

        public void Update(float dt)
        {
            if (_isDefeated)
                return;

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

            // Defeat is resolved from GameLayer.Update, AFTER the physics
            // step - never from inside a contact callback, where the world
            // is locked and silently ignores DestroyBody. That makes it
            // safe to destroy the body right here.
            RemoveFromWorld();

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
