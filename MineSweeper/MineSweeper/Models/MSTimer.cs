﻿using System;
using Xamarin.Forms;

namespace MineSweeper.Models
{
    public class MSTimer
    {
        private TimeSpan timer = new TimeSpan(0, 0, 0);
        public bool IsPaused { get; set; } = false;
        public string Timer
        {
            get => (timer.Minutes > 9 ? "" : "0") + timer.Minutes.ToString() + ":" +
                (timer.Seconds > 9 ? "" : "0") + timer.Seconds.ToString();
        }

        public void Start(Label _timer)
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (!IsPaused)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        timer = timer.Add(new TimeSpan(0, 0, 1));
                        _timer.Text = Timer;
                    });
                }

                return true;
            });
        }

        public void Reset()
        {
            timer = new TimeSpan(0, 0, 0);
        }

    }
}
