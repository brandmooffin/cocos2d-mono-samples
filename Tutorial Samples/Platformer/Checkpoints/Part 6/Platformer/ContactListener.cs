using System.Collections.Generic;
using Box2D.Collision;
using Box2D.Dynamics;
using Box2D.Dynamics.Contacts;

namespace Platformer
{
    public class ContactListener : b2ContactListener
    {
        // Contact callbacks run in the middle of world.Step, while the world
        // is locked and the order of same-step contacts is unspecified. So
        // the callbacks below only RECORD what happened; GameLayer.Update
        // resolves the results after the step, where stomps can reliably win
        // over side hits and physics bodies can safely be destroyed.
        private readonly List<Enemy> _pendingStomps = new List<Enemy>();
        private readonly List<Enemy> _pendingSideHits = new List<Enemy>();
        private readonly List<Collectible> _pendingCollections = new List<Collectible>();

        public List<Enemy> PendingStomps { get { return _pendingStomps; } }
        public List<Enemy> PendingSideHits { get { return _pendingSideHits; } }
        public List<Collectible> PendingCollections { get { return _pendingCollections; } }

        public override void BeginContact(b2Contact contact)
        {
            // Check for foot sensor contacts to enable jumping
            CheckFootContact(contact.GetFixtureA(), contact.GetFixtureB(), true);
            CheckFootContact(contact.GetFixtureB(), contact.GetFixtureA(), true);

            // Check for collectible contacts
            CheckCollectibleContact(contact.GetFixtureA(), contact.GetFixtureB());
            CheckCollectibleContact(contact.GetFixtureB(), contact.GetFixtureA());

            // Check for enemy contacts (stomp or side hit)
            CheckEnemyContact(contact.GetFixtureA(), contact.GetFixtureB());
            CheckEnemyContact(contact.GetFixtureB(), contact.GetFixtureA());
        }

        private void CheckFootContact(b2Fixture fixtureA, b2Fixture fixtureB, bool began)
        {
            Player.FootSensorUserData footData = fixtureA.UserData as Player.FootSensorUserData;
            if (footData == null)
                return;

            // Only solid platform ground counts as standing on something.
            // The foot sensor also brushes coins and enemy head sensors,
            // and those must not reset the player's jumps mid-air.
            if (fixtureB.IsSensor ||
                fixtureB.Filter.categoryBits != PhysicsHelper.CATEGORY_PLATFORM)
                return;

            footData.Player.SetCanJump(began);
        }

        private void CheckEnemyContact(b2Fixture fixtureA, b2Fixture fixtureB)
        {
            // Stomp: the player's FOOT sensor touched an enemy's HEAD sensor -
            // the two sensor patterns from Parts 3 and 6 meeting in the middle.
            Enemy.HeadSensorUserData headData = fixtureA.UserData as Enemy.HeadSensorUserData;
            Player.FootSensorUserData footData = fixtureB.UserData as Player.FootSensorUserData;

            if (headData != null && footData != null)
            {
                // Only a falling player scores a stomp - the same contact
                // fires when jumping UP past the head zone, and that shouldn't
                // count.
                if (footData.Player.IsFalling &&
                    !_pendingStomps.Contains(headData.Enemy))
                {
                    _pendingStomps.Add(headData.Enemy);
                }
                return;
            }

            // Side hit: an enemy BODY fixture touched the player's BODY fixture.
            // The IsSensor check matters: without it, the player's foot sensor
            // brushing the enemy body would count as damage during a stomp.
            Enemy enemy = fixtureA.UserData as Enemy;
            if (enemy != null && !enemy.IsDefeated &&
                !fixtureB.IsSensor &&
                fixtureB.Filter.categoryBits == PhysicsHelper.CATEGORY_PLAYER)
            {
                if (!_pendingSideHits.Contains(enemy))
                {
                    _pendingSideHits.Add(enemy);
                }
            }
        }

        private void CheckCollectibleContact(b2Fixture fixtureA, b2Fixture fixtureB)
        {
            // Record a pickup when any PLAYER-category fixture touches a
            // coin. Both player fixtures qualify (the body, and the foot
            // sensor's default filter) - requiring a specific one would make
            // pickups depend on which fixture happens to overlap first.
            Collectible collectible = fixtureA.UserData as Collectible;
            if (collectible != null && !collectible.IsCollected &&
                fixtureB.Filter.categoryBits == PhysicsHelper.CATEGORY_PLAYER)
            {
                if (!_pendingCollections.Contains(collectible))
                {
                    _pendingCollections.Add(collectible);
                }
            }
        }

        public override void EndContact(b2Contact contact)
        {
            // Check for foot sensor contacts to disable jumping
            CheckFootContact(contact.GetFixtureA(), contact.GetFixtureB(), false);
            CheckFootContact(contact.GetFixtureB(), contact.GetFixtureA(), false);
        }

        public override void PostSolve(b2Contact contact, ref b2ContactImpulse impulse)
        {

        }

        public override void PreSolve(b2Contact contact, b2Manifold oldManifold)
        {

        }
    }
}
