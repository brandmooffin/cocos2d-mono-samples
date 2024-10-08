﻿using Cocos2D;
using CocosDenshion;

namespace AngryNinjas.Shared.Custom
{
    public class GameSounds : CCNode
    {
        bool soundFXTurnedOff;
        bool voiceFXTurnedOff;
        bool ambientFXTurnedOff; //ambient or background music
        string delayedSoundName;

        AmbientFXSounds musicChoice;

        const string soundsFolder = "Sounds";
        static GameSounds sharedGameSounds;

        private GameSounds()
        {
            voiceFXTurnedOff = GameData.SharedData.AreVoiceFXMuted;
            soundFXTurnedOff = GameData.SharedData.AreSoundFXMuted;
            ambientFXTurnedOff = GameData.SharedData.AreAmbientFXMuted;
        }

        /// <summary>
        /// returns a shared instance of the GameData
        /// </summary>
        /// <value> </value>
        public static GameSounds SharedGameSounds
        {
            get
            {
                if (sharedGameSounds == null)
                    sharedGameSounds = new GameSounds();
                return sharedGameSounds;
            }
        }

        public void PreloadSounds()
        {
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("grunt1"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("grunt2"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("grunt3"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("grunt4"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("grunt5"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("grunt6"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("grunt7"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("grunt8"));

            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("break1"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("break2"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("break3"));

            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("impact1"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("impact2"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("impact3"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("impact4"));
            CCSimpleAudioEngine.SharedEngine.PreloadEffect(FormatSoundFilePath("impact5"));
        }

        string FormatSoundFilePath(string sound)
        {
            string sndfile = System.IO.Path.Combine(soundsFolder, sound);
            if (sndfile.IndexOf(".mp3") > -1)
                sndfile = sndfile.Substring(0, sndfile.IndexOf(".mp3"));
            else if (sndfile.IndexOf(".wav") > -1)
                sndfile = sndfile.Substring(0, sndfile.IndexOf(".wav"));
            return (sndfile);
        }

        public bool AreSoundFXMuted
        {
            get { return soundFXTurnedOff; }
            set { soundFXTurnedOff = value; }
        }

        public bool AreVoiceFXMuted
        {
            get { return voiceFXTurnedOff; }
            set { voiceFXTurnedOff = value; }
        }

        public void PlaySoundFX(string fileToPlay)
        {
            if (!soundFXTurnedOff)
                CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath(fileToPlay));
        }

        public void PlayVoiceSoundFX(string fileToPlay)
        {
            if (!voiceFXTurnedOff)
                CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath(fileToPlay));
        }

        public void PlaySoundFXWithDelay(string fileToPlay, float theDelay)
        {
            if (!soundFXTurnedOff)
            {
                delayedSoundName = fileToPlay;
                ScheduleOnce(PlayThisAfterDelay, theDelay);
            }
        }

        public void PlayVoiceSoundFXWithDelay(string fileToPlay, float theDelay)
        {
            if (!voiceFXTurnedOff)
            {
                delayedSoundName = fileToPlay;
                ScheduleOnce(PlayThisAfterDelay, theDelay);
            }
        }

        public void PlayThisAfterDelay(float delay)
        {
            CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath(delayedSoundName));
        }

        public void IntroTag()
        {
            if (!soundFXTurnedOff)
                CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("gong"));
        }

        public void PlayStackImpactSound()
        {
            if (!soundFXTurnedOff)
            {
                int randomNum = CCRandom.Next(0, 4); //0 to 4
                switch (randomNum)
                {
                    case 0:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("impact1"));
                        break;
                    case 1:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("impact2"));
                        break;
                    case 2:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("impact3"));
                        break;
                    case 3:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("impact4"));
                        break;
                    case 4:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("impact5"));
                        break;
                }
            }
        }

        public void PlayBreakSound()
        {
            if (!soundFXTurnedOff)
            {
                int randomNum = CCRandom.Next(0, 2); //0 to 2

                switch (randomNum)
                {
                    case 0:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("break1"));
                        break;
                    case 1:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("break2"));
                        break;
                    case 2:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("break3"));
                        break;
                }
            }
        }

        public void ReleaseSlingSounds()
        {
            if (!soundFXTurnedOff)
                CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("whoosh"));

            if (!voiceFXTurnedOff)
            {
                int randomNum = CCRandom.Next(0, 7); //0 to 7

                switch (randomNum)
                {
                    case 0:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("grunt1"));
                        break;
                    case 1:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("grunt2"));
                        break;
                    case 2:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("grunt3"));
                        break;
                    case 3:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("grunt4"));
                        break;
                    case 4:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("grunt5"));
                        break;
                    case 5:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("grunt6"));
                        break;
                    case 6:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("grunt7"));
                        break;
                    case 7:
                        CCSimpleAudioEngine.SharedEngine.PlayEffect(FormatSoundFilePath("grunt8"));
                        break;
                }
            }
        }

        public void PlayBackgroundMusic(AmbientFXSounds track)
        {
            //or AMBIENT SOUND FX
            musicChoice = track;
            CCSimpleAudioEngine.SharedEngine.StopBackgroundMusic();

            if (!ambientFXTurnedOff)
            {
                if (musicChoice == AmbientFXSounds.Frogs)
                    CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic(FormatSoundFilePath("birds"), true);
                else if (musicChoice == AmbientFXSounds.Insects)
                    CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic(FormatSoundFilePath("frogs"), true);
            }
        }

        public void StopBackgroundMusic()
        {
            CCSimpleAudioEngine.SharedEngine.StopBackgroundMusic();
            ambientFXTurnedOff = true;
        }

        public void RestartBackgroundMusic()
        {
            ambientFXTurnedOff = false;
            PlayBackgroundMusic(musicChoice);
        }
    }
}
