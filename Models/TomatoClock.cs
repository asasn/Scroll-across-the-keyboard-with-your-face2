using RootNS.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RootNS.Models
{
    public class TomatoClock : NotificationObject
    {
        public TomatoClock()
        {
            Loaded();
            PropertyChanged += Tomato_PropertyChanged;
        }

        private void Tomato_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TimeSetTotalMinutes))
            {
                TagChange = true;
                TimeSetTotalSeconds = TimeSetTotalMinutes * 60;
            }
        }

        private void Loaded()
        {
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            Timer.Tick += TimeRuner;
            ShowTimeText = String.Format("{0:D2}:{1:D2}", (int)StopWatch.Elapsed.TotalMinutes, StopWatch.Elapsed.Seconds);
        }


        #region 番茄时间
        private string _showTimeText;

        public string ShowTimeText
        {
            get { return _showTimeText; }
            set
            {
                _showTimeText = value;
                RaisePropertyChanged(nameof(ShowTimeText));
            }
        }

        private Stopwatch StopWatch = new Stopwatch();
        public DispatcherTimer Timer = new DispatcherTimer();

        bool TagChange = false;

        private Visibility _isSetting = Visibility.Collapsed;

        public Visibility IsSetting
        {
            get { return _isSetting; }
            set
            {
                _isSetting = value;
                RaisePropertyChanged(nameof(IsSetting));
            }
        }


        public MediaElement MeDida { get; set; }
        public MediaElement MeRing { get; set; }

        private int _timeSetTotalMinutes = 25;
        /// <summary>
        /// 闹钟总时间（分钟数）
        /// </summary>
        public int TimeSetTotalMinutes
        {
            get { return _timeSetTotalMinutes; }
            set
            {
                _timeSetTotalMinutes = value;
                RaisePropertyChanged(nameof(TimeSetTotalMinutes));
            }
        }

        private int _timeSetTotalSeconds = 25 * 60;
        /// <summary>
        /// 闹钟总时间（秒数）
        /// </summary>
        public int TimeSetTotalSeconds
        {
            get { return _timeSetTotalSeconds; }
            set
            {
                _timeSetTotalSeconds = value;
                RaisePropertyChanged(nameof(TimeSetTotalSeconds));
            }
        }

        private int _progressValue = 0;

        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
            }
        }



        private object _buttonIconPlay = "\ue9cf";//默认图标：play

        private object _buttonIconStop = "\ueb40";//图标：stop


        private object _buttonContent = "\ue9cf";//默认图标：play
        public object ButtonContent
        {
            get { return _buttonContent; }
            set
            {
                _buttonContent = value;
                RaisePropertyChanged(nameof(ButtonContent));
            }
        }


        public void Start()
        {
            if (Timer.IsEnabled)
            {
                ButtonContent = _buttonIconPlay;
                Stop();
            }
            else
            {
                ButtonContent = _buttonIconStop;
                if (TagChange == true)
                {
                    Settings.Set(Gval.MaterialBook, Gval.SettingsKeys.TomatoTimeSetTotalMinutes, TimeSetTotalMinutes);
                    TagChange = false;
                }

                IsSetting = Visibility.Collapsed;
                MeDida.Stop();
                MeRing.Stop();
                Timer.Start();
                StopWatch.Start();
            }
        }

        /// <summary>
        /// 方法：每次间隔运行的内容
        /// </summary>
        private void TimeRuner(object sender, EventArgs e)
        {
            MeDida.Stop();
            MeDida.Play();

            if ((int)StopWatch.Elapsed.TotalMinutes >= TimeSetTotalMinutes)
            {
                MeRing.Play();
                Start();
            }

            ShowTimeText = String.Format("{0:D2}:{1:D2}", (int)StopWatch.Elapsed.TotalMinutes, StopWatch.Elapsed.Seconds);
            ProgressValue = (int)StopWatch.Elapsed.TotalSeconds;
        }

        public void Stop()
        {
            StopWatch = new Stopwatch();
            Timer.Stop();
            ShowTimeText = String.Format("{0:D2}:{1:D2}", (int)StopWatch.Elapsed.TotalMinutes, StopWatch.Elapsed.Seconds);
            ProgressValue = (int)StopWatch.Elapsed.TotalSeconds;
        }

        public void Pause()
        {
            StopWatch.Stop();
            Timer.Stop();
            ShowTimeText = String.Format("{0:D2}:{1:D2}", (int)StopWatch.Elapsed.TotalMinutes, StopWatch.Elapsed.Seconds);
        }

        /// <summary>
        /// 事件：时间设置值更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CbTime_ValueChanged()
        {

        }

        /// <summary>
        /// 事件：时间设置值载入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CbTime_Loaded()
        {
            try
            {
                object value = Settings.Get(Gval.MaterialBook, Gval.SettingsKeys.TomatoTimeSetTotalMinutes);
                if (value != null && value.ToString() != "0")
                {
                    TimeSetTotalMinutes = Convert.ToInt16(value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

    }
}
