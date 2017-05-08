using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

	using ContainerVmBwProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmBwProvisioner;
	using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
	using ContainerVmRamProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmRamProvisioner;
	using ContainerVmScheduler = org.cloudbus.cloudsim.container.schedulers.ContainerVmScheduler;
	using PowerModel = org.cloudbus.cloudsim.power.models.PowerModel;
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;

	/// <summary>
	/// Created by sareh on 15/07/15.
	/// </summary>
	public class PowerContainerHostUtilizationHistory : PowerContainerHost
	{

		/// <summary>
		/// Instantiates a new power host utilization history.
		/// </summary>
		/// <param name="id">             the id </param>
		/// <param name="ramProvisioner"> the ram provisioner </param>
		/// <param name="bwProvisioner">  the bw provisioner </param>
		/// <param name="storage">        the storage </param>
		/// <param name="peList">         the pe list </param>
		/// <param name="vmScheduler">    the vm scheduler </param>
		/// <param name="powerModel">     the power model </param>
		public PowerContainerHostUtilizationHistory(int id, ContainerVmRamProvisioner ramProvisioner, ContainerVmBwProvisioner bwProvisioner, long storage, IList<ContainerVmPe> peList, ContainerVmScheduler vmScheduler, PowerModel powerModel) : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler, powerModel)
		{
		}

		/// <summary>
		/// Gets the host utilization history.
		/// </summary>
		/// <returns> the host utilization history </returns>
		public virtual double[] UtilizationHistory
		{
			get
			{
				double[] utilizationHistory = new double[PowerContainerVm.HISTORY_LENGTH];
				double hostMips = TotalMips;
				foreach (PowerContainerVm vm in VmListProperty)
				{
					for (int i = 0; i < vm.UtilizationHistory.Count; i++)
					{
						utilizationHistory[i] += vm.UtilizationHistory[i].Value * vm.Mips / hostMips;
					}
				}
				return MathUtil.trimZeroTail(utilizationHistory);
			}
		}

	}


}