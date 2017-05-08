/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{

	using PeProvisioner = org.cloudbus.cloudsim.provisioners.PeProvisioner;

	/// <summary>
	/// Pe (Processing Element) class represents a CPU core of a physical machine (PM), 
	/// defined in terms of Millions Instructions Per Second (MIPS) rating.<br/>
	/// <b>ASSUMPTION:<b> All PEs under the same Machine have the same MIPS rating.
	/// @todo This assumption is not being assured on different class (where other TODOs where introduced)
	/// @todo Pe statuses have to be defined using an enum
	/// 
	/// @author Manzur Murshed
	/// @author Rajkumar Buyya
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class Pe
	{

		/// <summary>
		/// Denotes Pe is FREE for allocation. </summary>
		public const int FREE = 1;

		/// <summary>
		/// Denotes Pe is allocated and hence busy processing some Cloudlet. </summary>
		public const int BUSY = 2;

		/// <summary>
		/// Denotes Pe is failed and hence it can't process any Cloudlet at this moment. This Pe is
		/// failed because it belongs to a machine which is also failed.
		/// </summary>
		public const int FAILED = 3;

		/// <summary>
		/// The Pe id. </summary>
		private int id;

		/// <summary>
		/// The status of Pe: FREE, BUSY, FAILED: . </summary>
		private int status;

		/// <summary>
		/// The pe provisioner. </summary>
		private PeProvisioner peProvisioner;

		/// <summary>
		/// Instantiates a new Pe object.
		/// </summary>
		/// <param name="id"> the Pe ID </param>
		/// <param name="peProvisioner"> the pe provisioner
		/// @pre id >= 0
		/// @pre peProvisioner != null
		/// @post $none </param>
		public Pe(int id, PeProvisioner peProvisioner)
		{
			Id = id;
			PeProvisioner = peProvisioner;

			// when created it should be set to FREE, i.e. available for use.
			status = FREE;
		}

		/// <summary>
		/// Sets the id.
		/// </summary>
		/// <param name="id"> the new id </param>
		public virtual int Id
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
        /// Gets or sets the MIPS Rating of the current Pe.
        /// </summary>
        public virtual int Mips
        {
            get
            {
                return (int)PeProvisioner.Mips;
            }

            set
            {
                PeProvisioner.Mips = value;
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


		/// <summary>
		/// Sets the pe provisioner.
		/// </summary>
		/// <param name="peProvisioner"> the new pe provisioner </param>
		public virtual PeProvisioner PeProvisioner
		{
			set
			{
				this.peProvisioner = value;
			}
			get
			{
				return peProvisioner;
			}
		}


	}

}