using AndonModbus.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AndonServer.Models
{
    public class Part
    {
        public string id="", name = "";
        private BitmapFrame img = null;
        public string imgname = "";

        public Part(string id, string name, string img)
        {
            this.id = id;
            this.name = name;
            imgname = img;
            if (img != string.Empty)
            {
                getImg(img);
            }
        }

        private void getImg(string img)
        {
            Task.Run(() =>
            {
                try
                {
                    this.img = Field.web.getImage(img);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.StackTrace);
                }
            });
        }

        public BitmapFrame getImg()
        {
            return img;
        }
    }
}
