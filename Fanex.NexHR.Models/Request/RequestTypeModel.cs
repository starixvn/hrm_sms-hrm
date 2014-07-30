// --------------------------------------------------------------------------
// <copyright file="RequestTypeModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;

    /// <summary>
    /// Request Type.
    /// </summary>
    public class RequestTypeModel : EntityBase<short>
    {
        public new string Name
        {
            get 
            {
                return Id.RequestTypeToText();
            }
        }

        public int RequestFilter
        {
            get 
            {
                switch (Id)
                {
                    case (int)RequestType.CheckIn:
                    case (int)RequestType.CheckOut:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.CheckInOut;
                    case (int)RequestType.LateIn:
                    case (int)RequestType.EarlyOut:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.LateInEarlyOut;
                    case (int)RequestType.ChangeShift:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.ChangeShift;
                    case (int)RequestType.Leave:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.Leave;
                    case (int)RequestType.KeepInView:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.KeepInView;
                    default:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.All;
                }
            }
        }

        public int RequestHistoryFilter
        {
            get
            {
                switch (Id)
                {
                    case (int)RequestType.CheckIn:
                    case (int)RequestType.CheckOut:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.CheckInOut;
                    case (int)RequestType.LateIn:
                    case (int)RequestType.EarlyOut:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.LateInEarlyOut;
                    case (int)RequestType.ChangeShift:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.ChangeShift;
                    case (int)RequestType.Leave:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.Leave;
                    case (int)RequestType.KeepInView:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.KeepInView;
                    default:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.All;
                }
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
