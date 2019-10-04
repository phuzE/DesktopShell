﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DesktopShell
{
    public partial class ConfigForm : Form
    {
        #region Declarations
        private Regex hexCheck = new Regex("^(#)?([a-fA-F0-9]){6}$");
        private string checkString;
        #endregion

        #region ConfigForm Constructor / Startup
        public ConfigForm() { InitializeComponent(); }
        #endregion

        #region Form Event Handlers
        private void ConfigForm_Load(object sender, EventArgs e)
        {
            // create checkboxes for each monitor
            List<System.Windows.Forms.CheckBox> screenCheckBoxList = new List<System.Windows.Forms.CheckBox>();
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                System.Windows.Forms.CheckBox tempBox = new System.Windows.Forms.CheckBox();
                tempBox.AutoSize = true;
                tempBox.CheckAlign = ContentAlignment.MiddleRight;
                if (i < Properties.Settings.multiscreenEnabled.Count)
                {
                    if (Properties.Settings.multiscreenEnabled[i])
                        tempBox.CheckState = System.Windows.Forms.CheckState.Checked;
                    else
                        tempBox.CheckState = System.Windows.Forms.CheckState.Unchecked;
                }
                tempBox.Location = new System.Drawing.Point((130 + (50*i)), 76);
                tempBox.Name = ("multiScreenCheckbox" + (i+1));
                tempBox.Size = new System.Drawing.Size(50, 17);
                tempBox.TabIndex = 3 + i;
                tempBox.Text = ("" + (i+1) + ":");
                tempBox.UseVisualStyleBackColor = true;
                tempBox.Click += tempBox_Click;

                screenCheckBoxList.Add(tempBox);
            }
            foreach (System.Windows.Forms.CheckBox c in screenCheckBoxList)
            {
                this.interfaceBox.Controls.Add(c);
            }

            //checkbox initialization
            if (Properties.Settings.hourlyChimeChecked) this.hourlyChimeCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            else this.hourlyChimeCheckbox.CheckState = System.Windows.Forms.CheckState.Unchecked;
            //foreground color initialization
            this.textColorInputBox.Text = (System.Drawing.ColorTranslator.ToHtml(Properties.Settings.foregroundColor)).ToString();
            //background color initialization
            this.backgroundColorInputBox.Text = (System.Drawing.ColorTranslator.ToHtml(Properties.Settings.backgroundColor)).ToString();
            //set initial position
            this.Location = Cursor.Position;
        }

        void tempBox_Click(object sender, EventArgs e)
        {
            string whichCheckBox = ((System.Windows.Forms.CheckBox)sender).Name.Replace("multiScreenCheckbox", "");
            int checkBoxIdx = (Convert.ToInt32(whichCheckBox) - 1);
            Properties.Settings.multiscreenEnabled[checkBoxIdx] = !Properties.Settings.multiscreenEnabled[checkBoxIdx];

            GlobalVar.initDropDownRects(GlobalVar.shellInstance);
        }
        private void ConfigForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (GlobalVar.screenSelectorInstance != null) GlobalVar.screenSelectorInstance.Close();
        }
        private void checkKeys_textColor(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; //prevents beep
                checkString = this.textColorInputBox.Text;
                if (hexCheck.IsMatch(checkString))
                {
                    MessageBox.Show("Font Color Changed");
                    //change setting
                    Properties.Settings.foregroundColor = System.Drawing.ColorTranslator.FromHtml(checkString);
                    //change in program
                    GlobalVar.shellInstance.changeFontColor();
                    //write to file
                    Properties.Settings.writeSettings();
                }
                else MessageBox.Show("Incorrect Format: (#123456)");
            }
        }
        private void checkKeys_backgroundColor(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; //prevents beep
                checkString = this.backgroundColorInputBox.Text;
                if (hexCheck.IsMatch(checkString))
                {
                    MessageBox.Show("Background Color Changed");
                    //Changing in settings
                    Properties.Settings.backgroundColor = System.Drawing.ColorTranslator.FromHtml(checkString);
                    //Changing in program
                    GlobalVar.shellInstance.changeBackgroundColor();
                    //Write to file
                    Properties.Settings.writeSettings();
                }
                else MessageBox.Show("Incorrect Format: (#123456)");
            }
        }
        private void hourlyChimeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            //Toggle / save setting
            GlobalVar.hourlyChime.Enabled = Properties.Settings.hourlyChimeChecked = !GlobalVar.hourlyChime.Enabled;
            //Write to file
            Properties.Settings.writeSettings();
        }
        private void screenSelectorButton_Click(object sender, System.EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ScreenSelectorProc));
            t.Start();
        }
        private void ColorWheel_Click(object sender, System.EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ColorWheelProc));
            t.Start();
        }
        private void BackColorWheel_Click(object sender, EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ColorWheelProc));
            t.Start();
        }

        private void ForeColorWheel_Click(object sender, EventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ColorWheelProc));
            t.Start();
        }
        #endregion

        public static void ScreenSelectorProc() { Application.Run(GlobalVar.screenSelectorInstance = new DesktopShell.Forms.ScreenSelectorForm()); }
        public static void ColorWheelProc() { Application.Run(GlobalVar.colorWheelInstance = new DesktopShell.Forms.ColorWheel()); }     
    }
}
