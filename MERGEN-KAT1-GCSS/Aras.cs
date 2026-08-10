using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class Aras
    {
        private PictureBox[] pictureBoxes;

        
        

        

        public Aras(params PictureBox[] pbs)
        {
            pictureBoxes = pbs;
        }

        public void Update(string error)
        {
            bool alarm = false;

            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                if (i < error.Length && error[i] == '1')
                {
                    pictureBoxes[i].BackColor = Color.Red;
                    alarm = true;
                }
                else
                {
                    pictureBoxes[i].BackColor = Color.Chartreuse;
                }
            }

            
            

            if (alarm)
            {
                SystemSounds.Exclamation.Play();
            }
        }
    }
}
