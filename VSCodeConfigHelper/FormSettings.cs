﻿// Copyright (C) 2020 Guyutongxue
// 
// This file is part of VSCodeConfigHelper.
// 
// VSCodeConfigHelper is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// VSCodeConfigHelper is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with VSCodeConfigHelper.  If not, see<http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VSCodeConfigHelper
{
    public partial class FormSettings : Form
    {

        #region Add Shield Icon

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        ///     Enables the elevated shield icon on the given button control
        /// </summary>
        /// <param name="ThisButton">
        ///     Button control to enable the elevated shield icon on.
        /// </param>
        private void EnableElevateIcon_BCM_SETSHIELD(Button ThisButton)
        {
            // Input validation, validate that ThisControl is not null
            if (ThisButton == null) return;

            // Define BCM_SETSHIELD locally, declared originally in Commctrl.h
            uint BCM_SETSHIELD = 0x0000160C;

            // Set button style to the system style
            ThisButton.FlatStyle = FlatStyle.System;

            // Send the BCM_SETSHIELD message to the button control
            SendMessage(new HandleRef(ThisButton, ThisButton.Handle), BCM_SETSHIELD, new IntPtr(0), new IntPtr(1));
        }
        #endregion

        public FormSettings()
        {
            InitializeComponent();
        }

        private void buttonSaveArgs_Click(object sender, EventArgs e)
        {

            GenerateArgs();
            ShowArgs();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            if (DateTime.Now.Date > new DateTime(2024, 10, 1)) radioButtonPKU.Enabled = false;
            if (Form1.isMinGWPku) radioButtonPKU.Checked = true;
            else radioButtonOffical.Checked = true;
            if (Form1.IsAdministrator)
            {
                labelAuth.Width = 409;
                labelAuth.Text = "当前权限：系统管理员" + Environment.NewLine;
                labelAuth.Text += "您在此工具进行的操作（包括安装、设置环境变量和启动等）" +
                    "将适用于所有用户，请谨慎操作。" + Environment.NewLine;
                labelAuth.Text += "若要使用普通用户权限，请重新使用非管理员权限运行此程序。";
                buttonAuth.Visible = false;

            }
            else
            {
                labelAuth.Text = "当前权限：普通用户" + Environment.NewLine;
                labelAuth.Text += "您在此工具进行的操作（包括安装、设置环境变量和启动等）" +
                    "将仅适用于此账户。" + Environment.NewLine;
                labelAuth.Text += "若要使用系统管理员权限，请点击右侧按钮。";
                EnableElevateIcon_BCM_SETSHIELD(buttonAuth);
            }

            ShowArgs();
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.isMinGWPku = radioButtonPKU.Checked;
        }

        private void buttonAuth_Click(object sender, EventArgs e)
        {
            try
            {
                var exeName = Process.GetCurrentProcess().MainModule.FileName;
                ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                startInfo.Verb = "runas";
                // prevent pop-up message box
                Form1.isSuccess = true;
                Process.Start(startInfo);
                Application.Exit();
            }
            catch (Win32Exception)
            {
                // Do nothing.
                // If user cancel the operation by UAC, Process.Start will
                // throw an exception. Just ignore it.
            }
        }

        private void GenerateArgs()
        {
            string text = textBoxArgs.Text.Trim();
            string[] argtext = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Form1.args = new JArray(argtext);
        }

        private void ShowArgs()
        {
            StringBuilder text = new StringBuilder();
            foreach (object i in Form1.args)
            {
                text.AppendLine(i.ToString());
            }
            textBoxArgs.Text = text.ToString().Trim();
        }

        private void SetDefaultArgs()
        {
            groupBoxArg.Text = "配置编译参数";
            Form1.args = new JArray {
                "-g",
                Form1.isCpp ? "-std=c++17" : "-std=c17",
                "\"${file}\"",
                "-o",
                "\"${fileDirname}\\${fileBasenameNoExtension}.exe\""
            };
            ShowArgs();
        }

        private void buttonArgDefault_Click(object sender, EventArgs e)
        {
            SetDefaultArgs();
        }

        private void linkLabelLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.gnu.org/licenses/gpl-2.0.html");
        }

        private void pictureGitHub_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Guyutongxue/VSCodeConfigHelper");
        }
    }
}
