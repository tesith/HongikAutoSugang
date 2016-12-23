using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using MSHTML;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace Sugang
{
    public partial class Form1 : Form
    {
        Thread refreshThread;
        volatile bool shouldStop = true;

        public Form1()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            refreshThread = new Thread(this.AutoRefresh);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!refreshThread.IsAlive)
            {
                shouldStop = false;
                button1.Text = "중지";
                refreshThread.Start();
            }
            else
            {
                button1.Text = "시작";
                label1.Text = "대기";
                shouldStop = true;
                refreshThread.Join();
                refreshThread = new Thread(this.AutoRefresh);
            }
        }

        private void AutoRefresh()
        {
            while (!shouldStop)
            {
                webBrowser1.Refresh(WebBrowserRefreshOption.Completely);
                Thread.Sleep(1000);
            }

            Thread.Sleep(10);
        }

        private bool checkDocument(string haksuNum)
        {
            string webDoc = webBrowser1.DocumentText;

            int haksuIndex = webDoc.IndexOf(haksuNum);

            string findWord = webDoc.Substring(haksuIndex);

            for (int i = 0; i < 3; ++i)
            {
                findWord = findWord.Substring(1);
                findWord = findWord.Substring(findWord.IndexOf("<TD"));
            }

            findWord = findWord.Substring(4);

            int endStringIndex = findWord.IndexOf("</TD>");

            string information = findWord.Substring(0, endStringIndex);
            int slashIndex = information.IndexOf("/");
            string numberOfStudent = information.Substring(0, slashIndex);
            string limitOfStudent = information.Substring(slashIndex + 1);

            int num = int.Parse(numberOfStudent);
            int limit = int.Parse(limitOfStudent);

            if (num < limit)
                return true;

            return false;
        }

        private void doWork(object sender, WebBrowserProgressChangedEventArgs e)
        {
            if (e.CurrentProgress == -1)
            {
                if (!shouldStop)
                {
                    string haksuNum = textBox1.Text;
                    label1.Text = "진행중";

                    if (checkDocument(haksuNum))
                    {
                        SystemSounds.Beep.Play();
                        label1.Text = "찾음";
                        button1.Text = "시작";
                        shouldStop = true;
                        refreshThread.Join();
                        MessageBox.Show("빈 자리가 열렸습니다!!");
                        refreshThread = new Thread(this.AutoRefresh);
                    }
                }
            }
        }
    }
}
