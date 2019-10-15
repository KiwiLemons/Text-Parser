using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.VisualBasic;

namespace TextParser
{
    public partial class Main : Form
    {
        WebClient webClient = new WebClient();
        public Main()
        {
            InitializeComponent();
        }
        //Form Load
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox4.MaxLength = int.MaxValue;
        }
        //Parse Button
        private void button1_Click(object sender, EventArgs e)
        {
            //Check if boxes are empty
            if (textBox1.Text == "" || textBox3.Text == "" || textBox4.Text == "")
                textBox5.Text = "Please do not leave the \"Left String\", \"Right String\", or the \"Input\" fields empty";
            else if (textBox1.Text == textBox3.Text)
                textBox5.Text = "The left and right strings cannot match";
            else
            {
                textBox2.Text = TextParser(textBox1.Text, textBox3.Text, textBox4.Text, Prefix.Text, Suffix.Text, checkBox1.Checked, checkBox2.Checked);
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
                    //OG method
                    //extracted = String.Format("{1}{0}{2}", Input.Substring(Input.IndexOf(leftString) + leftString.Length, Input.IndexOf(rightString) - Input.IndexOf(leftString) - leftString.Length), prefix, suffix);
                    //Maybe we could add an option to change the regex to it will match even when the group is empty with (.*)
                    extracted = $"{prefix}{Regex.Match(Input, $"{Regex.Escape(leftString)}(.+?){Regex.Escape(rightString)}").Groups[1]}{suffix}";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    //Do something idk
                    MessageBox.Show(ex.Message);
                }
            }
            else if (recursive)
            {
                extracted = "";
                MatchCollection Matches = Regex.Matches(Input, $"{Regex.Escape(leftString)}(.+?){Regex.Escape(rightString)}");
                for (int i = 0; i < Matches.Count; i++)
                {
                    //would be cool to have a number format option (1. , #1 , and others if i can think of any) Make a method that returns a string
                    extracted += $"{(numbers ? $"#{i + 1}" : "")} {prefix}{Matches[i].Groups[1]}{suffix}{(i == Matches.Count - 1 ? "" : "\n")}";
                }
                #region OG recursive method
                /* This method isnt used anymore but i just like to keep it
                Regex counter = new Regex(String.Format("{0}.*{1}", Regex.Escape(leftString), Regex.Escape(rightString)));
                int amount = counter.Matches(Input).Count;
                int i = 1, errors = 0;
                extracted = "";
                while (i - errors <= amount)
                {
                    try
                    {
                        if (numbers)
                        {
                            extracted += String.Format("#{0} {2}{1}{3}\n", i, Input.Substring(Input.IndexOf(leftString) + leftString.Length, Input.IndexOf(rightString) - Input.IndexOf(leftString) - leftString.Length), prefix, suffix);
                        }
                        else
                        {
                            extracted += String.Format("{1}{0}{2}\n", Input.Substring(Input.IndexOf(leftString) + leftString.Length, Input.IndexOf(rightString) - Input.IndexOf(leftString) - leftString.Length), prefix, suffix);
                        }
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        --i;
                    }
                    Input = Input.Substring(Input.IndexOf(rightString) + rightString.Length);
                    ++i;
                }
                */
                #endregion
                //if extracted contains more than one \n
                if (Regex.Matches(extracted, @"\n").Count > 1)
                {
                    Clipboard.SetText(extracted);
                    extracted = "The text has been copied to your clipboard!";
                }
            }
            stopwatch.Stop();
            textBox5.Text = String.Format("Time elapsed:  ~{0} ms", stopwatch.ElapsedMilliseconds.ToString());
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
            //Check is input boxes are emmpty
            if (textBox1.Text == "" && textBox3.Text == "" && textBox4.Text == "" && Prefix.Text == "" && Suffix.Text == "")
            {
                textBox1.Text = "///";
                textBox3.Text = @"\\\";
                textBox4.Text = @"///find me\\\";
                Prefix.Text = "You can ";
                Suffix.Text = "!";
                textBox2.Text = TextParser(textBox1.Text, textBox3.Text, textBox4.Text, Prefix.Text, Suffix.Text, checkBox1.Checked, checkBox2.Checked);
            }
            else if (MessageBox.Show("The input text boxes are not empty and will be replaced if you want to see the example.\nDo you wish to proceed?", "Inputs not empty", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                textBox1.Text = "///";
                textBox3.Text = @"\\\";
                textBox4.Text = @"///find me\\\";
                Prefix.Text = "You can ";
                Suffix.Text = "!";
                textBox2.Text = TextParser(textBox1.Text, textBox3.Text, textBox4.Text, Prefix.Text, Suffix.Text, checkBox1.Checked, checkBox2.Checked);
            }
        }

        //input from URL button
        private void button4_Click(object sender, EventArgs e)
        {
            string[] urls = Interaction.InputBox("Enter the URL into the textbox", "URL Input", "").Split(',');
            //Just put a valid fucking url so I don't have to check it
            if (urls.Length != 0)
            {
                foreach (string url in urls)
                {
                    try
                    {
                        textBox4.Text += $"{webClient.DownloadString(url)}\n";
                    }
                    catch (WebException ex)
                    {
                        if (ex.Message.Contains("The remote name could not be resolved"))
                        {
                            MessageBox.Show("Either your internet connection is down or the web server is down", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show(String.Format("An unexpected error has occured!\n{0}", ex.Message), "Error");
                        }
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