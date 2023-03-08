namespace GO.AutoJobs.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GO.AutoJobs.Models;

    public static class JobHelper
    {
        public static JobSettings GetJobConfig(IEnumerable<JobSettings> jobConfigs, string jobName)
        {
            return jobConfigs.FirstOrDefault(job => job.JobName.Equals(jobName, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsEnabledJob(IEnumerable<JobSettings> jobConfigs, string jobName)
        {
            var settings = GetJobConfig(jobConfigs, jobName);

            return settings != null && settings.IsEnabled;
        }
    }

}