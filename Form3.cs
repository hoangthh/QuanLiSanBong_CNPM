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
    public partial class Form3 : Form
    {
        private string inputstring;
        private int ID;
        private string Date;
        private string Time;
        private void SolveInputInformation()
        {
            string[] strings = inputstring.Split(' ');
            if (strings != null && strings.Length <= 5)
            {
                ID = Convert.ToInt32(strings[0]);
                Date = strings[1];
                Time = strings[2] + strings[3] + strings[4];
            }
        }
        private void UpdateDatabase()
        {
            using(var db = new QuanLySanBongEntities())
            {
                int currentSanID = 1;
                DateTime date = ConvertToDateTime1(Date);
                Tuple<TimeSpan, TimeSpan> Timing = TimeSolving(Time);
                TimeSpan StartTime = Timing.Item1;
                TimeSpan EndTime = Timing.Item2;
                var lastBooking = db.Bookings
                                    .Where(b => b.BookingDate == date && b.StartTime == StartTime && b.EndTime == EndTime)
                                    .OrderByDescending(b => b.SanID)
                                    .FirstOrDefault();

                if (lastBooking != null)
                {
                    currentSanID = lastBooking.SanID + 1 ?? 1;
                }

                var PhieuDatSan = new Booking
                {
                    UserID = ID,
                    SanID = currentSanID,
                    BookingDate = date,
                    StartTime = StartTime,
                    EndTime = EndTime,
                    Status = "Da Dat"
                };

                db.Bookings.Add(PhieuDatSan);
                db.SaveChanges();
                MessageBox.Show("Thành công!");
            }
        }
        public DateTime ConvertToDateTime1(string dateString)
        {
            DateTime dateTime = DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            return dateTime;
        }
        public Form3(string input)
        {
            InitializeComponent();
            inputstring = input;
            SolveInputInformation();
            DataBinding();
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
        private void DataBinding()
        {
            using(var db = new QuanLySanBongEntities())
            {
                var User = db.Users.Where(p => p.UserID == ID).FirstOrDefault();
                textboxname.Text = User.UserName;
                textboxsdt.Text = User.PhoneNumber;
                textboxemail.Text = User.Email;
                textboxdate.Text = Date;
                textboxtime.Text = Time;
                textboxsanId.Text = "Sân 5";
            }
        }
        private void guna2Button17_Click(object sender, EventArgs e)
        {
            UpdateDatabase();
            this.Close();
        }
    }
}
