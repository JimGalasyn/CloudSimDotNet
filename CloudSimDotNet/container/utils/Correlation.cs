using System;

namespace org.cloudbus.cloudsim.container.utils
{

	/// <summary>
	/// Created by sareh on 7/08/15.
	/// </summary>
	public class Correlation
	{
	//    Ref : http://en.wikipedia.org/wiki/Correlation_and_dependence

		public Correlation()
		{
		}

		public virtual double getCor(double[] xs1, double[] ys1)
		{
			//TODO: check here that arrays are not null, of the same length etc
			double[] xs = null;
			double[] ys = null;
			if (xs1.Length > ys1.Length)
			{
                // TODO: Translate array copy.
                //xs = Arrays.copyOfRange(xs1, (xs1.Length - ys1.Length), xs1.Length);
                //Array.Copy(xs1, (xs1.Length - ys1.Length), xs, 0, xs1.Length);
                ys = ys1;

			}
			else if (xs1.Length < ys1.Length)
			{
                // TODO: Translate array copy.
                //ys = Arrays.copyOfRange(ys1, (ys1.Length - xs1.Length), ys1.Length);
				xs = xs1;

			}
			else
			{
				ys = ys1;
				xs = xs1;
			}

			double sx = 0.0;
			double sy = 0.0;
			double sxx = 0.0;
			double syy = 0.0;
			double sxy = 0.0;

			int n = xs.Length;

			for (int i = 0; i < n; ++i)
			{
				double x = xs[i];
				double y = ys[i];

				sx += x;
				sy += y;
				sxx += x * x;
				syy += y * y;
				sxy += x * y;
			}

			// covariation
			double cov = sxy / n - sx * sy / n / n;
			// standard error of x
			double sigmax = Math.Sqrt(sxx / n - sx * sx / n / n);
			// standard error of y
			double sigmay = Math.Sqrt(syy / n - sy * sy / n / n);

			// correlation is just a normalized covariation
			return cov / sigmax / sigmay;
		}

	}

}