// --------------------------------------------------------------------------
// <copyright file="ServiceController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.6">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using Fanex.NexHR.ReminderService.Services;

    public class ServiceController : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private readonly ReminderAttendanceService _reminderService;

        private readonly System.Timers.Timer _timer;
        private Task _task;
        private bool _isBusy;

        public ServiceController()
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = ConfigSettings.TimerInterval * 1000;
            _timer.Elapsed += TimerElapsed;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _reminderService = new ReminderAttendanceService();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _task = new Task(DoWork, _cancellationToken);
            _task.Start();
        }

        public void Start()
        {
            _timer.Enabled = true;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Enabled = false;
            _cancellationTokenSource.Cancel();
            _task.Wait();
        }

        public void Pause()
        {
            _timer.Enabled = false;
            LogControlService.WriteWhenPaused();
        }

        public void Continue()
        {
            _timer.Enabled = true;
            LogControlService.WhenContinued();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed. Suppression is OK here.")]
        private void DoWork()
        {
            if (!_cancellationTokenSource.IsCancellationRequested && (!_isBusy || (ConfigSettings.IsAsynchronous && _isBusy)))
            {
                _isBusy = true;
                try
                {
                    _reminderService.Remind();
                }
                catch (Exception ex)
                {
                    LogControlService.WriteException(ex);
                }

                _isBusy = false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
                _task.Dispose();
                _cancellationTokenSource.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
