using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;

namespace FTPprojesi
{
    public partial class Form1 : Form
    {

        private void Form1_Load(object sender, EventArgs e)
        { }

        public string Username;
        public string Filename;
        public string Fullname;
        public string Server;
        public string Password;
        public string path;
        public string localdest;

        public Form1()
        {
            InitializeComponent();
            checkdown.Checked = true;
            checkup.Enabled = false;
            btnIndir.Text = @"İndir";
            label5.Enabled = true;
            textBox4.Enabled = true;
            checkup.Enabled = false;
            label4.Text = @"İndiriliyor %0";

        }


        private void label1_Click(object sender, EventArgs e) { }


        //Background
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (checkdown.Checked == true)
            {
                label4.Text = $"İndiriliyor %{e.ProgressPercentage}";
                label4.Update();
                progressBar1.Value = e.ProgressPercentage;
                progressBar1.Update();
            }

            if (checkup.Checked == true)
            {
                label4.Text = $"Yükleniyor %{e.ProgressPercentage}";
                label4.Update();
                progressBar1.Value = e.ProgressPercentage;
                progressBar1.Update();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (checkdown.Checked == true)
            {
                label4.Text = "İndirme Tamamlandı!";
                MessageBox.Show("İndirme Tamamlandı!");
            }

            if (checkup.Checked == true)
            {
                label4.Text = "Yükleme Tamamlandı!";
                MessageBox.Show("Yükleme Tamamlandı!");
            }

        }

        private void checkdown_CheckedChanged(object sender, EventArgs e)
        {
            if (checkdown.Checked == false)
            {
                checkup.Enabled = true;
                checkup.Checked = true;
                btnIndir.Text = @"Yükle";
                label5.Enabled = false;
                textBox4.Enabled = false;
                checkdown.Enabled = false;
                label4.Text = @"Yükleniyor %0";
            }
        }

        private void checkup_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkup.Checked == false)
            {
                checkdown.Enabled = true;
                checkdown.Checked = true;
                btnIndir.Text = @"İndir";
                label5.Enabled = true;
                textBox4.Enabled = true;
                checkup.Enabled = false;
                label4.Text = @"İndiriliyor %0";
            }
        }

        private void btnIndir_Click_1(object sender, EventArgs e)
        {
            if (checkup.Checked == true)
            {

                using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true, ValidateNames = true, Filter = "All Files|*.*" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        FileInfo fi = new FileInfo(ofd.FileName);
                        Username = textBox1.Text;
                        Password = textBox2.Text;
                        Server = textBox3.Text;
                        Filename = fi.Name;
                        Fullname = fi.FullName;
                    }
                }
            }


            if (checkdown.Checked == true)
            {
                Username = textBox1.Text;
                Password = textBox2.Text;
                Server = textBox3.Text;
                Filename = textBox4.Text;
                path = @"C:\Users\erkoc\Documents\";
                localdest = path + @"" + Filename;
                Fullname = Server + @"/" + Filename;
            }
            
            backgroundWorker1.RunWorkerAsync();  
            
        }

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {

            if (checkdown.Checked == true)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", Server, Filename)));

                request.Credentials = new NetworkCredential(Username, Password);
                request.Method = WebRequestMethods.Ftp.DownloadFile;  //Download metotu


                // Kaynaktan Verileri alma
                FtpWebRequest request1 = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", Server, Filename)));
                request1.Credentials = new NetworkCredential(Username, Password);
                request1.Method = WebRequestMethods.Ftp.GetFileSize;  //GetFileze Metotu
                FtpWebResponse response = (FtpWebResponse)request1.GetResponse();
                double total = response.ContentLength;
                response.Close();

                FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", Server, Filename)));
                request2.Credentials = new NetworkCredential(Username, Password);
                request2.Method = WebRequestMethods.Ftp.GetDateTimestamp; //GetTimestamp Metotu
                FtpWebResponse response2 = (FtpWebResponse)request2.GetResponse();
                DateTime modify = response2.LastModified;
                response2.Close();


                Stream ftpstream = request.GetResponse().GetResponseStream();
                FileStream fs = new FileStream(localdest, FileMode.Create);

                // İlerlemeyi hesaplama ve gösterme yöntemi.
                byte[] buffer = new byte[1024];
                int byteRead = 0;
                double read = 0;
                do
                {
                    byteRead = ftpstream.Read(buffer, 0, 1024);
                    fs.Write(buffer, 0, byteRead);
                    read += (double)byteRead;
                    double percentage = read / total * 100;
                    backgroundWorker1.ReportProgress((int)percentage);
                }
                while (byteRead != 0);
                ftpstream.Close();
                fs.Close();


            }
            else
            {

                //Upload Method.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", Server, Filename)));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(Username, Password);
                Stream ftpstream = request.GetRequestStream();
                FileStream fs = File.OpenRead(Fullname);

                // İlerlemeyi hesaplama ve gösterme yöntemi.
                byte[] buffer = new byte[1024];
                double total = (double)fs.Length;
                int byteRead = 0;
                double read = 0;
                do
                {
                    byteRead = fs.Read(buffer, 0, 1024);
                    ftpstream.Write(buffer, 0, byteRead);
                    read += (double)byteRead;
                    double percentage = read / total * 100;
                    backgroundWorker1.ReportProgress((int)percentage);
                }
                while (byteRead != 0);
                fs.Close();
                ftpstream.Close();

            }
        }

      
    }
}
