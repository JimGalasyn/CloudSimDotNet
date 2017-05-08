using System;

namespace org.cloudbus.cloudsim.container.utils
{

	/// <summary>
	/// The class is generated to produce an integer with a gaussian/normal distribution
	/// Created by sareh on 16/12/15.
	/// </summary>


	public class RandomGaussian
	{
		internal Random random;

		public RandomGaussian()
		{
			Random = new Random();

		}

		public virtual Random Random
		{
			set
			{
				this.random = value;
			}
			get
			{
				return this.random;
			}
		}

	}

}