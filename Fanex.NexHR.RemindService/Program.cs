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
    using Fanex.NexHR.ReminderService.Services;
    using Topshelf;
    using Topshelf.HostConfigurators;

    public static class Program
    {
        public static void Main()
        {
            try
            {
                TopshelfExitCode exitCode = HostFactory.Run(hostConfigurator =>
                {
                    hostConfigurator.Service<ServiceController>(serviceConfigurator =>
                    {
                        serviceConfigurator.ConstructUsing(() => new ServiceController());
                        serviceConfigurator.WhenStarted(sv => sv.Start());
                        serviceConfigurator.WhenStopped(sv => sv.Stop());
                        serviceConfigurator.WhenPaused(sv => sv.Pause());
                        serviceConfigurator.WhenContinued(sv => sv.Continue());
                        serviceConfigurator.BeforeStartingService(() =>
                        {
                            LogControlService.WriteBeforeStartingService();
                        });
                        serviceConfigurator.AfterStoppingService(() => 
                        {
                            LogControlService.WriteAfterStoppingService();
                        });
                    });

                    RecoveryConfig(hostConfigurator);

                    InstallConfig(hostConfigurator);
                });

                if (exitCode != TopshelfExitCode.Ok)
                {
                    LogControlService.WriteError(exitCode.ToString());
                }
            }
            catch (Exception ex)
            {
                LogControlService.WriteException(ex);
            }
        }

        private static void InstallConfig(HostConfigurator hostConfigurator)
        {
            hostConfigurator.AfterInstall(() =>
            {
                LogControlService.WriteAfterInstall();
            });
            hostConfigurator.AfterUninstall(() =>
            {
                LogControlService.WriteAfterUninstall();
            });

            hostConfigurator.RunAsLocalSystem();
            hostConfigurator.EnablePauseAndContinue();

            hostConfigurator.SetDisplayName(ConfigSettings.ServiceDisplayName);
            hostConfigurator.SetDescription(ConfigSettings.ServiceDescription);
            hostConfigurator.SetServiceName(ConfigSettings.ServiceName);
        }

        private static void RecoveryConfig(HostConfigurator hostConfigurator)
        {
            hostConfigurator.EnableServiceRecovery(r =>
            {
                r.RestartService(0);
                r.RestartService(0);
                r.RestartService(0);
                r.OnCrashOnly();
                r.SetResetPeriod(1);
            });
        }
    }
}
