using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace MERGEN_KAT1_GCSS
{
    public class Aras
    {
        private PictureBox[] pictureBoxes;

        // 5. hata biti değiştiğinde haber verecek event
        public event Action<bool> FifthErrorChanged;

        private bool? lastFifthError = null; // Başlangıçta bilinmiyor

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

            // 5. bit (error[4]) kontrolü
            if (error.Length > 4)
            {
                bool currentFifthError = error[4] == '1';

                if (lastFifthError == null || currentFifthError != lastFifthError)
                {
                    FifthErrorChanged?.Invoke(currentFifthError);
                    lastFifthError = currentFifthError;
                }
            }

            if (alarm)
            {
                SystemSounds.Exclamation.Play();
            }
        }
    }
}
