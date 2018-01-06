using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    textBox2.Text = "It has been copied to your Clipboard";
                    Clipboard.SetText(TextParser(textBox1.Text, textBox3.Text, textBox4.Text, Prefix.Text, Suffix.Text, checkBox1.Checked, checkBox2.Checked));
                }
                
            }
        }
        private string TextParser(string leftString, string rightString, string Input, string prefix, string suffix, bool recursive,bool numbers)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string extracted = "error";
            if (!recursive)
            {
                try
                {
                    extracted = String.Format("{1}{0}{2}", Input.Substring(Input.IndexOf(leftString) + leftString.Length, Input.IndexOf(rightString) - Input.IndexOf(leftString) - leftString.Length),prefix,suffix);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    textBox5.Text = ex.Message;
                }
            }
            else if (recursive)
            {
                Regex counter = new Regex(String.Format("{0}.*{1}", leftString, rightString));
                int amount = counter.Matches(Input).Count;
                int i = 1, errors = 0;
                extracted = "";
                while (i <= amount)
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
                        //textBox5.Text = ex.Message;
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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