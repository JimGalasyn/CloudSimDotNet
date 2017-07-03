using System;

namespace org.cloudbus.cloudsim.examples.container
{
    using Constants = org.cloudbus.cloudsim.examples.power.Constants;

    /// <summary>
    /// Created by sareh on 5/08/15.
    /// </summary>
    public class UtilizationModelPlanetLabInMemoryExtended : UtilizationModelPlanetLabInMemory
    {
        public UtilizationModelPlanetLabInMemoryExtended(string inputPath, double schedulingInterval) : base(inputPath, schedulingInterval)
        {
        }

        public UtilizationModelPlanetLabInMemoryExtended(string inputPath, double schedulingInterval, int dataSamples) : base(inputPath, schedulingInterval, dataSamples)
        {
        }

        public override double getUtilization(double inputTime)
        {
            double utilization;
            if (inputTime > Constants.SIMULATION_LIMIT || inputTime == Constants.SIMULATION_LIMIT)
            {
                utilization = calUtilization(inputTime % Constants.SIMULATION_LIMIT);
            }
            else
            {
                utilization = calUtilization(inputTime);
            }

            return utilization;
        }

        public virtual double calUtilization(double time)
        {
            //        Log.print(time);
            double[] data = base.Data;
            if (time % SchedulingInterval == 0)
            {
                return data[(int)time / (int)SchedulingInterval];
            }
            int time1 = (int)Math.Floor(time / SchedulingInterval);
            int time2 = (int)Math.Ceiling(time / SchedulingInterval);
            double utilization1 = data[time1];
            double utilization2 = data[time2];
            double delta = (utilization2 - utilization1) / ((time2 - time1) * SchedulingInterval);
            double utilization = utilization1 + delta * (time - time1 * SchedulingInterval);

            return utilization;
        }
    }
}