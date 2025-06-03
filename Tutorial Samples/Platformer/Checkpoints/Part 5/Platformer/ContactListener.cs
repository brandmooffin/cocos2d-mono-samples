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