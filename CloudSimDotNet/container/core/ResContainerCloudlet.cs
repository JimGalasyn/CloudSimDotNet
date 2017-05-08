namespace org.cloudbus.cloudsim.container.core
{


	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ResContainerCloudlet : ResCloudlet
	{
		public ResContainerCloudlet(Cloudlet cloudlet) : base(cloudlet)
		{
		}

		public ResContainerCloudlet(Cloudlet cloudlet, long startTime, int duration, int reservID) : base(cloudlet, startTime, duration, reservID)
		{
		}


		public virtual int ContainerId
		{
			get
			{
				return ((ContainerCloudlet)Cloudlet).ContainerId;
			}
		}
	}

}