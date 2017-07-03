using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.utils
{

	/// <summary>
	/// Created by sareh on 13/08/15.
	/// </summary>

	using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
	using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
	using org.cloudbus.cloudsim.container.core;


	/// <summary>
	/// A factory for CloudSim entities' ids. CloudSim requires a lot of ids, that
	/// are provided by the end user. This class is a utility for automatically
	/// generating valid ids.
	/// Modifies for containers
	/// 
	/// @author nikolay.grozev
	/// </summary>

	public sealed class IDs
	{


        //private static readonly IDictionary<Type, int?> COUNTERS = new LinkedHashMap<Type, int?>();
        // TEST: (fixed) Dictionary == LinkedHashMap?
        private static readonly IDictionary<Type, int?> COUNTERS = new Dictionary<Type, int?>();
        private static readonly ISet<Type> NO_COUNTERS = new HashSet<Type>();
		private static int globalCounter = 1;

		static IDs()
		{
			COUNTERS[typeof(ContainerCloudlet)] = 1;
			COUNTERS[typeof(ContainerVm)] = 1;
			COUNTERS[typeof(Container)] = 1;
			COUNTERS[typeof(ContainerHost)] = 1;
			COUNTERS[typeof(ContainerDatacenterBroker)] = 1;
			COUNTERS[typeof(ContainerPe)] = 1;
			COUNTERS[typeof(ContainerVmPe)] = 1;
		}

		private IDs()
		{
		}

		/// <summary>
		/// Returns a valid id for the specified class.
		/// </summary>
		/// <param name="clazz"> - the class of the object to get an id for. Must not be null. </param>
		/// <returns> a valid id for the specified class. </returns>
		public static int pollId(Type clazz)
		{
			lock (typeof(IDs))
			{
				Type matchClass = null;
				if (COUNTERS.ContainsKey(clazz))
				{
					matchClass = clazz;
				}
				else if (!NO_COUNTERS.Contains(clazz))
				{
					foreach (Type key in COUNTERS.Keys)
					{
                        // TEST: (fixed) Make sure that this IsAssignableFrom thing is okay.
                        //if (key.IsAssignableFrom(clazz))
                        if ( key.GetType() == clazz)
						{
							matchClass = key;
							break;
						}
					}
				}
        
				int result = -1;
				if (matchClass == null)
				{
					NO_COUNTERS.Add(clazz);
					result = pollGlobalId();
				}
				else
				{
					result = COUNTERS[matchClass].Value;
					COUNTERS[matchClass] = result + 1;
				}
        
				if (result < 0)
				{
					throw new System.InvalidOperationException("The generated id for class:" + clazz.FullName + " is negative. Possible integer overflow.");
				}
        
				return result;
			}
		}

		private static int pollGlobalId()
		{
			lock (typeof(IDs))
			{
				return globalCounter++;
			}
		}

	}



}