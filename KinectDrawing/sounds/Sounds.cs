using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Sounds
{
    class Sounds
    {
        private System.Media.SoundPlayer player;
        private string PathProject;

        public Sounds()
        {
            this.PathProject = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            this.player = new System.Media.SoundPlayer();
        }

        public void playCorrectVoice()
        {
            this.player.SoundLocation = this.PathProject + @"\sounds\goodjob.wav";
            this.player.Play();
        }

        public void playNotCorrectVoice()
        {
            this.player.SoundLocation = this.PathProject + @"\sounds\sorrynottheanswer.wav";
            this.player.Play();
        }

        public void playOpeningSound()
        {
            

            this.player.SoundLocation = this.PathProject + @"\sounds\opening.wav";
            this.player.Play();
        }

        public void playGoodLuckSound()
        {
            this.player.SoundLocation = this.PathProject + @"\sounds\goodluck.wav";
            this.player.Play();
        }

        public void playYoureGreatSound()
        {
            this.player.SoundLocation = this.PathProject + @"\sounds\youaregreet.wav";
            this.player.Play();
        }

        public void playYoureWonderfulSound()
        {
            this.player.SoundLocation = this.PathProject + @"\sounds\youarewonderful.wav";
            this.player.Play();
        }

        public void playNextLevelSound()
        {
            this.player.SoundLocation = this.PathProject + @"\sounds\nextlevel.wav";
            this.player.Play();
        }

        public void playAnotherOneSound()
        {
            this.player.SoundLocation = this.PathProject + @"\sounds\tryanoterone.wav";
            this.player.Play();
        }

        public void playCanDoItSound()
        {
            this.player.SoundLocation = this.PathProject + @"\sounds\youcandoit.wav";
            this.player.Play();
        }
    }
}
