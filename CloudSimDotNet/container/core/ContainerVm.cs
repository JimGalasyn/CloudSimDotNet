using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

    using ContainerBwProvisioner = org.cloudbus.cloudsim.container.containerProvisioners.ContainerBwProvisioner;
    using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
    using ContainerRamProvisioner = org.cloudbus.cloudsim.container.containerProvisioners.ContainerRamProvisioner;
    using ContainerPeList = org.cloudbus.cloudsim.container.lists.ContainerPeList;
    using ContainerScheduler = org.cloudbus.cloudsim.container.schedulers.ContainerScheduler;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;


    /// <summary>
    /// Vm represents a VM: it runs inside a Host, sharing hostList with other VMs. It processes
    /// containers. This processing happens according to a policy, defined by the containerscheduler. Each
    /// VM has a owner, which can submit containers to the VM to be executed
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @author Sareh Fotuhi Piraghaj
    /// @since CloudSim Toolkit 1.0
    /// <p/>
    /// Created by sareh on 9/07/15.
    /// </summary>
    public class ContainerVm
    {

        /// <summary>
        /// The user id.
        /// </summary>
        private int userId;

        /// <summary>
        /// The uid.
        /// </summary>
        private string uid;

        /// <summary>
        /// The size.
        /// </summary>
        private long size;

        /// <summary>
        /// The MIPS.
        /// </summary>
        private double mips;

        /// <summary>
        /// The number of PEs.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") private int numberOfPes;
        private int numberOfPes;

        /// <summary>
        /// The ram.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") private float ram;
        private float ram;

        /// <summary>
        /// The bw.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") private long bw;
        private long bw;


        /// <summary>
        /// The vmm.
        /// </summary>
        private string vmm;

        /// <summary>
        /// The Cloudlet scheduler.
        /// </summary>
        private ContainerScheduler containerScheduler;

        /// <summary>
        /// The host.
        /// </summary>
        private ContainerHost host;

        /// <summary>
        /// In migration flag.
        /// </summary>
        private bool inMigration;
        /// <summary>
        /// In waiting flag. shows that vm is waiting for containers to come.
        /// </summary>
        private bool inWaiting;

        /// <summary>
        /// The current allocated size.
        /// </summary>
        private long currentAllocatedSize;

        /// <summary>
        /// The current allocated ram.
        /// </summary>
        private float currentAllocatedRam;

        /// <summary>
        /// The current allocated bw.
        /// </summary>
        private long currentAllocatedBw;

        /// <summary>
        /// The current allocated mips.
        /// </summary>
        private IList<double?> currentAllocatedMips;

        /// <summary>
        /// The VM is being instantiated.
        /// </summary>
        private bool beingInstantiated;

        /// <summary>
        /// The mips allocation history.
        /// </summary>
        private readonly IList<VmStateHistoryEntry> stateHistory = new List<VmStateHistoryEntry>();


        /// <summary>
        /// The id.
        /// </summary>
        private int id;

        /// <summary>
        /// The storage.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") private long storage;
        // TEST: (fixed) Never used.
        //private long storage;

        /// <summary>
        /// The ram provisioner.
        /// </summary>
        private ContainerRamProvisioner containerRamProvisioner;

        /// <summary>
        /// The bw provisioner.
        /// </summary>
        private ContainerBwProvisioner containerBwProvisioner;

        /// <summary>
        /// The vm list.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //ORIGINAL LINE: private final java.util.List<? extends Container> containerList = new java.util.ArrayList<>();
        private readonly IList<Container> containerList = new List<Container>();

        /// <summary>
        /// The pe list.
        /// </summary>
        //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe> peList;
        private IList<ContainerPe> peList;

        /// <summary>
        /// Tells whether this machine is working properly or has failed.
        /// </summary>
        private bool failed;

        /// <summary>
        /// The vms migrating in.
        /// </summary>
        private readonly IList<Container> containersMigratingIn = new List<Container>();

        /// <summary>
        /// The datacenter where the host is placed.
        /// </summary>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unused") private ContainerDatacenter datacenter;
        // TEST: (fixed) Never used.
        //private ContainerDatacenter datacenter;


        /// <summary>
        /// Creates a new VMCharacteristics object. </summary>
        /// <param name="id"> </param>
        /// <param name="userId"> </param>
        /// <param name="mips"> </param>
        /// <param name="ram"> </param>
        /// <param name="bw"> </param>
        /// <param name="size"> </param>
        /// <param name="vmm"> </param>
        /// <param name="containerScheduler"> </param>
        /// <param name="containerRamProvisioner"> </param>
        /// <param name="containerBwProvisioner"> </param>
        /// <param name="peList"> </param>

        //public ContainerVm<T1>(int id, int userId, double mips, float ram, long bw, long size, string vmm, ContainerScheduler containerScheduler, ContainerRamProvisioner containerRamProvisioner, ContainerBwProvisioner containerBwProvisioner, IList<T1> peList) where T1 : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public ContainerVm(int id, int userId, double mips, float ram, long bw, long size, string vmm, ContainerScheduler containerScheduler, ContainerRamProvisioner containerRamProvisioner, ContainerBwProvisioner containerBwProvisioner, IList<ContainerPe> peList)
        {
            Id = id;
            UserId = userId;
            Uid = getUid(userId, id);
            Mips = mips;
            PeListProperty = peList;
            NumberOfPes = PeListProperty.Count;
            Ram = ram;
            Bw = bw;
            Size = size;
            Vmm = vmm;
            ContainerScheduler = containerScheduler;

            InMigration = false;
            InWaiting = false;
            BeingInstantiated = true;

            CurrentAllocatedBw = 0;
            CurrentAllocatedMips = null;
            CurrentAllocatedRam = 0;
            CurrentAllocatedSize = 0;

            ContainerRamProvisioner = containerRamProvisioner;
            ContainerBwProvisioner = containerBwProvisioner;


        }



        /// <summary>
        /// Updates the processing of containers running on this VM.
        /// </summary>
        /// <param name="currentTime"> current simulation time </param>
        /// <param name="mipsShare">   array with MIPS share of each Pe available to the scheduler </param>
        /// <returns> time predicted completion time of the earliest finishing cloudlet, or 0 if there is no
        /// next events
        /// @pre currentTime >= 0
        /// @post $none </returns>
        public virtual double updateVmProcessing(double currentTime, IList<double?> mipsShare)
        {
            //        Log.printLine("Vm: update Vms Processing at " + currentTime);
            if (mipsShare != null && ContainerListProperty.Count != 0)
            {
                double smallerTime = double.MaxValue;
                //            Log.printLine("ContainerVm: update Vms Processing");
                //            Log.printLine("The VM list size is:...." + getContainerList().size());

                foreach (Container container in ContainerListProperty)
                {
                    double time = container.updateContainerProcessing(currentTime, ContainerSchedulerProperty.getAllocatedMipsForContainer(container));
                    if (time > 0.0 && time < smallerTime)
                    {
                        smallerTime = time;
                    }
                }
                //            Log.printLine("ContainerVm: The Smaller time is:......" + smallerTime);

                return smallerTime;
            }
            //        if (mipsShare != null) {
            //            return getContainerScheduler().updateVmProcessing(currentTime, mipsShare);
            //        }
            return 0.0;
        }

        /// <summary>
        /// Gets the current requested mips.
        /// </summary>
        /// <returns> the current requested mips </returns>
        public virtual IList<double?> CurrentRequestedMips
        {
            get
            {

                double requestedMipsTemp = 0;


                if (BeingInstantiated)
                {

                    requestedMipsTemp = Mips;
                }
                else
                {
                    foreach (Container contianer in ContainerListProperty)
                    {

                        IList<double?> containerCurrentRequestedMips = contianer.CurrentRequestedMips;
                        // TEST: (fixed) decide on correct logic for double? type.
                        //requestedMipsTemp += containerCurrentRequestedMips[0] * containerCurrentRequestedMips.Count;
                        requestedMipsTemp += containerCurrentRequestedMips[0].Value * containerCurrentRequestedMips.Count;
                        //                Log.formatLine(
                        //                        " [Container #%d] utilization is %.2f",
                        //                        contianer.getId() ,
                        //                        contianer.getCurrentRequestedMips().get(0) * contianer.getCurrentRequestedMips().size() );

                    }
                    //            Log.formatLine("Total mips usage is %.2f", requestedMipsTemp);
                }

                IList<double?> currentRequestedMips = new List<double?>(NumberOfPes);

                for (int i = 0; i < NumberOfPes; i++)
                {
                    currentRequestedMips.Add(requestedMipsTemp);
                }
                //Log.printLine("Vm: get Current requested Mips" + currentRequestedMips);
                return currentRequestedMips;
            }
        }

        /// <summary>
        /// Gets the current requested total mips.
        /// </summary>
        /// <returns> the current requested total mips </returns>
        public virtual double CurrentRequestedTotalMips
        {
            get
            {
                double totalRequestedMips = 0;
                foreach (double mips in CurrentRequestedMips)
                {
                    totalRequestedMips += mips;
                }
                //Log.printLine("Container: get Current totalRequestedMips" + totalRequestedMips);
                return totalRequestedMips;
            }
        }

        /// <summary>
        /// Gets the current requested max mips among all virtual PEs.
        /// </summary>
        /// <returns> the current requested max mips </returns>
        public virtual double CurrentRequestedMaxMips
        {
            get
            {
                double maxMips = 0;
                foreach (double mips in CurrentRequestedMips)
                {
                    if (mips > maxMips)
                    {
                        maxMips = mips;
                    }
                }
                //Log.printLine("Container: get Current RequestedMaxMips" + maxMips);
                return maxMips;
            }
        }

        /// <summary>
        /// Gets the current requested bw.
        /// </summary>
        /// <returns> the current requested bw </returns>
        public virtual long CurrentRequestedBw
        {
            get
            {

                if (BeingInstantiated)
                {
                    return Bw;
                }
                else
                {

                    long requestedBwTemp = 0;

                    foreach (Container container in ContainerListProperty)
                    {
                        requestedBwTemp += container.CurrentRequestedBw;

                    }

                    //Log.printLine("Vm: get Current requested Mips" + requestedBwTemp);
                    return requestedBwTemp;


                }
            }
        }

        /// <summary>
        /// Gets the current requested ram.
        /// </summary>
        /// <returns> the current requested ram </returns>
        public virtual float CurrentRequestedRam
        {
            get
            {
                if (BeingInstantiated)
                {
                    return Ram;
                }
                else
                {

                    float requestedRamTemp = 0;

                    foreach (Container container in ContainerListProperty)
                    {
                        requestedRamTemp += container.CurrentRequestedRam;

                    }

                    //Log.printLine("Vm: get Current requested Mips" + requestedRamTemp);
                    return requestedRamTemp;

                }
            }
        }

        /// <summary>
        /// Get utilization created by all clouddlets running on this Container.
        /// </summary>
        /// <param name="time"> the time </param>
        /// <returns> total utilization </returns>
        public virtual double getTotalUtilizationOfCpu(double time)
        {
            float TotalUtilizationOfCpu = 0;

            foreach (Container container in ContainerListProperty)
            {
                // TEST: (fixed) cast to float appropriate?
                //TotalUtilizationOfCpu += container.getTotalUtilizationOfCpu(time);
                TotalUtilizationOfCpu += (float)container.getTotalUtilizationOfCpu(time);

            }

            //Log.printLine("Vm: get Current requested Mips" + TotalUtilizationOfCpu);
            return TotalUtilizationOfCpu;


        }

        /// <summary>
        /// Get utilization created by all containers running on this Container in MIPS.
        /// </summary>
        /// <param name="time"> the time </param>
        /// <returns> total utilization </returns>
        public virtual double getTotalUtilizationOfCpuMips(double time)
        {
            //Log.printLine("Container: get Current getTotalUtilizationOfCpuMips" + getTotalUtilizationOfCpu(time) * getMips());
            return getTotalUtilizationOfCpu(time) * Mips;
        }

        /// <summary>
        /// Sets the uid.
        /// </summary>
        /// <param name="uid"> the new uid </param>
        public virtual string Uid
        {
            set
            {
                this.uid = value;
            }
            get
            {
                return uid;
            }
        }


        /// <summary>
        /// Generate unique string identificator of the Container.
        /// </summary>
        /// <param name="userId"> the user id </param>
        /// <param name="vmId">   the vm id </param>
        /// <returns> string uid </returns>
        public static string getUid(int userId, int vmId)
        {
            return userId + "-" + vmId;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <returns> the id </returns>
        public virtual int Id
        {
            get
            {
                return id;
            }
            set
            {
                this.id = value;
            }
        }


        /// <summary>
        /// Sets the user id.
        /// </summary>
        /// <param name="userId"> the new user id </param>
        protected internal virtual int UserId
        {
            set
            {
                this.userId = value;
            }
            get
            {
                return userId;
            }
        }


        /// <summary>
        /// Gets the mips.
        /// </summary>
        /// <returns> the mips </returns>
        public virtual double Mips
        {
            get
            {
                return mips;
            }
            set
            {
                this.mips = value;
            }
        }



        /// <summary>
        /// Sets the number of pes.
        /// </summary>
        /// <param name="numberOfPes"> the new number of pes </param>
        protected internal virtual int NumberOfPes
        {
            set
            {
                this.numberOfPes = value;
            }
            get
            {
                //        Log.printLine("ContainerVm: get the PeList Size......" + getPeList().size());
                return PeListProperty.Count;
            }
        }


        /// <summary>
        /// Sets the amount of ram.
        /// </summary>
        /// <param name="ram"> new amount of ram
        /// @pre ram > 0
        /// @post $none </param>
        public virtual float Ram
        {
            get
            {
                // TODO: Not ContainerRamProvisioner.Ram? 
                return this.ram;
            }
            set
            {
                this.ram = value;
            }
        }


        /// <summary>
        /// Sets the amount of bandwidth.
        /// </summary>
        /// <param name="bw"> new amount of bandwidth
        /// @pre bw > 0
        /// @post $none </param>
        public virtual long Bw
        {
            get
            {
                // TODO: Not ContainerBwProvisioner.Bw?
                return this.bw;
            }
            set
            {
                this.bw = value;
            }
        }

        /// <summary>
        /// Gets the amount of storage.
        /// </summary>
        /// <returns> amount of storage
        /// @pre $none
        /// @post $none </returns>
        public virtual long Size
        {
            get
            {
                return size;
            }
            set
            {
                this.size = value;
            }
        }


        /// <summary>
        /// Gets the VMM.
        /// </summary>
        /// <returns> VMM
        /// @pre $none
        /// @post $none </returns>
        public virtual string Vmm
        {
            get
            {
                return vmm;
            }
            set
            {
                this.vmm = value;
            }
        }


        /// <summary>
        /// Sets the host that runs this VM.
        /// </summary>
        /// <param name="host"> Host running the VM
        /// @pre host != $null
        /// @post $none </param>
        public virtual ContainerHost Host
        {
            set
            {
                this.host = value;
            }
            get
            {
                return host;
            }
        }



        /// <summary>
        /// Sets the vm scheduler.
        /// </summary>
        /// <param name="containerScheduler"> the new vm scheduler </param>
        public virtual ContainerScheduler ContainerScheduler
        {
            get
            {
                return this.containerScheduler;
            }
            set
            {
                this.containerScheduler = value;
            }
        }

        /// <summary>
        /// Checks if is in migration.
        /// </summary>
        /// <returns> true, if is in migration </returns>
        public virtual bool InMigration
        {
            get
            {
                return inMigration;
            }
            set
            {
                this.inMigration = value;
            }
        }


        /// <summary>
        /// Gets the current allocated size.
        /// </summary>
        /// <returns> the current allocated size </returns>
        public virtual long CurrentAllocatedSize
        {
            get
            {
                return currentAllocatedSize;
            }
            set
            {
                this.currentAllocatedSize = value;
            }
        }


        /// <summary>
        /// Gets the current allocated ram.
        /// </summary>
        /// <returns> the current allocated ram </returns>
        public virtual float CurrentAllocatedRam
        {
            get
            {
                return currentAllocatedRam;
            }
            set
            {
                this.currentAllocatedRam = value;
            }
        }


        /// <summary>
        /// Gets the current allocated bw.
        /// </summary>
        /// <returns> the current allocated bw </returns>
        public virtual long CurrentAllocatedBw
        {
            get
            {
                return currentAllocatedBw;
            }
            set
            {
                this.currentAllocatedBw = value;
            }
        }


        /// <summary>
        /// Gets the current allocated mips.
        /// </summary>
        /// <returns> the current allocated mips </returns>
        public virtual IList<double?> CurrentAllocatedMips
        {
            get
            {
                return currentAllocatedMips;
            }
            set
            {
                this.currentAllocatedMips = value;
            }
        }


        /// <summary>
        /// Checks if is being instantiated.
        /// </summary>
        /// <returns> true, if is being instantiated </returns>
        public virtual bool BeingInstantiated
        {
            get
            {
                return beingInstantiated;
            }
            set
            {
                this.beingInstantiated = value;
            }
        }


        /// <summary>
        /// Gets the state history.
        /// </summary>
        /// <returns> the state history </returns>
        public virtual IList<VmStateHistoryEntry> StateHistory
        {
            get
            {
                return stateHistory;
            }
        }

        /// <summary>
        /// Adds the state history entry.
        /// </summary>
        /// <param name="time">          the time </param>
        /// <param name="allocatedMips"> the allocated mips </param>
        /// <param name="requestedMips"> the requested mips </param>
        /// <param name="isInMigration"> the is in migration </param>
        public virtual void addStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isInMigration)
        {
            VmStateHistoryEntry newState = new VmStateHistoryEntry(time, allocatedMips, requestedMips, isInMigration);
            if (StateHistory.Count > 0)
            {
                VmStateHistoryEntry previousState = StateHistory[StateHistory.Count - 1];
                if (previousState.Time == time)
                {
                    StateHistory[StateHistory.Count - 1] = newState;
                    return;
                }
            }
            StateHistory.Add(newState);
        }

        /// <summary>
        /// Adds the migrating in vm.
        /// </summary>
        /// <param name="container"> the vm </param>
        public virtual void addMigratingInContainer(Container container)
        {
            //        Log.printLine("ContainerVm: addMigratingInContainer:......");
            container.InMigration = true;

            if (!ContainersMigratingIn.Contains(container))
            {
                if (Size < container.Size)
                {
                    Log.printConcatLine("[ContainerScheduler.addMigratingInContainer] Allocation of VM #", container.Id, " to Host #", Id, " failed by storage");
                    //Environment.Exit(0);
                    throw new ArgumentException("Allocation of VM failed", "container");
                }

                if (!ContainerRamProvisioner.allocateRamForContainer(container, container.CurrentRequestedRam))
                {
                    Log.printConcatLine("[ContainerScheduler.addMigratingInContainer] Allocation of VM #", container.Id, " to Host #", Id, " failed by RAM");
                    //Environment.Exit(0);
                    throw new ArgumentException("Allocation of VM failed", "container");
                }

                if (!ContainerBwProvisioner.allocateBwForContainer(container, container.CurrentRequestedBw))
                {
                    Log.printLine("[ContainerScheduler.addMigratingInContainer] Allocation of VM #" + container.Id + " to Host #" + Id + " failed by BW");
                    //Environment.Exit(0);
                    throw new ArgumentException("Allocation of VM failed", "container");
                }

                ContainerSchedulerProperty.ContainersMigratingIn.Add(container.Uid);
                if (!ContainerSchedulerProperty.allocatePesForContainer(container, container.CurrentRequestedMips))
                {
                    Log.printLine(string.Format("[ContainerScheduler.addMigratingInContainer] Allocation of VM #{0:D} to Host #{1:D} failed by MIPS", container.Id, Id));
                    //Environment.Exit(0);
                    throw new ArgumentException("Allocation of VM failed", "container");
                }

                Size = Size - container.Size;

                ContainersMigratingIn.Add(container);
                ContainerListProperty.Add(container);
                updateContainersProcessing(CloudSim.clock());
                container.Vm.updateContainersProcessing(CloudSim.clock());
            }
        }

        public virtual double updateContainersProcessing(double currentTime)
        {
            double smallerTime = double.MaxValue;
            //        Log.printLine("ContainerVm: update Vms Processing");
            //        Log.printLine("The VM list size is:...." + getContainerList().size());

            foreach (Container container in ContainerListProperty)
            {
                double time = container.updateContainerProcessing(currentTime, ContainerSchedulerProperty.getAllocatedMipsForContainer(container));
                if (time > 0.0 && time < smallerTime)
                {
                    smallerTime = time;
                }
                double totalRequestedMips = container.CurrentRequestedTotalMips;
                double totalAllocatedMips = ContainerSchedulerProperty.getTotalAllocatedMipsForContainer(container);
                container.addStateHistoryEntry(currentTime, totalAllocatedMips, totalRequestedMips, (container.InMigration && !ContainersMigratingIn.Contains(container)));
            }
            //Log.printLine("Vm: The Smaller time is:......" + smallerTime);

            return smallerTime;
        }

        /// <summary>
        /// Removes the migrating in vm.
        /// </summary>
        /// <param name="container"> the container </param>
        public virtual void removeMigratingInContainer(Container container)
        {
            containerDeallocate(container);
            ContainersMigratingIn.Remove(container);
            ContainerListProperty.Remove(container);
            Log.printLine("ContainerVm# " + Id + "removeMigratingInContainer:......" + container.Id + "   Is deleted from the list");
            ContainerSchedulerProperty.ContainersMigratingIn.Remove(container.Uid);
            container.InMigration = false;
        }

        /// <summary>
        /// Reallocate migrating in containers.
        /// </summary>
        public virtual void reallocateMigratingInContainers()
        {
            //        Log.printLine("ContainerVm: re alocating MigratingInContainer:......");
            foreach (Container container in ContainersMigratingIn)
            {
                if (!ContainerListProperty.Contains(container))
                {
                    ContainerListProperty.Add(container);
                }
                if (!ContainerSchedulerProperty.ContainersMigratingIn.Contains(container.Uid))
                {
                    ContainerSchedulerProperty.ContainersMigratingIn.Add(container.Uid);
                }
                ContainerRamProvisioner.allocateRamForContainer(container, container.CurrentRequestedRam);
                ContainerBwProvisioner.allocateBwForContainer(container, container.CurrentRequestedBw);
                ContainerSchedulerProperty.allocatePesForContainer(container, container.CurrentRequestedMips);
                Size = Size - container.Size;
            }
        }

        /// <summary>
        /// Checks if is suitable for container.
        /// </summary>
        /// <param name="container"> the container </param>
        /// <returns> true, if is suitable for container </returns>
        public virtual bool isSuitableForContainer(Container container)
        {

            return (ContainerSchedulerProperty.PeCapacity >= container.CurrentRequestedMaxMips && ContainerSchedulerProperty.AvailableMips >= container.WorkloadTotalMips && ContainerRamProvisioner.isSuitableForContainer(container, container.CurrentRequestedRam) && ContainerBwProvisioner.isSuitableForContainer(container, container.CurrentRequestedBw));
        }

        /// <summary>
        /// Destroys a container running in the VM.
        /// </summary>
        /// <param name="container"> the container
        /// @pre $none
        /// @post $none </param>
        public virtual void containerDestroy(Container container)
        {
            //Log.printLine("Vm:  Destroy Container:.... " + container.getId());
            if (container != null)
            {
                containerDeallocate(container);
                //            Log.printConcatLine("The Container To remove is :   ", container.getId(), "Size before removing is ", getContainerList().size(), "  vm ID is: ", getId());
                ContainerListProperty.Remove(container);
                Log.printLine("ContainerVm# " + Id + " containerDestroy:......" + container.Id + "Is deleted from the list");

                //            Log.printConcatLine("Size after removing", getContainerList().size());
                while (ContainerListProperty.Contains(container))
                {
                    Log.printConcatLine("The container", container.Id, " is still here");
                    //                getContainerList().remove(container);
                }
                container.Vm = null;
            }
        }

        /// <summary>
        /// Destroys all containers running in the VM.
        /// 
        /// @pre $none
        /// @post $none
        /// </summary>
        public virtual void containerDestroyAll()
        {
            //        Log.printLine("ContainerVm: Destroy all Containers");
            containerDeallocateAll();
            foreach (Container container in ContainerListProperty)
            {
                container.Vm = null;
                Size = Size + container.Size;
            }
            //        Log.printLine("ContainerVm# "+getId()+" : containerDestroyAll:...... the whole list is cleared ");

            ContainerListProperty.Clear();

        }

        /// <summary>
        /// Deallocate all VMList for the container.
        /// </summary>
        /// <param name="container"> the container </param>
        protected internal virtual void containerDeallocate(Container container)
        {
            //        Log.printLine("ContainerVm: Deallocated the VM:......" + vm.getId());
            ContainerRamProvisioner.deallocateRamForContainer(container);
            ContainerBwProvisioner.deallocateBwForContainer(container);
            ContainerSchedulerProperty.deallocatePesForContainer(container);
            Size = Size + container.Size;
        }

        /// <summary>
        /// Deallocate all vmList for the container.
        /// </summary>
        protected internal virtual void containerDeallocateAll()
        {
            //        Log.printLine("ContainerVm: Deallocate all the Vms......");
            ContainerRamProvisioner.deallocateRamForAllContainers();
            ContainerBwProvisioner.deallocateBwForAllContainers();
            ContainerSchedulerProperty.deallocatePesForAllContainers();
        }

        /// <summary>
        /// Returns a container object.
        /// </summary>
        /// <param name="containerId"> the container id </param>
        /// <param name="userId">      ID of container's owner </param>
        /// <returns> the container object, $null if not found
        /// @pre $none
        /// @post $none </returns>
        public virtual Container getContainer(int containerId, int userId)
        {
            foreach (Container container in ContainerListProperty)
            {
                if (container.Id == containerId && container.UserId == userId)
                {
                    return container;
                }
            }
            return null;
        }


        /// <summary>
        /// Gets the free pes number.
        /// </summary>
        /// <returns> the free pes number </returns>
        public virtual int NumberOfFreePes
        {
            get
            {
                //        Log.printLine("ContainerVm: get the free Pes......" + ContainerPeList.getNumberOfFreePes(getPeList()));
                return ContainerPeList.getNumberOfFreePes(PeListProperty);
            }
        }

        /// <summary>
        /// Gets the total mips.
        /// </summary>
        /// <returns> the total mips </returns>
        public virtual int TotalMips
        {
            get
            {
                //        Log.printLine("ContainerVm: get the total mips......" + ContainerPeList.getTotalMips(getPeList()));
                return ContainerPeList.getTotalMips(PeListProperty);
            }
        }

        /// <summary>
        /// Allocates PEs for a VM.
        /// </summary>
        /// <param name="container"> the vm </param>
        /// <param name="mipsShare"> the mips share </param>
        /// <returns> $true if this policy allows a new VM in the host, $false otherwise
        /// @pre $none
        /// @post $none </returns>
        public virtual bool allocatePesForContainer(Container container, IList<double?> mipsShare)
        {
            //Log.printLine("ContainerVm: allocate Pes for Container:......" + container.getId());
            return ContainerSchedulerProperty.allocatePesForContainer(container, mipsShare);
        }

        /// <summary>
        /// Releases PEs allocated to a container.
        /// </summary>
        /// <param name="container"> the container
        /// @pre $none
        /// @post $none </param>
        public virtual void deallocatePesForContainer(Container container)
        {
            //Log.printLine("ContainerVm: deallocate Pes for Container:......" + container.getId());
            ContainerSchedulerProperty.deallocatePesForContainer(container);
        }

        /// <summary>
        /// Returns the MIPS share of each Pe that is allocated to a given container.
        /// </summary>
        /// <param name="container"> the container </param>
        /// <returns> an array containing the amount of MIPS of each pe that is available to the container
        /// @pre $none
        /// @post $none </returns>
        public virtual IList<double?> getAllocatedMipsForContainer(Container container)
        {
            return ContainerSchedulerProperty.getAllocatedMipsForContainer(container);
        }

        /// <summary>
        /// Gets the total allocated MIPS for a container over all the PEs.
        /// </summary>
        /// <param name="container"> the container </param>
        /// <returns> the allocated mips for container </returns>
        public virtual double getTotalAllocatedMipsForContainer(Container container)
        {
            //Log.printLine("ContainerVm: total allocated Pes for Container:......" + container.getId());
            return ContainerSchedulerProperty.getTotalAllocatedMipsForContainer(container);
        }

        /// <summary>
        /// Returns maximum available MIPS among all the PEs.
        /// </summary>
        /// <returns> max mips </returns>
        public virtual double MaxAvailableMips
        {
            get
            {
                //Log.printLine("ContainerVm: Maximum Available Pes:......");
                return ContainerSchedulerProperty.MaxAvailableMips;
            }
        }

        /// <summary>
        /// Gets the free mips.
        /// </summary>
        /// <returns> the free mips </returns>
        public virtual double AvailableMips
        {
            get
            {
                //Log.printLine("ContainerVm: Get available Mips");
                return ContainerSchedulerProperty.AvailableMips;
            }
        }

        /// <summary>
        /// Gets the machine bw.
        /// </summary>
        /// <returns> the machine bw
        /// @pre $none
        /// @post $result > 0 </returns>
        //public virtual long Bw
        //{
        //    get
        //    {
        //        //Log.printLine("ContainerVm: Get BW:......" + getContainerBwProvisioner().getBw());
        //        return ContainerBwProvisioner.Bw;
        //    }
        //}

        /// <summary>
        /// Gets the machine memory.
        /// </summary>
        /// <returns> the machine memory
        /// @pre $none
        /// @post $result > 0 </returns>
        //public virtual float Ram
        //{
        //    get
        //    {
        //        //Log.printLine("ContainerVm: Get Ram:......" + getContainerRamProvisioner().getRam());

        //        return ContainerRamProvisioner.Ram;
        //    }
        //}

        /// <summary>
        /// Gets the VM scheduler.
        /// </summary>
        /// <returns> the VM scheduler </returns>
        public virtual ContainerScheduler ContainerSchedulerProperty
        {
            get
            {
                return containerScheduler;
            }
        }


        /// <summary>
        /// Gets the pe list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the pe list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe> java.util.List<T> getPeList()
        //public virtual IList<T> getPeList<T>() where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public virtual IList<ContainerPe> PeListProperty
        {
            get

            {
                return (IList<ContainerPe>)peList;
            }
            set

            {
                this.peList = value;
            }
        }


        /// <summary>
        /// Gets the container list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the container list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends Container> java.util.List<T> getContainerList()
        public virtual IList<Container> ContainerListProperty
        {
            get

            {
                return (IList<Container>)containerList;
            }
        }


        /// <summary>
        /// Checks if is failed.
        /// </summary>
        /// <returns> true, if is failed </returns>
        public virtual bool Failed
        {
            get
            {
                return failed;
            }
        }

        /// <summary>
        /// Sets the PEs of this machine to a FAILED status. NOTE: <tt>resName</tt> is used for debugging
        /// purposes, which is <b>ON</b> by default. Use <seealso cref="#setFailed(boolean)"/> if you do not want
        /// this information.
        /// </summary>
        /// <param name="resName"> the name of the resource </param>
        /// <param name="failed">  the failed </param>
        /// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
        public virtual bool setFailed(string resName, bool failed)
        {
            // all the PEs are failed (or recovered, depending on fail)
            this.failed = failed;
            ContainerPeList.setStatusFailed(PeListProperty, resName, Id, failed);
            return true;
        }

        /// <summary>
        /// Sets the PEs of this machine to a FAILED status.
        /// </summary>
        /// <param name="failed"> the failed </param>
        /// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
        public virtual bool setFailed(bool failed)
        {
            // all the PEs are failed (or recovered, depending on fail)
            this.failed = failed;
            ContainerPeList.setStatusFailed(PeListProperty, failed);
            return true;
        }

        /// <summary>
        /// Sets the particular Pe status on this Machine.
        /// </summary>
        /// <param name="peId">   the pe id </param>
        /// <param name="status"> Pe status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
        /// <returns> <tt>true</tt> if the Pe status has changed, <tt>false</tt> otherwise (Pe id might not
        /// be exist)
        /// @pre peID >= 0
        /// @post $none </returns>
        public virtual bool setPeStatus(int peId, int status)
        {
            return ContainerPeList.setPeStatus(PeListProperty, peId, status);
        }

        /// <summary>
        /// Gets the containers migrating in.
        /// </summary>
        /// <returns> the containers migrating in </returns>
        public virtual IList<Container> ContainersMigratingIn
        {
            get
            {
                return containersMigratingIn;
            }
        }

        public virtual ContainerRamProvisioner ContainerRamProvisioner
        {
            get
            {
                return containerRamProvisioner;
            }
            set
            {
                this.containerRamProvisioner = value;
            }
        }



        public virtual ContainerBwProvisioner ContainerBwProvisioner
        {
            get
            {
                return containerBwProvisioner;
            }
            set
            {
                this.containerBwProvisioner = value;
            }
        }


        /// <summary>
        /// Allocates PEs and memory to a new container in the VM.
        /// </summary>
        /// <param name="container"> container being started </param>
        /// <returns> $true if the container could be started in the VM; $false otherwise
        /// @pre $none
        /// @post $none </returns>
        public virtual bool containerCreate(Container container)
        {
            //        Log.printLine("Host: Create VM???......" + container.getId());
            if (Size < container.Size)
            {
                Log.printConcatLine("[ContainerScheduler.ContainerCreate] Allocation of Container #", container.Id, " to VM #", Id, " failed by storage");
                return false;
            }

            if (!ContainerRamProvisioner.allocateRamForContainer(container, container.CurrentRequestedRam))
            {
                Log.printConcatLine("[ContainerScheduler.ContainerCreate] Allocation of Container #", container.Id, " to VM #", Id, " failed by RAM");
                return false;
            }

            if (!ContainerBwProvisioner.allocateBwForContainer(container, container.CurrentRequestedBw))
            {
                Log.printConcatLine("[ContainerScheduler.ContainerCreate] Allocation of Container #", container.Id, " to VM #", Id, " failed by BW");
                ContainerRamProvisioner.deallocateRamForContainer(container);
                return false;
            }

            if (!ContainerSchedulerProperty.allocatePesForContainer(container, container.CurrentRequestedMips))
            {
                Log.printConcatLine("[ContainerScheduler.ContainerCreate] Allocation of Container #", container.Id, " to VM #", Id, " failed by MIPS");
                ContainerRamProvisioner.deallocateRamForContainer(container);
                ContainerBwProvisioner.deallocateBwForContainer(container);
                return false;
            }

            Size = Size - container.Size;
            ContainerListProperty.Add(container);
            container.Vm = this;
            return true;
        }

        public virtual int NumberOfContainers
        {
            get
            {
                int c = 0;
                foreach (Container container in ContainerListProperty)
                {
                    if (!ContainersMigratingIn.Contains(container))
                    {
                        c++;
                    }
                }
                return c;
            }
        }

        public virtual bool InWaiting
        {
            get
            {
                return inWaiting;
            }
            set
            {
                this.inWaiting = value;
            }
        }
    }
}