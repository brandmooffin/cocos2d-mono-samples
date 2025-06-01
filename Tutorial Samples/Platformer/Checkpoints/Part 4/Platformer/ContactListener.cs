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