using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDrawing.Sounds
{
    class Sounds
    {
        private System.Media.SoundPlayer player;

        public Sounds()
        {
            this.player = new System.Media.SoundPlayer();
        }

        public void playCorrectVoice()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\goodjob.wav";
            this.player.Play();
        }

        public void playNotCorrectVoice()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\sorrynottheanswer.wav";
            this.player.Play();
        }

        public void playOpeningSound()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\opening.wav";
            this.player.Play();
        }

        public void playGoodLuckSound()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\goodluck.wav";
            this.player.Play();
        }

        public void playYoureGreatSound()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\youaregreet.wav";
            this.player.Play();
        }

        public void playYoureWonderfulSound()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\youarewonderful.wav";
            this.player.Play();
        }

        public void playNextLevelSound()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\nextlevel.wav";
            this.player.Play();
        }

        public void playAnotherOneSound()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\tryanoterone.wav";
            this.player.Play();
        }

        public void playCanDoItSound()
        {
            this.player.SoundLocation = @"C:\Users\admin\Desktop\_ABCaaS\KinectDrawing\sounds\youcandoit.wav";
            this.player.Play();
        }
    }
}
