using Box2D.Collision;
using Box2D.Dynamics;
using Box2D.Dynamics.Contacts;

namespace Platformer
{
    public class ContactListener : b2ContactListener
    {
        public override void BeginContact(b2Contact contact)
        {
            // Check for foot sensor contacts to enable jumping
            object userDataA = contact.GetFixtureA().UserData;
            object userDataB = contact.GetFixtureB().UserData;

            Player.FootSensorUserData footData = userDataA as Player.FootSensorUserData
                                                 ?? userDataB as Player.FootSensorUserData;

            if (footData != null)
            {
                footData.Player.SetCanJump(true);
            }

            // Check for collectible contacts
            CheckCollectibleContact(contact.GetFixtureA(), contact.GetFixtureB());
            CheckCollectibleContact(contact.GetFixtureB(), contact.GetFixtureA());

            // Check for enemy contacts (stomp or side hit)
            CheckEnemyContact(contact.GetFixtureA(), contact.GetFixtureB());
            CheckEnemyContact(contact.GetFixtureB(), contact.GetFixtureA());
        }

        private void CheckEnemyContact(b2Fixture fixtureA, b2Fixture fixtureB)
        {
            // Stomp: the player's FOOT sensor touched an enemy's HEAD sensor -
            // the two sensor patterns from Parts 3 and 6 meeting in the middle.
            Enemy.HeadSensorUserData headData = fixtureA.UserData as Enemy.HeadSensorUserData;
            Player.FootSensorUserData footData = fixtureB.UserData as Player.FootSensorUserData;

            if (headData != null && footData != null)
            {
                if (headData.Enemy.Parent is GameLayer stompLayer)
                {
                    headData.Enemy.Defeat(stompLayer);
                    footData.Player.Bounce();
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
                if (enemy.Parent is GameLayer gameLayer)
                {
                    gameLayer.OnPlayerHit();
                }
            }
        }

        private void CheckCollectibleContact(b2Fixture fixtureA, b2Fixture fixtureB)
        {
            // Check if fixA is a collectible and fixB is the player
            Collectible collectible = fixtureA.UserData as Collectible;
            if (collectible != null &&
                fixtureB.Filter.categoryBits == PhysicsHelper.CATEGORY_PLAYER)
            {
                // Get the game layer from the player's parent
                Player.FootSensorUserData playerNode = fixtureB.UserData as Player.FootSensorUserData;
                if (playerNode != null && collectible.Parent is GameLayer gameLayer)
                {
                    collectible.Collect(gameLayer);
                }
            }
        }

        public override void EndContact(b2Contact contact)
        {
            // Check for foot sensor contacts to disable jumping
            object userDataA = contact.GetFixtureA().UserData;
            object userDataB = contact.GetFixtureB().UserData;

            Player.FootSensorUserData footData = userDataA as Player.FootSensorUserData
                                                 ?? userDataB as Player.FootSensorUserData;

            if (footData != null)
            {
                footData.Player.SetCanJump(false);
            }
        }

        public override void PostSolve(b2Contact contact, ref b2ContactImpulse impulse)
        {
            
        }

        public override void PreSolve(b2Contact contact, b2Manifold oldManifold)
        {
            
        }
    }
}