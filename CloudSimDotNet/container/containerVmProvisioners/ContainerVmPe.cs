namespace org.cloudbus.cloudsim.container.containerVmProvisioners
{


	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerVmPe
	{

		/// <summary>
		/// Denotes Pe is FREE for allocation.
		/// </summary>
		public const int FREE = 1;

		/// <summary>
		/// Denotes Pe is allocated and hence busy in processing Cloudlet.
		/// </summary>
		public const int BUSY = 2;

		/// <summary>
		/// Denotes Pe is failed and hence it can't process any Cloudlet at this moment. This Pe is
		/// failed because it belongs to a machine which is also failed.
		/// </summary>
		public const int FAILED = 3;

		/// <summary>
		/// The id.
		/// </summary>
		private int id;

		// FOR SPACE SHARED RESOURCE: Jan 21
		/// <summary>
		/// The status of Pe: FREE, BUSY, FAILED: .
		/// </summary>
		private int status;

		/// <summary>
		/// The pe provisioner.
		/// </summary>
		private ContainerVmPeProvisioner containerVmPeProvisioner;

		/// <summary>
		/// Allocates a new Pe object.
		/// </summary>
		/// <param name="id">            the Pe ID </param>
		/// <param name="containerVmPeProvisioner"> the pe provisioner
		/// @pre id >= 0
		/// @pre peProvisioner != null
		/// @post $none </param>
		public ContainerVmPe(int id, ContainerVmPeProvisioner containerVmPeProvisioner)
		{
			Id = id;
			ContainerVmPeProvisioner = containerVmPeProvisioner;

			// when created it should be set to FREE, i.e. available for use.
			status = FREE;
		}

		/// <summary>
		/// Sets the id.
		/// </summary>
		/// <param name="id"> the new id </param>
		protected internal virtual int Id
		{
			set
			{
				this.id = value;
			}
			get
			{
				return id;
			}
		}


		/// <summary>
		/// Sets the MIPS Rating of this Pe.
		/// </summary>
		/// <param name="d"> the mips
		/// @pre mips >= 0
		/// @post $none </param>
		public virtual void setMips(double d)
		{
			ContainerVmPeProvisioner.Mips = d;
		}

		/// <summary>
		/// Gets the MIPS Rating of this Pe.
		/// </summary>
		/// <returns> the MIPS Rating
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int Mips
		{
            get
            {
                return (int)ContainerVmPeProvisioner.Mips;
            }
        }

		/// <summary>
		/// Gets the status of this Pe.
		/// </summary>
		/// <returns> the status of this Pe
		/// @pre $none
		/// @post $none </returns>
		public virtual int Status
		{
			get
			{
				return status;
			}
			set
			{
				this.status = value;
			}
		}

		/// <summary>
		/// Sets Pe status to free, meaning it is available for processing. This should be used by SPACE
		/// shared hostList only.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void setStatusFree()
		{
			Status = FREE;
		}

		/// <summary>
		/// Sets Pe status to busy, meaning it is already executing Cloudlets. This should be used by
		/// SPACE shared hostList only.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void setStatusBusy()
		{
			Status = BUSY;
		}

		/// <summary>
		/// Sets this Pe to FAILED.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void setStatusFailed()
		{
			Status = FAILED;
		}

		public virtual ContainerVmPeProvisioner ContainerVmPeProvisioner
		{
			get
			{
				return containerVmPeProvisioner;
			}
			set
			{
				this.containerVmPeProvisioner = value;
			}
		}
	}



}