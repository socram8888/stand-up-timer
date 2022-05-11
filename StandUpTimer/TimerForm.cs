using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace StandUpTimer
{
    public partial class TimerForm : Form
    {
        private DateTime StartTime;
        private readonly Timer UpdateTimer = new Timer();
        private readonly Timer ResetButtonEnableTimer = new Timer();
        private readonly TimeSpan STAND_UP_TIME = TimeSpan.FromMinutes(30);

        public TimerForm()
        {
            InitializeComponent();

            UpdateTimer.Interval = 100;
            UpdateTimer.Tick += new EventHandler(OnUpdateTimer);

            ResetButtonEnableTimer.Interval = 3000;
            ResetButtonEnableTimer.Tick += new EventHandler(OnResetButtonEnableTimer);
        }

        private void TimerForm_Load(object sender, EventArgs e)
        {
            StartTime = DateTime.Now;
            UpdateTimer.Start();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            StartTime = DateTime.Now;
            TimeLabel.ForeColor = DefaultForeColor;
            UpdateTimer.Start();
        }

        private void OnUpdateTimer(object sender, EventArgs e)
        {
            var diff = STAND_UP_TIME - (DateTime.Now - StartTime);

            int totalSecs = (int)Math.Ceiling(diff.TotalSeconds);
            if (totalSecs < 0)
                totalSecs = 0;

            int minutes = totalSecs / 60;
            int secs = totalSecs % 60;

            TimeLabel.Text = $"{minutes:00}:{secs:00}";

            if (totalSecs == 0)
            {
                TimeLabel.ForeColor = Color.Red;
                UpdateTimer.Stop();

                // Disable briefly, to prevent the reset button from being accidentally pressed if the user was typing
                ResetButton.Enabled = false;
                ResetButtonEnableTimer.Start();

                // Bring to foreground
                Activate();

                // Play tone to notify
                var dingPlayer = new SoundPlayer(ProgramResources.ding);
                dingPlayer.Play();
            }
        }

        private void OnResetButtonEnableTimer(object sender, EventArgs e)
        {
            ResetButton.Enabled = true;
            ResetButtonEnableTimer.Stop();
        }
    }
}
