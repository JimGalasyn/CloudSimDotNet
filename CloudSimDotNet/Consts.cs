namespace org.cloudbus.cloudsim
{

	/// 
	/// <summary>
	/// Defines common constants, used throughout cloudsim.
	/// 
	/// @author nikolay.grozev
	/// 
	/// </summary>
	public sealed class Consts
	{

		/// <summary>
		/// Suppreses intantiation. </summary>
		private Consts()
		{
		}

		/// <summary>
		/// One million. </summary>
		public const int MILLION = 1000000;

		// ================== Time constants ==================
		/// <summary>
		/// One minute time in seconds. </summary>
		public const int MINUTE = 60;
		/// <summary>
		/// One hour time in seconds. </summary>
		public static readonly int HOUR = 60 * MINUTE;
		/// <summary>
		/// One day time in seconds. </summary>
		public static readonly int DAY = 24 * HOUR;
		/// <summary>
		/// One week time in seconds. </summary>
		public static readonly int WEEK = 24 * HOUR;

		// ================== OS constants ==================
		/// <summary>
		/// Constant for *nix Operating Systems. </summary>
		public const string NIX_OS = "Linux/Unix";
		/// <summary>
		/// Constant for Windows Operating Systems. </summary>
		public const string WINDOWS = "Windows";
	}

}