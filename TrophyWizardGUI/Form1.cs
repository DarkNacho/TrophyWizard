using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TrophyParser;
using TrophyParser.PS3;
using TrophyParser.Vita;

namespace PS3TrophyIsGood
{
    public partial class Form1 : Form {
        IUnlocker unlocker;
        string path;
        DateTimePickForm dtpForm = null;
        DateTimePickForm dtpfForInstant = null;
        CopyFrom copyFrom = null;
        bool haveBeenEdited = false;

        DateTime ps3Time = new DateTime(2008,1,1);
        DateTime randomEndTime = DateTime.Now;
        bool isOpen = false;
        int baseGamaCount;

        public Form1() 
        {
            /*
            CultureInfo curinfo = null;
            switch (Properties.Settings.Default.Language) { 
                case 0:
                    curinfo = new CultureInfo("zh-TW");
                    break;
                default:
                    curinfo = CultureInfo.CreateSpecificCulture("en");
                    break;
            }*/

            //Thread.CurrentThread.CurrentCulture = curinfo;
            //Thread.CurrentThread.CurrentUICulture = curinfo;
            InitializeComponent();
            toolStripComboBox1.SelectedIndexChanged -= toolStripComboBox1_SelectedIndexChanged;
            //toolStripComboBox1.SelectedIndex = Properties.Settings.Default.Language;
            toolStripComboBox1.SelectedIndexChanged += toolStripComboBox1_SelectedIndexChanged;
            Directory.CreateDirectory("profiles");
            var profiles = new DirectoryInfo("profiles").GetFiles("*.sfo").Select(p => p.Name).ToArray();
            toolStripComboBox2.Items.Add("Default Profile");
            toolStripComboBox2.Items.AddRange(profiles);
            toolStripComboBox2.SelectedIndex = 0;
            consoleComboBoxSelection.SelectedIndex = 0;
            dtpForm = new DateTimePickForm();
            dtpfForInstant = new DateTimePickForm();
            copyFrom = new CopyFrom();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start("http://darkautism.blogspot.tw/");
            System.Diagnostics.Process.Start("https://www.youtube.com/user/TheDarkNachoXD");
        }

        private void 關閉ToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void 開啟ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                OpenFile(folderBrowserDialog1.SelectedPath);
            }
        }

        private void RefreashCompoment() {
            EmptyAllCompoment();
            listViewEx1.BeginUpdate();
            for (int i = 0; i < unlocker.Count; i++) {
                listViewEx1.LargeImageList.Images.Add("", Image.FromFile(path + @"\TROP" + string.Format("{0:000}", unlocker[i].Id) + ".PNG"));
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i; // 在這裡imageid其實等於trophy ID   ex 白金0號, 1...
                lvi.Text = unlocker[i].Name;
                lvi.SubItems.Add(unlocker[i].Detail);
                lvi.SubItems.Add(unlocker[i].Type);
                lvi.SubItems.Add(unlocker[i].Hidden);
                lvi.SubItems.Add((unlocker[i].TrophyInfo.IsUnlock) ? "Yes": "No");
                lvi.SubItems.Add((unlocker[i].TrophyInfo.Time.HasValue && unlocker[i].TrophyInfo.IsSync) ? "Yes": "No");
                if (unlocker[i].TrophyInfo.Time.HasValue) {
                    lvi.SubItems.Add(unlocker[i].TrophyInfo.Time.Value.ToString("yyyy/M/dd  HH:mm:ss"));
                    if (unlocker[i].TrophyInfo.IsSync) lvi.BackColor = Color.LightPink;
                } else {
                    lvi.SubItems.Add(DateTime.MinValue.ToString("yyyy/M/dd  HH:mm:ss"));
                    //lvi.SubItems.Add(tusr.trophyTimeInfoTable[i].Time.ToString("yyyy/M/dd  HH:mm:ss"));
                    lvi.BackColor = Color.LightGray;
                }
                if(unlocker[i].Gid == 0)
                {
                    lvi.SubItems.Add("BaseGame");
                    baseGamaCount = i;
                }
                else lvi.SubItems.Add($"DLC{unlocker[i].Gid}");


                listViewEx1.Items.Add(lvi);
            }
            listViewEx1.EndUpdate();
            CompletionRates();
        }

        private void EmptyAllCompoment() {
            listViewEx1.Items.Clear();
            listViewEx1.LargeImageList.Images.Clear();
            listViewEx1.LargeImageList.ImageSize = new Size(50, 50);
            this.Text = Application.ProductName;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            label2.Text = "00/00";
            label4.Text = "000/000";
        }

        private void CompletionRates() {
            int totalGrade = 0, getGrade = 0, isGetTrophyNumber = 0;
            for (int i = 0; i < unlocker.Count; i++) {
                switch (unlocker[i].Type[0]) {
                    case 'P':
                        totalGrade += 180;
                        getGrade += (unlocker[i].TrophyInfo.Time.HasValue) ? 180 : 0;
                        break;
                    case 'G':
                        totalGrade += 90;
                        getGrade += (unlocker[i].TrophyInfo.Time.HasValue) ? 90 : 0;
                        break;
                    case 'S':
                        totalGrade += 30;
                        getGrade += (unlocker[i].TrophyInfo.Time.HasValue) ? 30 : 0;
                        break;
                    case 'B':
                        totalGrade += 15;
                        getGrade += (unlocker[i].TrophyInfo.Time.HasValue) ? 15 : 0;
                        break;
                }

                if (unlocker[i].TrophyInfo.Time.HasValue) isGetTrophyNumber++;
            }
            progressBar1.Maximum = totalGrade;
            progressBar1.Value = getGrade;
            label2.Text = isGetTrophyNumber + "/" + unlocker.Count;
            label4.Text = getGrade + "/" + totalGrade;
            //this.Text = Application.ProductName + "-[" + tconf.title_name + "]";
        }

        private void listViewEx1_SubItemClicked(object sender, ListViewEx.SubItemEventArgs e) {
            int trophyID = e.Item.ImageIndex;// 在這裡imageid其實等於trophy ID   ex 白金0號, 1...
            if (e.SubItem == 6 && unlocker[trophyID].TrophyInfo.Time.HasValue && !unlocker[trophyID].TrophyInfo.IsSync) { // 已經取得且尚未同步的才可編輯
                listViewEx1.StartEditing(dateTimePicker1, e.Item, e.SubItem);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                if (Directory.Exists(files[0])) {
                    e.Effect = DragDropEffects.All;
                }
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e) {
            String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
            OpenFile(files[0]);
        }

        private void 重新整理ToolStripMenuItem_Click(object sender, EventArgs e) {
            RefreashCompoment();
        }

        private void listViewEx1_SubItemEndEditing(object sender, ListViewEx.SubItemEndEditingEventArgs e) {
            try {

                unlocker.ChangeTime(e.Item.ImageIndex, Convert.ToDateTime(e.DisplayText));
                /*
                tpsn.ChangeTime(e.Item.ImageIndex, Convert.ToDateTime(e.DisplayText));
                TROPUSR.TrophyTimeInfo tti = tusr.trophyTimeInfoTable[e.Item.ImageIndex];
                tti.Time = Convert.ToDateTime(e.DisplayText);
                tusr.trophyTimeInfoTable[e.Item.ImageIndex] = tti;
                */
                haveBeenEdited = true;
            } catch (Exception ex) {
                e.Cancel = true;
                MessageBox.Show(ex.Message);
            }
        }

        private void listViewEx1_DoubleClick(object sender, EventArgs e) {
            int trophyID = ((ListView)sender).SelectedItems[0].ImageIndex;// 在這裡imageid其實等於trophy ID   ex 白金0號, 1...
            ListViewItem lvi = ((ListView)sender).SelectedItems[0];
            if (unlocker[trophyID].TrophyInfo.Time.HasValue && unlocker[trophyID].TrophyInfo.IsSync) { // 尚未同步的才可編輯
                MessageBox.Show("Sync trophy can't be edit");
            } else if (unlocker[trophyID].TrophyInfo.Time.HasValue)
            { // 已經取得的獎杯，刪除之
                if (MessageBox.Show("Delete trophy", "Are you sure you want to delete the trophy?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    //tpsn.DeleteTrophyByID(trophyID);
                    //tusr.LockTrophy(trophyID);
                    lvi.SubItems[4].Text = "No";
                    lvi.BackColor = Color.LightGray;
                    lvi.SubItems[6].Text = new DateTime(0).ToString(dtpForm.dateTimePicker1.CustomFormat);
                    unlocker.LockTrophy(trophyID);
                    //tusr.LockTrophy(trophyID);
                    CompletionRates();
                    haveBeenEdited = true;
                }
            } else {  // nonget
                if (dtpForm.ShowDialog(this) == DialogResult.OK) {
                    //tpsn.PutTrophy(trophyID, tusr.trophyTypeTable[trophyID].Type, dtpForm.dateTimePicker1.Value);
                    //tusr.UnlockTrophy(trophyID, dtpForm.dateTimePicker1.Value);
                    lvi.SubItems[4].Text = "Yes";
                    lvi.BackColor = ((ListView)sender).BackColor;
                    lvi.SubItems[6].Text = dtpForm.dateTimePicker1.Value.ToString(dtpForm.dateTimePicker1.CustomFormat);
                    unlocker.UnlockTrophy(trophyID, dtpForm.dateTimePicker1.Value);
                    //tusr.UnlockTrophy(trophyID, dtpForm.dateTimePicker1.Value);
                    CompletionRates();
                    haveBeenEdited = true;
                }
            }
        }

        private void 存檔ToolStripMenuItem_Click(object sender, EventArgs e) {
            if (isOpen) {
                unlocker.Save();
                haveBeenEdited = false;
            }
        }

        private void 關閉檔案CToolStripMenuItem_Click(object sender, EventArgs e) {
            CloseFile();
        }

        private void OpenFile(string path_in) {
            try {
                if (isOpen) {
                    CloseFile();
                }
                path = path_in;
                Utility.decryptTrophy(path);

                if (consoleComboBoxSelection.SelectedIndex == 0) unlocker = new PS3Unlocker(path);
                else unlocker = new VitaUnlocker(path);
                //tconf = new TROPCONF(path);
                //tpsn = new TROPTRNS(path);
                //tusr = new TROPUSR(path);
                //tpsn.PrintState();
                // tusr.PrintState();
                RefreashCompoment();
                isOpen = true;
                重新整理ToolStripMenuItem.Enabled = true;
                進階ToolStripMenuItem.Enabled = true;
            } catch (Exception ex) {
                unlocker = null;
                //tconf = null;
                //tpsn = null;
                //tusr = null;
                GC.Collect();
                Utility.encryptTrophy(path, toolStripComboBox2.Text);
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("Open Failed:" + ex.Message);
            }
        }

        public bool CloseFile() {
            if (haveBeenEdited) {
                DialogResult dr = MessageBox.Show("Close", "Do you want to close?", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes) {
                    // tpsn.PrintState();
                    // tusr.PrintState();
                    unlocker.Save();
                } else if (dr == DialogResult.No) {
                } else {
                    return false;
                }
            }
            unlocker = null;
            EmptyAllCompoment();
            haveBeenEdited = false;
            重新整理ToolStripMenuItem.Enabled = false;
            進階ToolStripMenuItem.Enabled = false;
            if (isOpen) {
                Utility.encryptTrophy(path,toolStripComboBox2.Text);
                isOpen = false;
            }
            return true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            if (isOpen) {
                e.Cancel = !CloseFile();
            }
        }

        private void 瞬間白金ToolStripMenuItem_Click(object sender, EventArgs e) {
            Random rand = new Random((int)DateTime.Now.Ticks);
            int i;

            //Base game
            for (i = 1; i < unlocker.Count && unlocker[i].Gid == 0; i++) 
            {
                if (!unlocker[i].TrophyInfo.IsUnlock)
                {
                    unlocker.UnlockTrophy(i, new DateTime(Utility.LongRandom(ps3Time.Ticks, randomEndTime.Ticks, rand)));
                    //tusr.UnlockTrophy(i, new DateTime(Utility.LongRandom(ps3Time.Ticks, randomEndTime.Ticks, rand)));
                    //tpsn.PutTrophy(i, tusr.trophyTypeTable[i].Type, new DateTime(Utility.LongRandom(ps3Time.Ticks, randomEndTime.Ticks, rand)));
                }
            }
            //Platinium game
            if (!unlocker[0].TrophyInfo.IsUnlock) {
                unlocker.UnlockTrophy(0, new DateTime(Utility.LongRandom(ps3Time.Ticks, randomEndTime.Ticks, rand)));
                //tusr.UnlockTrophy(0, tpsn.GetLastTrophyTime().AddSeconds(1));
                //tpsn.PutTrophy(0, tusr.trophyTypeTable[0].Type, tpsn.GetLastTrophyTime().AddSeconds(1));
            }

            //DLC 
            for (; i < unlocker.Count; i++)
            {
                if (!unlocker[i].TrophyInfo.IsUnlock)
                {
                    unlocker.UnlockTrophy(i, new DateTime(Utility.LongRandom(ps3Time.Ticks, randomEndTime.Ticks, rand)));

                    //tusr.UnlockTrophy(i, new DateTime(Utility.LongRandom(ps3Time.Ticks, randomEndTime.Ticks, rand)));
                    //tpsn.PutTrophy(i, tusr.trophyTypeTable[i].Type, new DateTime(Utility.LongRandom(ps3Time.Ticks, randomEndTime.Ticks, rand)));
                }
            }
            haveBeenEdited = true;
            RefreashCompoment();
        }

        private void 清除獎杯ToolStripMenuItem_Click(object sender, EventArgs e) {
            /*TROPTRNS.TrophyInfo? ti = tpsn.PopTrophy();
            while (ti.HasValue) {
                tusr.LockTrophy(ti.Value.TrophyID);
                ti = tpsn.PopTrophy();
            }*/
            foreach (var trophy in unlocker) unlocker.LockTrophy(trophy);
            haveBeenEdited = true;
            RefreashCompoment();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            //Properties.Settings.Default.Language = toolStripComboBox1.SelectedIndex;
            //Properties.Settings.Default.Save();
           // MessageBox.Show(Properties.strings.RestartProgram);
        }

        private void setRandomStartTimeToolStripMenuItem_Click(object sender, EventArgs e) {
            dtpfForInstant.Title.Text = "Random Time";
            if (dtpfForInstant.ShowDialog() == DialogResult.OK) {
                ps3Time = dtpfForInstant.dateTimePicker1.Value;
            }
        }

        private void setRandomEndTimeToolStripMenuItem_Click(object sender, EventArgs e) {
            dtpfForInstant.Title.Text = "Random Time";
            if (dtpfForInstant.ShowDialog() == DialogResult.OK) {
                randomEndTime = dtpfForInstant.dateTimePicker1.Value;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (copyFrom.ShowDialog(this) == DialogResult.OK)
            {
                var _times = copyFrom.checkBox1.Checked ? copyFrom.smartCopy().ToList() : copyFrom.copyFrom().ToList();
                if(_times.Any()) 清除獎杯ToolStripMenuItem_Click(sender, e); // no idea why but sometimes it get bug and it don't update, so lockin first fix it
                try
                {
                    for (int i = 0; i < unlocker.Count; ++i)
                    {

                        if (!unlocker[i].TrophyInfo.Time.HasValue && _times[i] != 0)
                        {
                            //var time = _times[i].TimeStampToDateTime();
                            unlocker.UnlockTrophy(i, _times[i].TimeStampToDateTime());
                            //tusr.UnlockTrophy(i, time);
                            //tpsn.PutTrophy(i, tusr.trophyTypeTable[i].Type, time);
                        }
                    }
                    haveBeenEdited = true;
                    RefreashCompoment();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
    }
}
