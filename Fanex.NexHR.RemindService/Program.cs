// --------------------------------------------------------------------------
// <copyright file="Program.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.6">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService
{
    using System;
    using Castle.MicroKernel.Registration;
    using Fanex.Data.Configuration;
    using Fanex.NexHR.ReminderService.Services;
    using Fanex.WindowsService;

    public static class Program
    {
        public static void Main()
        {
            try
            {
                DbSettingProviderManager.Current.Start();

                var container = WindowsService.InstallWindsor();
                container.Register(Component.For<IService>()
                       .UsingFactoryMethod<IService>(() => new ReminderAttendanceService()).LifestyleTransient());

                WindowsService.Run(container);
            }
            catch (Exception ex)
            {
                LogControlService.WriteException(ex);
            }
        }
    }
}
