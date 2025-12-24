namespace Client
{
    public class AttackAnimation
    {
        private Form parentForm;
        private int attackCount;
        private Timer timer;
        private int step = 0;

        public AttackAnimation(Form parent)
        {
            parentForm = parent;
        }

        public void ShowAttack(int soldiers)
        {
            attackCount = soldiers;
            step = 0;

            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            step++;
            if (step > 10)
            {
                timer.Stop();
                timer.Dispose();
            }
            else
            {
                parentForm.Invalidate();
            }
        }
    }
}
