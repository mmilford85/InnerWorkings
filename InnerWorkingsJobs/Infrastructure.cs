using InnerWorkingsJobs.Jobs;

using Microsoft.Practices.Unity.Configuration;

using Unity;

namespace InnerWorkingsJobs
{
    public class Infrastructure
    {
        private static UnityContainer _unityContainer;

        public static IJobsService GetJobsService()
        {
            return UnityContainer.Resolve<IJobsService>();
        }

        public static UnityContainer UnityContainer
        {
            get
            {
                if (_unityContainer == null)
                {
                    _unityContainer = new UnityContainer();
                    _unityContainer.LoadConfiguration();
                }

                return _unityContainer;
            }
        }
    }
}
