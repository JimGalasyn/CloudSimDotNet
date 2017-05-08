using System;

namespace org.cloudbus.cloudsim.container.utils
{

	/// <summary>
	/// Created by sareh on 13/08/15.
	/// </summary>
	public class RandomGen
	{
		internal Random random;

		public RandomGen()
		{
			Random = new Random();
	//        random.setSeed(123456789);
		}

		public virtual Random Random
		{
			get
			{
				return random;
			}
			set
			{
				this.random = value;
			}
		}


		public virtual int getNum(int i)
		{

			return Random.Next(i);
		}
	}

}