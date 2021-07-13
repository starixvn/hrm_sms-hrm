namespace HRM.UserControl.Client.Core
{
    using System;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Session;

    public class UserControlClientManager
    {
        private IUCSession ucSession;
        private IUserControlSettings userControlSettings;
        private static UserControlClientManager CurrentInstance = new UserControlClientManager();

        public static UserControlClientManager UseSettings(IUserControlSettings settings)
        {
            CurrentInstance.userControlSettings = settings ?? throw new ArgumentNullException(nameof(settings));

            return CurrentInstance;
        }

        public UserControlClientManager UseSession(IUCSession userControlSession)
        {
            CurrentInstance.ucSession = userControlSession ?? throw new ArgumentNullException(nameof(userControlSession));

            return CurrentInstance;
        }

        public static IUserControlSettings UserControlSettings
            => CurrentInstance.userControlSettings ?? throw new InvalidOperationException(nameof(CurrentInstance.userControlSettings));

        public static IUCSession UserControlSession
            => CurrentInstance.ucSession ?? throw new InvalidOperationException(nameof(CurrentInstance.ucSession));
    }
}