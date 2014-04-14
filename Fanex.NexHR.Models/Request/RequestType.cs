// --------------------------------------------------------------------------
// <copyright file="RequestType.cs" company="Nexcel Solutions Vietnam">
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
    using Resources;
    
    /// <summary>
    /// Request Type.
    /// </summary>
    public class RequestType : EntityBase<short>
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
                    case (int)RequestTypes.CheckIn:
                    case (int)RequestTypes.CheckOut:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.CheckInOut;
                    case (int)RequestTypes.LateIn:
                    case (int)RequestTypes.EarlyOut:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.LateInEarlyOut;
                    case (int)RequestTypes.ChangeShift:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.ChangeShift;
                    case (int)RequestTypes.Leave:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.Leave;
                    case (int)RequestTypes.KeepInView:
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
                    case (int)RequestTypes.CheckIn:
                    case (int)RequestTypes.CheckOut:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.CheckInOut;
                    case (int)RequestTypes.LateIn:
                    case (int)RequestTypes.EarlyOut:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.LateInEarlyOut;
                    case (int)RequestTypes.ChangeShift:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.ChangeShift;
                    case (int)RequestTypes.Leave:
                        return (int)Fanex.NexHR.Core.Enums.RequestFilter.Leave;
                    case (int)RequestTypes.KeepInView:
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
