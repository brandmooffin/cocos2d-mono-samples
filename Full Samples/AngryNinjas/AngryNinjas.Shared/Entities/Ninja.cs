﻿using Box2D.Collision.Shapes;
using Box2D.Dynamics;
using Cocos2D;
using System;
using AngryNinjas.Shared.Custom;

namespace AngryNinjas.Shared.Entities
{
    public class Ninja : BodyNode
    {

        b2World theWorld;
        string baseImageName;
        string spriteImageName;
        CCPoint initialLocation;
        bool onGround;
        short counter;

        public Ninja(b2World world, CCPoint location, string baseFileName)
        {
            InitWithWorld(world, location, baseFileName);
        }

        private void InitWithWorld(b2World world, CCPoint location, string baseFileName)
        {

            theWorld = world;
            initialLocation = location;
            baseImageName = baseFileName;


            //later we use initialLocation.x 

            CreateNinja();
        }

        public bool IsOnGround { get { return onGround; } }

        void CreateNinja()
        {


            spriteImageName = String.Format("{0}_standing", baseImageName);

            onGround = false;

            // Define the dynamic body.
            var bodyDef = new b2BodyDef();
            bodyDef.type = b2BodyType.b2_staticBody; //or you could use b2DynamicBody to start the ninja as dynamic

            bodyDef.position.Set(initialLocation.X / Constants.PTM_RATIO, initialLocation.Y / Constants.PTM_RATIO);

            var shape = new b2CircleShape();
            var radiusInMeters = (40 / Constants.PTM_RATIO) * 0.5f; //increase or decrease 40 for a different circle size definition

            shape.Radius = radiusInMeters;


            // Define the dynamic body fixture.
            var fixtureDef = new b2FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = 1.0f;
            fixtureDef.friction = 1.0f;
            fixtureDef.restitution = 0.1f;

            CreateBodyWithSpriteAndFixture(theWorld, bodyDef, fixtureDef, spriteImageName);

        }


        public void SpriteInSlingState()
        {

            Unschedule(Blink);
            Unschedule(OpenEyes);

            onGround = false;

            sprite.Texture = new CCSprite(String.Format("{0}_in_sling", baseImageName)).Texture;

        }


        public void SpriteInStandingState()
        {

            sprite.Texture = new CCSprite(String.Format("{0}_standing", baseImageName)).Texture;

            var blinkInterval = CCRandom.Next(3, 8); // range 3 to 8

            Schedule(Blink, blinkInterval);
        }

        public void SpriteInPulledBackSlingState()
        {

            Unschedule(Blink);
            Unschedule(OpenEyes);
            sprite.Texture = new CCSprite(String.Format("{0}_roll", baseImageName)).Texture;


        }

        public void SpriteInGroundState()
        {

            onGround = true;
            sprite.Texture = new CCSprite(String.Format("{0}_on_ground", baseImageName)).Texture;


        }

        public void SpriteInRollStateWithAnimationFirst()
        {

            if (!onGround)
            {

                counter = 0;
                Schedule(RollAnimation, 1.0f / 60.0f);
            }

        }

        void RollAnimation(float delta)
        {

            counter++;

            if (counter <= 3)
            {

                sprite.Texture = new CCSprite(String.Format("{0}_roll000{1}", baseImageName, counter)).Texture;

            }
            else
            {

                if (!onGround)
                {

                    sprite.Texture = new CCSprite(String.Format("{0}_roll", baseImageName)).Texture;

                }
                else
                {

                    sprite.Texture = new CCSprite(String.Format("{0}_on_ground", baseImageName)).Texture;

                }

                Unschedule(RollAnimation);

                counter = 0;
            }

        }

        public void SpriteInAirState()
        {


            onGround = false;

            sprite.Texture = new CCSprite(String.Format("{0}_air", baseImageName)).Texture;

        }

        void Blink(float delta)
        {

            sprite.Texture = new CCSprite(String.Format("{0}_blink", baseImageName)).Texture;

            Unschedule(Blink);
            Schedule(OpenEyes, 0.5f);
        }

        void OpenEyes(float delta)
        {

            sprite.Texture = new CCSprite(String.Format("{0}_standing", baseImageName)).Texture;

            Unschedule(OpenEyes);

            int blinkInterval = CCRandom.Next(3, 8);//   random.Next(3,8); // range 3 to 8
            Schedule(Blink, blinkInterval);
        }

    }
}
