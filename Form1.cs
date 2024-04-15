using Guna.UI2.WinForms;
using QuanLyDatLichSanBong.UsingUserControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyDatLichSanBong
{
    public partial class Form1 : Form
    {
        static string information;
        static string checkedbutton;
        static int IDUser;
        public Form1(int ID)
        {
            InitializeComponent();
            IDUser = ID;
            DisplayDays();
            UpdateDay();
            DisableButton();
        }
        #region Calendar
        public static int month, year, DayPicked, DayNum;
        public static List<int> Days;
        private bool CheckButton(string time)
        {
            Tuple<TimeSpan, TimeSpan> Timeusing = TimeSolving(time);
            using(var db = new QuanLySanBongEntities())
            {
                DateTime usingdate = ConvertToDateTime1(takeTimePicked());
                var booking = db.Bookings.Where(p => p.BookingDate == usingdate
                && p.StartTime == Timeusing.Item1 && p.EndTime == Timeusing.Item2).Count();
                if (booking >= 4) return true;
                return false;
            }
        }
        private void DisableButton()
        {
            foreach(Control x in flowLayoutPanel1.Controls)
            {
                if(x is Guna2Button item)
                {
                    if (CheckButton(item.Text))
                    {
                        item.Enabled = false;
                    }
                    else
                    {
                        item.Enabled = true;
                    }
                }
            }
        }
        private void WireAllControls(Control cont)
        {
            foreach (Control ctl in cont.Controls)
            {
                ctl.Click += ctl_Click;
                if (ctl.HasChildren)
                {
                    WireAllControls(ctl);
                }
            }
        }
        private void ClearCadetSlot()
        {
            foreach (Control x in dayFlowpanel.Controls)
            {
                if (x is UserDay)
                {
                    if (x.BackColor == Color.CadetBlue)
                    {
                        x.BackColor = Color.FromArgb(240, 252, 254);
                    }
                }
            }
        }
        private void ctl_Click(object sender, EventArgs e)
        {
            this.InvokeOnClick(dayFlowpanel, EventArgs.Empty);

        }
        private void MoveDown_Click(object sender, EventArgs e)
        {
            if (month > 1)
            {
                dayFlowpanel.Controls.Clear();
                month--;
                Update();
            }
        }

        private void MoveUp_Click(object sender, EventArgs e)
        {
            if (month < 12)
            {
                dayFlowpanel.Controls.Clear();
                month++;
                Update();
            }
        }
        private void UpdateDay()
        {
            foreach (Control x in dayFlowpanel.Controls)
            {
                if (x is UserDay x1)
                {
                    if (x1.BackColor == Color.CadetBlue)
                    {
                        DayPicked = Convert.ToInt32(x1.labelText());
                    }
                }
            }
        }
        private void DisplayDays()
        {
            DateTime now = DateTime.Now;
            month = now.Month;
            year = now.Year;
            Update();
        }
        public void Update()
        {
            string monthname = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
            labelYearMonth.Text = monthname + " " + year;
            DateTime startofthemonth = new DateTime(year, month, 1);
            int days = DateTime.DaysInMonth(year, month);
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")) + 1;
            for (int i = 1; i < dayoftheweek; i++)
            {
                UserBlank Blank = new UserBlank();
                dayFlowpanel.Controls.Add(Blank);
            }
            for (int i = 1; i <= days; i++)
            {
                UserDay DayControl = new UserDay();
                DayControl.days(i);
                if (i == DateTime.Now.Day)
                {
                    DayControl.BackColor = Color.CadetBlue;
                    DayPicked = DateTime.Now.Day;
                    Days = new List<int>();
                    Days.Add(DayPicked);
                }
                DayControl.Click += DayControl_Click;
                dayFlowpanel.Controls.Add(DayControl);
            }
            WireAllControls(dayFlowpanel);
        }
        private void DayControl_Click(object sender, EventArgs e)
        {
            UserDay pressedbutton = sender as UserDay;
            if (pressedbutton.BackColor != Color.CadetBlue)
            {
                ClearCadetSlot();
                pressedbutton.BackColor = Color.CadetBlue;
            }
            UpdateDay();
            UncheckButton();
            DisableButton();
            //LoadingCaLam(sender, e);
            //LoadTableLayOutPanel();
        }
        private void UncheckButton()
        {
            foreach (Guna2Button item in flowLayoutPanel1.Controls)
            {
                item.Checked = false;
            }
            checkedbutton = null;
        }
        private void labelThongTinSan_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button17_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(takeTimePicked()) || string.IsNullOrEmpty(checkedbutton))
            {
                MessageBox.Show("Hãy chọn đầy đủ các thông tin cần đặt");
                return;
            }
            information =  IDUser + " " + takeTimePicked() + " " + checkedbutton;
            Form3 newform = new Form3(information);
            newform.ShowDialog();
            DisableButton();
        }

        public string takeTimePicked() //Lấy ra ngày/tháng/năm đang chọn (màu xanh)
        {
            string DateNow = "";
            string MonthNow = "";
            string DayNow = "";
            if (month >= 10)
            {
                MonthNow = month.ToString();
            }
            else
            {
                MonthNow = "0" + month.ToString();
            }
            if (DayPicked < 10)
            {
                DayNow = "0" + DayPicked.ToString();
            }
            else
            {
                DayNow = DayPicked.ToString();
            }
            if (DayPicked != 0)
            {
                DateNow = year + "-" + MonthNow + "-" + DayNow;
            }
            return DateNow;
        }

        #endregion
        #region TimePicked
        private void button_click(object sender, EventArgs e) //Các Radio Button chọn giờ
        {
            foreach (Guna2Button item in flowLayoutPanel1.Controls)
            {
                if (item.Checked == true)
                {
                    UpdateDay();
                    checkedbutton = item.Text;
                }
            }
            UpdateDay();
        }
        public DateTime ConvertToDateTime1(string dateString)
        {
            DateTime dateTime = DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return dateTime;
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
        #endregion
    }
}
