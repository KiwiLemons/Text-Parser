using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.VisualBasic;

namespace TextParser
{
    public partial class Form1 : Form
    {
        WebClient webClient = new WebClient();
        public Form1()
        {
            InitializeComponent();
        }
        //Form Load
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string V = webClient.DownloadString("https://pastebin.com/raw/Ky3810DF");
                if (V != "1.1")
                {
                    if (MessageBox.Show(String.Format("A new version is available!\nCurrent Version: 1.1\nNew Version: {0}\nWould you like to go to the download page?", V), "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start("https://github.com/KiwiLemons/Text-Parser/releases");
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("The remote name could not be resolved"))
                {
                    MessageBox.Show("Version check has failed!\nEither your internet is disconnected or servers are down, which is unlikely.","An error has occured",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }
        //Parse Button
        private void button1_Click(object sender, EventArgs e)
        {
            //Check if boxes are empty
            if (textBox1.Text == "" || textBox3.Text == "" || textBox4.Text == "")
                textBox5.Text = "Please do not leave the \"Left String\", \"Right String\", or the \"Input\" fields empty";
            else if (textBox1.Text == textBox3.Text)
                textBox5.Text = "The left and right string cannot match";
            else
            {
                if (!checkBox1.Checked)
                {
                    textBox2.Text = TextParser(textBox1.Text, textBox3.Text, textBox4.Text, Prefix.Text, Suffix.Text, checkBox1.Checked, checkBox2.Checked);
                }
                else
                {
                    Clipboard.SetText(TextParser(textBox1.Text, textBox3.Text, textBox4.Text, Prefix.Text, Suffix.Text, checkBox1.Checked, checkBox2.Checked));
                    textBox2.Text = "It has been copied to your Clipboard";
                }
            }
        }
        private string TextParser(string leftString, string rightString, string Input, string prefix, string suffix, bool recursive, bool numbers)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string extracted = "error";
            //Check if input contains left string
            if (!Input.Contains(leftString))
            {
                textBox5.Text = "The left string does not exist in this input";
                return extracted;
            }
            //Check if input contains left string
            else if (!Input.Contains(rightString))
            {
                textBox5.Text = "The right string does not exist in this input";
                return extracted;
            }
            try
            {
                //Remove everything before the first left string
                Input = Input.Substring(Input.IndexOf(leftString), Input.Length - Input.IndexOf(leftString));
            }
            catch (Exception ex)
            {
                //Extra left string check
                if (ex.Message.Contains("StartIndex cannot be less than zero"))
                {
                    textBox5.Text = "Left string does not exist in this input";
                    return extracted;
                }
                else
                    MessageBox.Show(ex.Message);
            }
            if (!recursive)
            {
                try
                {
                    //Do stuff
                    extracted = String.Format("{1}{0}{2}", Input.Substring(Input.IndexOf(leftString) + leftString.Length, Input.IndexOf(rightString) - Input.IndexOf(leftString) - leftString.Length),prefix,suffix);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    //Do something idk
                }
            }
            else if (recursive)
            {
                Regex counter = new Regex(String.Format("{0}.*{1}", leftString, rightString));
                int amount = counter.Matches(Input).Count;
                int i = 1, errors = 0;
                extracted = "";
                while (i - errors <= amount)
                {
                    try
                    {
                        if (numbers)
                        {
                            extracted += String.Format("#{0} {2}{1}{3}\n", i - errors, Input.Substring(Input.IndexOf(leftString) + leftString.Length, Input.IndexOf(rightString) - Input.IndexOf(leftString) - leftString.Length), prefix, suffix);
                        }
                        else
                        {
                            extracted += String.Format("{1}{0}{2}\n",Input.Substring(Input.IndexOf(leftString) + leftString.Length, Input.IndexOf(rightString) - Input.IndexOf(leftString) - leftString.Length), prefix, suffix);
                        }
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        ++errors;
                    }
                    Input = Input.Substring(Input.IndexOf(rightString) + rightString.Length);
                    ++i;
                }
            }
            stopwatch.Stop();
            textBox5.Text = String.Format("Time elapsed:  {0} ms", stopwatch.ElapsedMilliseconds.ToString());
            return extracted;
        }
        //Input from File button
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = ".txt files | *.txt";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = File.ReadAllText(fileDialog.FileName);
                textBox5.Text = String.Format("Imported file  {0}", fileDialog.FileName);
            }
        }
        //Example button
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "///";
            textBox3.Text = @"\\\";
            textBox4.Text = @"///find me\\\";
            Prefix.Text = "You can ";
            Suffix.Text = "!";
            textBox2.Text = TextParser(textBox1.Text, textBox3.Text, textBox4.Text,Prefix.Text,Suffix.Text, false,false);
        }
        //input from URL button
        private void button4_Click(object sender, EventArgs e)
        {
            string url = Interaction.InputBox("Enter the URL into the textbox", "URL Input", "");
            if (url != "")
            {
                try
                {
                    textBox4.Text = webClient.DownloadString(url);
                }
                catch (WebException ex)
                {
                    if (ex.Message.Contains("The remote name could not be resolved"))
                    {
                        MessageBox.Show("Either your internet connection is down or the web server is down","Connection Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(String.Format("An unexpected error has occured!\n{0}",ex.Message),"Error");
                    }
                }
            }
        }
        //Recusive checkbox state change
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Enabled = checkBox1.Checked;
        }
    }
}