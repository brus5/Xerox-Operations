﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xerox_operations.values;
using xerox_operations_0._0._1.datacard;
using xerox_operations_0._0._1.utils;
using xerox_operations.window;
using xerox_operations.utils;

namespace xerox_operations_0._0._1
{
    /// <summary>
    /// This class represents Main View of program with all controls and units.
    /// </summary>
    public partial class MainForm : Form
    {
        private DirectoryMonitor b2b;
        private DirectoryMonitor b2c;
        private Printer nv1;
        private Printer nv2;
        private Printer nv3;
        private Printer nv4;
        private Printer phaser;


        ToolTip tip = new ToolTip();
        private string timeStamp = string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now);

        public MainForm()
        {
            InitializeComponent();
            b2b = new DirectoryMonitor(this, Paths.B2B_PATH);
            b2c = new DirectoryMonitor(this, Paths.B2C_PATH);
            initPrinters();
            initDataCards();
            Load += MainForm_ChangeSize;
            new Clock(this);
 
            FileCounter fileCounterStoreTno = new FileCounter(this, Paths.STORE_TNO);
            fileCounterStoreTno.loadViewStoreTnoDelayed();
            
            FileCounter fileCounterMaterials = new FileCounter(this, Paths.MATERIALS);
            fileCounterMaterials.loadViewMaterialsDelayed();
        }

        /// <summary>
        /// Creating printers on different threads.
        /// </summary>
        private void initPrinters()
        {
            nv1 = new Printer("nv1", Printers.IP_NV1);
            PrinterStats psNv1 = new PrinterStats(this, nv1);
            Thread thread_1 = new Thread(psNv1.getPrinterStatus);
            thread_1.Start();

            nv2 = new Printer("nv2", Printers.IP_NV2);
            PrinterStats psNv2 = new PrinterStats(this, nv2);
            Thread thread_2 = new Thread(psNv2.getPrinterStatus);
            thread_2.Start();

            nv3 = new Printer("nv3", Printers.IP_NV3);
            PrinterStats psNv3 = new PrinterStats(this, nv3);
            Thread thread_3 = new Thread(psNv3.getPrinterStatus);
            thread_3.Start();

            nv4 = new Printer("nv4", Printers.IP_NV4);
            PrinterStats psNv4 = new PrinterStats(this, nv4);
            Thread thread_4 = new Thread(psNv4.getPrinterStatus);
            thread_4.Start();

            phaser = new Printer("Phaser", Printers.IP_PHASER);
            PrinterStats psPhaser = new PrinterStats(this, phaser);
            Thread thread_5 = new Thread(psPhaser.getPrinterStatus);
            thread_5.Start();
        }

        /// <summary>
        /// Creating Datacards on different threads.
        /// </summary>
        private void initDataCards()
        {
            Datacard dc2 = new Datacard("CD800-2", Datacards.IP_DC2, Datacards.NumID_SM_DC2);
            DatacardStats datacardStats2 = new DatacardStats(this, dc2);
            Thread thread_7 = new Thread(datacardStats2.getDatacardStatus);
            thread_7.Start();

            Datacard dc3 = new Datacard("CD800-3", Datacards.IP_DC3, Datacards.NumID_SM_DC3);
            DatacardStats datacardStats3 = new DatacardStats(this, dc3);
            Thread thread_8 = new Thread(datacardStats3.getDatacardStatus);
            thread_8.Start();

            Datacard dc4 = new Datacard("CD800-4", Datacards.IP_DC4, Datacards.NumID_SM_DC4);
            DatacardStats datacardStats4 = new DatacardStats(this, dc4);
            Thread thread_9 = new Thread(datacardStats4.getDatacardStatus);
            thread_9.Start();

            Datacard dc5 = new Datacard("CD800-5", Datacards.IP_DC5, Datacards.NumID_SM_DC5);
            DatacardStats datacardStats5 = new DatacardStats(this, dc5);
            Thread thread_10 = new Thread(datacardStats5.getDatacardStatus);
            thread_10.Start();

            Datacard dc6 = new Datacard("CD800-6", Datacards.IP_DC6, Datacards.NumID_SM_DC6);
            DatacardStats datacardStats6 = new DatacardStats(this, dc6);
            Thread thread_11 = new Thread(datacardStats6.getDatacardStatus);
            thread_11.Start();
        }

        /// <summary>
        /// Poiting window to new Location at startup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainForm_ChangeSize(object sender, EventArgs e)
        {
            Location = new Point(880, 0);
        }

        /// <summary>
        /// Button responsible for cleaning log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            b2c.clearLog();
            richTextBoxB2C_TextChanged(sender, e);
            b2b.clearLog();
            richTextBoxB2B_TextChanged(sender, e);
        }

        /// <summary>
        /// Richbox B2B textChanger. This method add text and Scrolling down to the end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void richTextBoxB2B_TextChanged(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                this.richTextBoxB2B.Text = b2b.getFileName();
                this.richTextBoxB2B.SelectionStart = this.richTextBoxB2B.Text.Length;
                this.richTextBoxB2B.ScrollToCaret();
            });
        }

        /// <summary>
        /// Richbox B2C textChanger. This method add text and Scrolling down to the end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void richTextBoxB2C_TextChanged(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                this.richTextBoxB2C.Text = b2c.getFileName();
                this.richTextBoxB2C.SelectionStart = this.richTextBoxB2C.Text.Length;
                this.richTextBoxB2C.ScrollToCaret();
            });
        }

        // *********** START PRINTERS *********** // 
        // Nuvera144-1
        /// <summary>
        /// Nuvera 1
        /// This methods will be responsible for updating view.
        /// </summary>
        /// <param name="value"></param>
        public void smNv1LabelChanged(string value)
        {
            InvokeLabelFromThread(smNuvera1Label, value);
        }

        /// <summary>
        /// When status changes, this indicator updates itself
        /// </summary>
        /// <param name="value"></param>
        public void smNv1IndicatorChanged(int value)
        {
            changeStatus(this.smNv1StatusIndicator, value);
        }

        /// <summary>
        /// When there is low paper in one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smNv1LowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv1LowPaperIndicator1, isEmpty);
        }

        /// <summary>
        /// When there is low paper in more that one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smNv1LowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv1LowPaperIndicator2, isEmpty);
        }

        /// <summary>
        /// When FinisherA finished. 
        /// </summary>
        /// <param name="isFinished"></param>
        public void smNv1FinishedStackerA(bool isFinished)
        {
            setVisibilityIndicator(this.smNv1FinishIndicator1, isFinished);
        }

        /// <summary>
        /// When FinisherB finished.
        /// </summary>
        /// <param name="isFinished"></param>
        public void smNv1FinishedStackerB(bool isFinished)
        {
            setVisibilityIndicator(this.smNv1FinishIndicator2, isFinished);
        }

        /// <summary>
        /// When cursor poiting on StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smNv1StatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(nv1.messages);
        }

        /// <summary>
        /// When cursor leaves from StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smNv1StatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }

        // Nuvera144-2
        /// <summary>
        /// Nuvera 2
        /// This methods will be responsible for updating view.
        /// </summary>
        /// <param name="value"></param>
        public void smNv2LabelChanged(string value)
        {
            InvokeLabelFromThread(smNuvera2Label, value);
        }

        /// <summary>
        /// When status changes, this indicator updates itself
        /// </summary>
        /// <param name="value"></param>
        public void smNv2IndicatorChanged(int value)
        {
            changeStatus(this.smNv2StatusIndicator, value);
        }

        /// <summary>
        /// When there is low paper in one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smNv2LowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv2LowPaperIndicator1, isEmpty);
        }

        /// <summary>
        /// When there is low paper in more that one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smNv2LowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv2LowPaperIndicator2, isEmpty);
        }

        /// <summary>
        /// When FinisherA finished. 
        /// </summary>
        /// <param name="isFinished"></param>
        public void smNv2FinishedStackerA(bool isFinished)
        {
            setVisibilityIndicator(this.smNv2FinishIndicator1, isFinished);
        }

        /// <summary>
        /// When FinisherB finished.
        /// </summary>
        /// <param name="isFinished"></param>
        public void smNv2FinishedStackerB(bool isFinished)
        {
            setVisibilityIndicator(this.smNv2FinishIndicator2, isFinished);
        }

        /// <summary>
        /// When cursor poiting on StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smNv2StatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(nv2.messages);
        }

        /// <summary>
        /// When cursor leaves from StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smNv2StatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }

        // Nuvera144-3
        /// <summary>
        /// Nuvera 3
        /// This methods will be responsible for updating view.
        /// </summary>
        /// <param name="value"></param>
        public void smNv3LabelChanged(string value)
        {
            InvokeLabelFromThread(smNuvera3Label, value);
        }

        /// <summary>
        /// When status changes, this indicator updates itself
        /// </summary>
        /// <param name="value"></param>
        public void smNv3IndicatorChanged(int value)
        {
            changeStatus(this.smNv3StatusIndicator, value);
        }

        /// <summary>
        /// When there is low paper in one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smNv3LowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv3LowPaperIndicator1, isEmpty);
        }

        /// <summary>
        /// When there is low paper in more that one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smNv3LowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv3LowPaperIndicator2, isEmpty);
        }

        /// <summary>
        /// When FinisherA finished. 
        /// </summary>
        /// <param name="isFinished"></param>
        public void smNv3FinishedStackerA(bool isFinished)
        {
            setVisibilityIndicator(this.smNv3FinishIndicator1, isFinished);
        }

        /// <summary>
        /// When FinisherB finished.
        /// </summary>
        /// <param name="isFinished"></param>
        public void smNv3FinishedStackerB(bool isFinished)
        {
            setVisibilityIndicator(this.smNv3FinishIndicator2, isFinished);
        }

        /// <summary>
        /// When cursor poiting on StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smNv3StatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(nv3.messages);
        }

        /// <summary>
        /// When cursor leaves from StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smNv3StatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }


        // Nuvera144-4
        /// <summary>
        /// Nuvera 4
        /// This methods will be responsible for updating view.
        /// </summary>
        /// <param name="value"></param>
        public void smNv4LabelChanged(string value)
        {
            InvokeLabelFromThread(this.smNuvera4Label, value);
        }

        /// <summary>
        /// When status changes, this indicator updates itself
        /// </summary>
        /// <param name="value"></param>
        public void smNv4IndicatorChanged(int value)
        {
            changeStatus(this.smNv4StatusIndicator, value);
        }

        /// <summary>
        /// When there is low paper in one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smNv4LowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv4LowPaperIndicator1, isEmpty);
        }

        /// <summary>
        /// When there is low paper in more that one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smNv4LowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smNv4LowPaperIndicator2, isEmpty);
        }

        /// <summary>
        /// When FinisherA finished. 
        /// </summary>
        /// <param name="isFinished"></param>
        public void smNv4FinishedStackerA(bool isFinished)
        {
            setVisibilityIndicator(this.smNv4FinishIndicator1, isFinished);
        }

        /// <summary>
        /// When FinisherB finished.
        /// </summary>
        /// <param name="isFinished"></param>
        public void smNv4FinishedStackerB(bool isFinished)
        {
            setVisibilityIndicator(this.smNv4FinishIndicator2, isFinished);
        }

        /// <summary>
        /// When cursor poiting on StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smNv4StatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(nv4.messages);
        }

        /// <summary>
        /// When cursor leaves from StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smNv4StatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }

        // Phaser 5550
        /// <summary>
        /// Phaser 5550
        /// This methods will be responsible for updating view.
        /// </summary>
        /// <param name="value"></param>
        public void smPhaserLabelChanged(string value)
        {
            InvokeLabelFromThread(this.smPhaserLabel, value);
        }

        /// <summary>
        /// When status changes, this indicator updates itself
        /// </summary>
        /// <param name="value"></param>
        public void smPhaserIndicatorChanged(int value)
        {
            changeStatus(this.smPhaserStatusIndicator, value);
        }

        /// <summary>
        /// When there is low paper in one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smPhaserLowPaperIndicator1Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smPhaserLowPaperIndicator1, isEmpty);
        }

        /// <summary>
        /// When there is low paper in more that one tray
        /// </summary>
        /// <param name="isEmpty"></param>
        public void smPhaserLowPaperIndicator2Changed(bool isEmpty)
        {
            setVisibilityIndicator(this.smPhaserLowPaperIndicator2, isEmpty);
        }

        /// <summary>
        /// When cursor poiting on StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smPhaserStatusIndicator_MouseHover(object sender, EventArgs e)
        {
            addItemsToListView(phaser.messages);
        }

        /// <summary>
        /// When cursor leaves from StatusIndicator icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void smPhaserStatusIndicator_MouseLeave(object sender, EventArgs e)
        {
            clearPrinterStatusListView();
        }

        // *********** END PRINTERS *********** // 

        // *********** START DATACARDS *********** // 
        // Datacard 2
        /// <summary>
        /// Updates percentage of ribbon
        /// </summary>
        /// <param name="value"></param>
        public void dc2_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc2RibbonLabel, value.ToString() + "%");
        }

        /// <summary>
        /// Shows actual billing state
        /// </summary>
        /// <param name="value"></param>
        public void dc2_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc2BillingLabel, value.ToString());
        }

        /// <summary>
        /// Actual status of printer
        /// </summary>
        /// <param name="value"></param>
        public void dc2_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc2StatusIndicator, value);
        }

        /// <summary>
        /// Shows if printer is locked by user.
        /// </summary>
        /// <param name="isLocked"></param>
        public void dc2_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc2LockIndicator, isLocked);
        }

        // Datacard 3
        /// <summary>
        /// Updates percentage of ribbon
        /// </summary>
        /// <param name="value"></param>
        public void dc3_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc3RibbonLabel, value.ToString() + "%");
        }

        /// <summary>
        /// Shows actual billing state
        /// </summary>
        /// <param name="value"></param>
        public void dc3_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc3BillingLabel, value.ToString());
        }

        /// <summary>
        /// Actual status of printer
        /// </summary>
        /// <param name="value"></param>
        public void dc3_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc3StatusIndicator, value);
        }

        /// <summary>
        /// Shows if printer is locked by user.
        /// </summary>
        /// <param name="isLocked"></param>
        public void dc3_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc3LockIndicator, isLocked);
        }

        // Datacard 4
        /// <summary>
        /// Updates percentage of ribbon
        /// </summary>
        /// <param name="value"></param>
        public void dc4_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc4RibbonLabel, value.ToString() + "%");
        }

        /// <summary>
        /// Shows actual billing state
        /// </summary>
        /// <param name="value"></param>
        public void dc4_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc4BillingLabel, value.ToString());
        }

        /// <summary>
        /// Actual status of printer
        /// </summary>
        /// <param name="value"></param>
        public void dc4_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc4StatusIndicator, value);
        }

        /// <summary>
        /// Shows if printer is locked by user.
        /// </summary>
        /// <param name="isLocked"></param>
        public void dc4_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc4LockIndicator, isLocked);
        }

        // Datacard 5
        /// <summary>
        /// Updates percentage of ribbon
        /// </summary>
        /// <param name="value"></param>
        public void dc5_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc5RibbonLabel, value.ToString() + "%");
        }

        /// <summary>
        /// Shows actual billing state
        /// </summary>
        /// <param name="value"></param>
        public void dc5_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc5BillingLabel, value.ToString());
        }

        /// <summary>
        /// Actual status of printer
        /// </summary>
        /// <param name="value"></param>
        public void dc5_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc5StatusIndicator, value);
        }

        /// <summary>
        /// Shows if printer is locked by user.
        /// </summary>
        /// <param name="isLocked"></param>
        public void dc5_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc5LockIndicator, isLocked);
        }

        // Datacard 6
        /// <summary>
        /// Updates percentage of ribbon
        /// </summary>
        /// <param name="value"></param>
        public void dc6_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc6RibbonLabel, value.ToString() + "%");
        }

        /// <summary>
        /// Shows actual billing state
        /// </summary>
        /// <param name="value"></param>
        public void dc6_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc6BillingLabel, value.ToString());
        }

        /// <summary>
        /// Actual status of printer
        /// </summary>
        /// <param name="value"></param>
        public void dc6_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc6StatusIndicator, value);
        }

        /// <summary>
        /// Shows if printer is locked by user.
        /// </summary>
        /// <param name="isLocked"></param>
        public void dc6_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc6LockIndicator, isLocked);
        }

        // Datacard 7
        /// <summary>
        /// Updates percentage of ribbon
        /// </summary>
        /// <param name="value"></param>
        public void dc7_ribbonLabelChanged(int value)
        {
            InvokeLabelFromThread(this.dc7RibbonLabel, value.ToString() + "%");
        }

        /// <summary>
        /// Shows actual billing state
        /// </summary>
        /// <param name="value"></param>
        public void dc7_billingLabelChanged(long value)
        {
            InvokeLabelFromThread(this.dc7BillingLabel, value.ToString());            
        }

        /// <summary>
        /// Actual status of printer
        /// </summary>
        /// <param name="value"></param>
        public void dc7_StatusIndicatorChanged(int value)
        {
            changeStatus(this.dc7StatusIndicator, value);
        }

        /// <summary>
        /// Shows if printer is locked by user.
        /// </summary>
        /// <param name="isLocked"></param>
        public void dc7_LockIndicatorChanged(bool isLocked)
        {
            setVisibilityIndicator(this.dc7LockIndicator, isLocked);
        }

        // *********** END DATACARDS *********** // 

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Point locationOnForm = smNv1StatusIndicator.FindForm().PointToClient(
            smNv1StatusIndicator.Parent.PointToScreen(smNv1StatusIndicator.Location));
            Console.WriteLine(locationOnForm);

            string s = "Łukasz Krawczak";
            //var listViewItem = new ListViewItem(s);
            //printerStatusListView.Items.Add(listViewItem);
        }

        /// <summary>
        /// Adds items to List View
        /// </summary>
        /// <param name="item"></param>
        private void addItemsToListView(List<string> messages)
        {
            foreach (var item in messages)
            {
                var listViewItem = new ListViewItem(item);
                printerStatusListView.Items.Add(listViewItem);
            }
        }    

        /// <summary>
        /// This method is responsible for cleaning listview
        /// </summary>
        public void clearPrinterStatusListView()
        {
            printerStatusListView.Items.Clear();
        }


        /// <summary>
        /// This method is responsible for hiding PictureBox from MainForm
        /// </summary>
        /// <param name="pictureBox"></param>
        /// <param name="isEmpty"></param>
        public void setVisibilityIndicator(PictureBox pictureBox, bool isEmpty)
        {
            Invoke((MethodInvoker)delegate
            {
                if (isEmpty) pictureBox.Show();
                else pictureBox.Hide();
            });
        }

        

        public void statusIndicatorRed(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_red.png");
        }

        public void statusIndicatorGreen(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_green.png");
        }

        public void statusIndicatorOrange(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_orange.png");
        }

        public void statusIndicatorCircle(PictureBox pictureBox)
        {
            pictureBox.Image = Image.FromFile("../Image/icon_indicator_circle_dark_green.png");
        }

        public void changeStatus(PictureBox pictureBox, int status)
        {
            switch (status)
            {
                case 1:
                    statusIndicatorRed(pictureBox);
                    break;
                case 2:
                    statusIndicatorGreen(pictureBox);
                    break;
                case 4:
                    statusIndicatorCircle(pictureBox);
                    break;
                case 5:
                    statusIndicatorOrange(pictureBox);
                    break;
                default:
                    statusIndicatorGreen(pictureBox);
                    break;
            }
        }

        //public void addItemToMethod(string item)
        //{
        //    Invoke((MethodInvoker)delegate
        //    {
        //        this.listView1.Items.Add(item);
        //    });
        //}

        public void onDriveProgressBar(int progress)
        {
            Invoke((MethodInvoker)delegate
            {
                this.progressBarDriveD.Value = progress;
                if (progress > 80)
                {
                    this.progressBarDriveD.ForeColor = Color.Red;
                }
            });
        }

        public void onDriveFreeSpaceAvailableLabelChanged(int progress)
        {
            InvokeLabelFromThread(this.labelDriveD, progress.ToString() + "% wolnego miejsca");
            //Invoke((MethodInvoker)delegate
            //{
            //    this.labelDriveD.Text = progress.ToString() + "% wolnego miejsca";
            //});
        }

        public void onToolStripStatusLabelSessionTimeChanged(string time)
        {
            Invoke((MethodInvoker)delegate
            {
                this.toolStripStatusLabelSessionTime.Text = "Czas sesji: " + time;
            });
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationController.close();
        }

        /// <summary>
        /// Invoking Text to UI from another Thread.
        /// </summary>
        /// <param name="label">Label which will be invoked</param>
        /// <param name="text">Text to change</param>
        private void InvokeLabelFromThread(Label label, string text)
        {
            Invoke((MethodInvoker)delegate
            {
                label.Text = text;
            });
        }
        
        private void showToolTip(string printerName, Point locationOnForm)
        {
            tip.ToolTipTitle = "NV1";
            tip.ToolTipIcon = ToolTipIcon.Info;
            tip.IsBalloon = true;
            tip.Show("hello! \r\nasdasd \r\nblablabla \r\nasdasdasdasd", this, new Point(locationOnForm.X, locationOnForm.Y - 40));
        }

        private void buttonDeleteOrderOrSid_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"C:\Skrypty\Usprawnienie\Usprawnienie_prod.jar");          
        }

        private void buttonBlockCurierLabels_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"C:\Skrypty\BlokowanieEtykietKurierskich.cmd");
        }

        private void buttonPrintInstalments_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"C:\Skrypty\Drukowanie instalmentow z wave.bat");
        }

        private void buttonReprintLeaflet_Click(object sender, EventArgs e)
        {
            ButtonController.startAnotherApplication(@"C:\Jonit\wfd\smProd\si\prod-sm-InsertReprint.pl");
        }

        private void buttonReprocessSid_Click(object sender, EventArgs e)
        {
            new ReprocessSid(openFileDialogReprocessSid);
        }

        public void labelFileStoreTnoNames_TextChanged(string value)
        {
            InvokeLabelFromThread(this.labelFileStoreTnoNames, value);
        }

        public void labelFileStoreTnoValues_TextChanged(string value)
        {
            InvokeLabelFromThread(this.labelFileStoreTnoCounter, value);
        }

        public void labelFileMaterialsNames_TextChanged(string value)
        {
            InvokeLabelFromThread(this.labelFileMaterialsNames, value);
        }

        public void labelFileMaterialsValues_TextChanged(string value)
        {
            InvokeLabelFromThread(this.labelFileMaterialsCounter, value);
        }
    }
}
