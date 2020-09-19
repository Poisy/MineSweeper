using Plugin.SimpleAudioPlayer;

namespace MineSweeper.Models
{
    public class AudioPlayer
    {
        private ISimpleAudioPlayer Player { get; set; }

        public AudioPlayer()
        {
            Player = CrossSimpleAudioPlayer.Current;
        }

        public void Load(Sounds sound)
        {
            if (Player.IsPlaying) return;

            string path = "";

            switch (sound)
            {
                case Sounds.CellClick:
                    path = "cell_click.mp3";
                    break;
                case Sounds.CellHold:
                    path = "cell_hold.mp3";
                    break;
                case Sounds.Lose:
                    path = "lose.mp3";
                    break;
                case Sounds.SpecialLose:
                    path = "special_lose.mp3";
                    break;
                case Sounds.Victory:
                    path = "victory.mp3";
                    break;
            }

            Player.Load(path);
        }

        public void Play()
        {
            Player.Play();
        }

        public enum Sounds
        {
            CellClick, CellHold, Lose, Victory, SpecialLose
        }
    }
}
