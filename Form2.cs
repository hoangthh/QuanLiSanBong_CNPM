using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyDatLichSanBong
{
    public partial class Form2 : Form
    {
        private static DateTime thisDateTime;
        public Form2()
        {
            InitializeComponent();
            thisDateTime = DateTime.Today.Date;
        }
        private Tuple<string, string> DateHandle(DateTime startDate, DateTime endDate)
        {
            string datestart = startDate.ToShortDateString();
            string dateend = endDate.ToShortDateString();
            return Tuple.Create(datestart, dateend);
        }
        private void Load_Date()
        {
            UpdateTag();
            DateTime currentDate = thisDateTime.Date;
            int daysToSubtract = ((int)currentDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime startDateOfWeek = currentDate.AddDays(-daysToSubtract);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(6);
            Tuple<string, string> DateSolve = DateHandle(startDateOfWeek, endDateOfWeek);
            labelDayMonth.Text = DateSolve.Item1 + " - " + DateSolve.Item2;
        }
        private void UpdateTag()
        {
            DateTime currentDate = thisDateTime.Date;
            int daysToSubtract = ((int)currentDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime startDateOfWeek = currentDate.AddDays(-daysToSubtract);
            for (int i = 6; i < tableLayoutPanel1.ColumnCount; i+=4)
            {
                Control control = tableLayoutPanel1.GetControlFromPosition(i, 0);
                Label timelabel = control as Label;
                timelabel.Tag = startDateOfWeek.AddDays((i - 6) / 4).ToShortDateString();
            }
        }

        public DateTime ConvertToDateTime1(string dateString)
        {
            string[] strings = dateString.Split('/');
            string Day;
            string Month;
            if (strings[1].Length == 1)
            {
                Day = "0" + strings[1];
            }
            else
            {
                Day = strings[1];
            }
            if (strings[0].Length == 1)
            {
                Month = "0" + strings[0];
            }
            else
            {
                Month = strings[0];
            }
            string datenew = strings[strings.Length - 1] + "-" + Month + "-" + Day;
            DateTime dateTime = DateTime.ParseExact(datenew, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return dateTime;
        }
        private void ClearTable()
        {
            int columncount = tableLayoutPanel1.ColumnCount;
            int rowcount = tableLayoutPanel1.RowCount;
            for (int i = 1; i < tableLayoutPanel1.RowCount; i++)
            {
                for (int j = 6; j < tableLayoutPanel1.ColumnCount; j++)
                {
                    Control control = tableLayoutPanel1.GetControlFromPosition(j, i);
                    if (control != null)
                    {
                        tableLayoutPanel1.Controls.Remove(control);
                        control.Dispose();
                    }
                }
            }
        }
        private void Load_Table()
        {
            UpdateTag();
            int columncount = tableLayoutPanel1.ColumnCount;
            int rowcount = tableLayoutPanel1.RowCount;
            using (var db = new QuanLySanBongEntities())
            {
                for (int i = 1; i < rowcount; i++)
                {
                    Control controlTime = tableLayoutPanel1.GetControlFromPosition(2, i);
                    Label timelabel = controlTime as Label;
                    for (int j = 6; j < columncount; j++)
                    {
                        Control controlDay = tableLayoutPanel1.GetControlFromPosition(j, 0);
                        Label daylabel = controlDay as Label;
                        string daytimetag = daylabel.Tag.ToString() + " " + timelabel.Text;
                        //Dữ liệu để tìm kiếm trong database
                        DateTime labelday = ConvertToDateTime1(daylabel.Tag.ToString());
                        Tuple<TimeSpan, TimeSpan> Timeusing = TimeSolving(timelabel.Text);
                        TimeSpan StartTime = Timeusing.Item1;
                        TimeSpan EndTime = Timeusing.Item2;
                        int pitchnumber = (((j - 2) % 4) + 1);
                        var PhieuDatSan = db.Bookings.Where(p => p.StartTime == StartTime
                        && p.EndTime == EndTime && p.BookingDate == labelday && p.SanID == pitchnumber).FirstOrDefault();
                        //
                        string pitchnumbertostring = (((j - 2) % 4) + 1).ToString();
                        Label newlabel = new Label();
                        newlabel.Dock = DockStyle.Fill;
                        if (PhieuDatSan == null)
                        {
                            newlabel.Text = "";
                        }
                        else
                        {
                            newlabel.Text = "X";
                            newlabel.Font = new Font(newlabel.Font.FontFamily, 25); // Đặt kích thước font là 15
                            newlabel.ForeColor = Color.Red;
                            newlabel.Click += Label_Click; // Gán sự kiện Click cho Label mới tạo
                        }
                        newlabel.Tag = daytimetag + " " + pitchnumber; 
                        newlabel.TextAlign = ContentAlignment.MiddleCenter;
                        newlabel.Margin = new Padding(0);
                        newlabel.BorderStyle = BorderStyle.FixedSingle;
                        tableLayoutPanel1.Controls.Add(newlabel, j, i);
                    }
                }
            }
        }
        private Tuple<TimeSpan, TimeSpan> TimeSolving(string TimeText)
        {
            string[] parts = TimeText.Split('-');
            string startTimeString = parts[0].Trim();
            string endTimeString = parts[1].Trim();
            TimeSpan startTimeSpan = TimeSpan.Parse(startTimeString);
            TimeSpan endTimeSpan = TimeSpan.Parse(endTimeString);
            return new Tuple<TimeSpan, TimeSpan>(startTimeSpan, endTimeSpan);
        }

        private void Label_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện click cho Label
            Label clickedLabel = sender as Label;
            if (clickedLabel != null)
            {
                string labelTag = clickedLabel.Tag.ToString();
                Form4 newform = new Form4(labelTag);
                newform.ShowDialog();
                Form2_Load(sender, e);
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            ClearTable();
            Load_Table();
            Load_Date();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            thisDateTime = thisDateTime.AddDays(7);
            Load_Date();
            ClearTable();
            Load_Table();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            thisDateTime = thisDateTime.AddDays(-7);
            Load_Date();
            ClearTable();
            Load_Table();

        }
    }
}
