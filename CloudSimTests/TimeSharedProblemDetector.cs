using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim
{

	/*
	 * Title:        CloudSim Toolkit
	 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation
	 *               of Clouds
	 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
	 *
	 * Copyright (c) 2009, The University of Melbourne, Australia
	 */


	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using BwProvisionerSimple = org.cloudbus.cloudsim.provisioners.BwProvisionerSimple;
	using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
	using RamProvisionerSimple = org.cloudbus.cloudsim.provisioners.RamProvisionerSimple;

	/// <summary>
	/// A simple example showing how to create a datacenter with one host and run one
	/// cloudlet on it.
	/// </summary>
	public class TimeSharedProblemDetector
	{

		/// <summary>
		/// The cloudlet list. </summary>
		private static IList<Cloudlet> cloudletList;

		/// <summary>
		/// The vmlist. </summary>
		private static IList<Vm> vmlist;

        /// <summary>
        /// Creates main() to run this example.
        /// </summary>
        /// <param name="args"> the args </param>
        public static void ExampleMain(string[] args)
        //public static void Main(string[] args)
        {

			Log.printLine("Starting CloudSimExample1...");

			try
			{
				// First step: Initialize the CloudSim package. It should be called
				// before creating any entities.
				int num_user = 1; // number of cloud users
				DateTime calendar = new DateTime();
				bool trace_flag = false; // mean trace events

				// Initialize the CloudSim library
				CloudSim.init(num_user, calendar, trace_flag);

				// Second step: Create Datacenters
				// Datacenters are the resource providers in CloudSim. We need at
				// list one of them to run a CloudSim simulation
				Datacenter datacenter0 = createDatacenter("Datacenter_0");

				// Third step: Create Broker
				DatacenterBroker broker = createBroker();
				int brokerId = broker.Id;

				// Fourth step: Create one virtual machine
				vmlist = new List<Vm>();

				// VM description
				int vmid = 0;
				int mips = 1000;
				long size = 10000; // image size (MB)
				int ram = 512; // vm memory (MB)
				long bw = 1000;
				int pesNumber = 1; // number of cpus
				string vmm = "Xen"; // VMM name

				// create VM
				Vm vm = new Vm(vmid, brokerId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());

				Vm vm1 = new Vm(1, brokerId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());
				Vm vm2 = new Vm(2, brokerId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());

				// add the VM to the vmList
				vmlist.Add(vm);

				vmlist.Add(vm1);
				vmlist.Add(vm2);

				// submit vm list to the broker
				broker.submitVmList(vmlist);

				// Fifth step: Create one Cloudlet
				cloudletList = new List<Cloudlet>();

				// Cloudlet properties
				int id = 0;
				long length = 400000;
				long fileSize = 300;
				long outputSize = 300;
				UtilizationModel utilizationModel = new UtilizationModelFull();

				Cloudlet cloudlet = new Cloudlet(id, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel);
				cloudlet.UserId = brokerId;
				cloudlet.VmId = vmid;

				Cloudlet cloudlet1 = new Cloudlet(1, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel);
				cloudlet1.UserId = brokerId;
				cloudlet1.VmId = 1;

				Cloudlet cloudlet2 = new Cloudlet(2, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel);
				cloudlet2.UserId = brokerId;
				cloudlet2.VmId = 2;


				// add the cloudlet to the list
				cloudletList.Add(cloudlet);
				cloudletList.Add(cloudlet1);
				cloudletList.Add(cloudlet2);

				// submit cloudlet list to the broker
				broker.submitCloudletList(cloudletList);

				// Sixth step: Starts the simulation
				CloudSim.startSimulation();

				CloudSim.stopSimulation();

                //Final step: Print results when simulation is over
                IList<Cloudlet> newList = broker.CloudletReceivedListProperty;
                printCloudletList(newList);

				Log.printLine("CloudSimExample1 finished!");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				Log.printLine("The simulation has been terminated due to an unexpected error");
			}
		}

		/// <summary>
		/// Creates the datacenter.
		/// </summary>
		/// <param name="name"> the name
		/// </param>
		/// <returns> the datacenter </returns>
		private static Datacenter createDatacenter(string name)
		{

			// Here are the steps needed to create a PowerDatacenter:
			// 1. We need to create a list to store
			// our machine
			IList<Host> hostList = new List<Host>();

			// 2. A Machine contains one or more PEs or CPUs/Cores.
			// In this example, it will have only one core.
			IList<Pe> peList = new List<Pe>();

			int mips = 1000;

			// 3. Create PEs and add these into a list.
			peList.Add(new Pe(0, new PeProvisionerSimple(mips))); // need to store Pe id and MIPS Rating

			// 4. Create Host with its id and list of PEs and add them to the list
			// of machines
			int hostId = 0;
			int ram = 2048; // host memory (MB)
			long storage = Consts.MILLION; // host storage
			int bw = 10000;

			hostList.Add(new Host(hostId, new RamProvisionerSimple(ram), new BwProvisionerSimple(bw), storage, peList, new VmSchedulerTimeSharedOverSubscription(peList)
			   )); // This is our machine

			// 5. Create a DatacenterCharacteristics object that stores the
			// properties of a data center: architecture, OS, list of
			// Machines, allocation policy: time- or space-shared, time zone
			// and its price (G$/Pe time unit).
			string arch = "x86"; // system architecture
			string os = "Linux"; // operating system
			string vmm = "Xen";
			double time_zone = 10.0; // time zone this resource located
			double cost = 3.0; // the cost of using processing in this resource
			double costPerMem = 0.05; // the cost of using memory in this resource
			double costPerStorage = 0.001; // the cost of using storage in this
											// resource
			double costPerBw = 0.0; // the cost of using bw in this resource
			List<Storage> storageList = new List<Storage>(); // we are not adding SAN
														// devices by now

			DatacenterCharacteristics characteristics = new DatacenterCharacteristics(arch, os, vmm, hostList, time_zone, cost, costPerMem, costPerStorage, costPerBw);

			// 6. Finally, we need to create a PowerDatacenter object.
			Datacenter datacenter = null;
			try
			{
				datacenter = new Datacenter(name, characteristics, new VmAllocationPolicySimple(hostList), storageList, 0);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			return datacenter;
		}

		// We strongly encourage users to develop their own broker policies, to
		// submit vms and cloudlets according
		// to the specific rules of the simulated scenario
		/// <summary>
		/// Creates the broker.
		/// </summary>
		/// <returns> the datacenter broker </returns>
		private static DatacenterBroker createBroker()
		{
			DatacenterBroker broker = null;
			try
			{
				broker = new DatacenterBroker("Broker");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				return null;
			}
			return broker;
		}

		/// <summary>
		/// Prints the Cloudlet objects.
		/// </summary>
		/// <param name="list"> list of Cloudlets </param>
		private static void printCloudletList(IList<Cloudlet> list)
		{
			int size = list.Count;
			Cloudlet cloudlet;

			string indent = "    ";
			Log.printLine();
			Log.printLine("========== OUTPUT ==========");
			Log.printLine("Cloudlet ID" + indent + "STATUS" + indent + "Data center ID" + indent + "VM ID" + indent + "Time" + indent + "Start Time" + indent + "Finish Time");

			//DecimalFormat dft = new DecimalFormat("###.##");
			for (int i = 0; i < size; i++)
			{
				cloudlet = list[i];
				Log.print(indent + cloudlet.CloudletId + indent + indent);

				if (cloudlet.CloudletStatus == Cloudlet.SUCCESS)
				{
					Log.print("SUCCESS");

                    //Log.printLine(indent + indent + cloudlet.ResourceId + indent + indent + indent + cloudlet.VmId + indent + indent + dft.format(cloudlet.ActualCPUTime) + indent + indent + dft.format(cloudlet.ExecStartTime) + indent + indent + dft.format(cloudlet.FinishTime));
                    Log.printLine(indent + indent + cloudlet.ResourceId + indent + indent + indent + cloudlet.VmId + indent + indent + cloudlet.ActualCPUTime + indent + indent + cloudlet.ExecStartTime + indent + indent + cloudlet.FinishTime);
                }
			}
		}

	}

}