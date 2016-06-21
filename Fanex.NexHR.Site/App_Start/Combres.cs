// --------------------------------------------------------------------------
// <copyright file="Combres.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Created</change>
// </history>

[assembly: WebActivator.PreApplicationStartMethod(typeof(Fanex.NexHR.Site.AppStart.Combres), "PreStart")]

namespace Fanex.NexHR.Site.AppStart
{
    using System.Web.Routing;
    using global::Combres;

    /// <summary>
    /// COMBRES class.
    /// </summary>
    public static class Combres
    {
        public static void PreStart()
        {
            RouteTable.Routes.AddCombresRoute("Combres");
        }
    }
}