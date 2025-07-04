using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
            if (error.Length != 6)
                throw new ArgumentException("error stringi sayısı eşleşmiyor!");

            bool alarm = false;

            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                if (error[i] == '0')
                {
                    pictureBoxes[i].BackColor = Color.Chartreuse; 
                }
                else if (error[i] == '1')
                {
                    pictureBoxes[i].BackColor = Color.Red;
                    alarm = true;
                }
            }

            if (alarm)
            {
                SystemSounds.Exclamation.Play();
            }
        }


    }
}
