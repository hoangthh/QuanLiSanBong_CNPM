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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace QuanLyDatLichSanBong
{
    public partial class Form4 : Form
    {
        private static string InputInfor;
        private static DateTime Date;
        private static Tuple<TimeSpan, TimeSpan> Time;
        private static string TimeStartEnd;
        private static int SanID;
        public Form4(string input)
        {
            InitializeComponent();
            InputInfor = input;
            SolveInputString();
            DataBinding();
        }
        //private string SolveDate(DateTime date) 
        //{
        //    string[] dates = date.ToString().Split(',');
        //}
        private void DataBinding()
        {
            using(var db = new QuanLySanBongEntities())
            {
                var PhieuBooking = db.Bookings.Where(p => p.BookingDate == Date && p.SanID == SanID
                && p.StartTime == Time.Item1 && p.EndTime == Time.Item2).FirstOrDefault();
                if (PhieuBooking == null) return;
                int UserID = PhieuBooking.UserID ?? 1;
                var User = db.Users.Where(p => p.UserID == UserID).FirstOrDefault();
                textboxname.Text = User.UserName;
                textboxsdt.Text = User.PhoneNumber;
                textboxemail.Text = User.Email;
                textboxdate.Text = Date.ToShortDateString().ToString();
                textboxtime.Text = TimeStartEnd;
                textboxsanId.Text = SanID.ToString();
            }
        }
        private void SolveInputString()
        {
            string[] strings = InputInfor.Split(' '); //4/18/2024 19:30 - 21:00 1
            if (strings != null && strings.Length <= 5)
            {
                Date = ConvertToDateTime(strings[0]);
                TimeStartEnd = strings[1] + strings[2] + strings[3];
                Time = TimeSolving(TimeStartEnd);
                SanID = Convert.ToInt32(strings[4]);
            }
        }
        public DateTime ConvertToDateTime(string dateString)
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
        private Tuple<TimeSpan, TimeSpan> TimeSolving(string TimeText)
        {
            string[] parts = TimeText.Split('-');
            string startTimeString = parts[0].Trim();
            string endTimeString = parts[1].Trim();
            TimeSpan startTimeSpan = TimeSpan.Parse(startTimeString);
            TimeSpan endTimeSpan = TimeSpan.Parse(endTimeString);
            return new Tuple<TimeSpan, TimeSpan>(startTimeSpan, endTimeSpan);
        }

        private void guna2Button17_Click(object sender, EventArgs e)
        {
            using (var db = new QuanLySanBongEntities())
            {
                var PhieuBooking = db.Bookings.Where(p => p.BookingDate == Date && p.SanID == SanID
                && p.StartTime == Time.Item1 && p.EndTime == Time.Item2).FirstOrDefault();
                if (PhieuBooking == null) return;
                int UserID = PhieuBooking.UserID ?? 1;
                var User = db.Users.Where(p => p.UserID == UserID).FirstOrDefault();
                User.UserName = textboxname.Text.ToString();
                PhieuBooking.BookingDate = ConvertToDateTime(textboxdate.Text);
                PhieuBooking.SanID = Convert.ToInt32(textboxsanId.Text);
                PhieuBooking.StartTime = TimeSolving(textboxtime.Text).Item1;
                PhieuBooking.EndTime = TimeSolving(textboxtime.Text).Item2;
                db.SaveChanges();
                MessageBox.Show("Cập Nhật Thành Công!");
                this.Close();
            }
        }
    }
}
