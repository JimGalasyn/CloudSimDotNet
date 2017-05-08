using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.cloudbus.cloudsim.core
{
    /// <summary>
    /// Supports cloning, which creates a new instance of a class with the same value as an existing instance.
    /// </summary>
    /// <remarks>
    /// .NET Core doesn't provide an ICloneable interface, so this one
    /// will have to do.
    /// https://msdn.microsoft.com/en-us/library/system.icloneable_methods.aspx
    /// </remarks>
    public interface ICloneable
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object Clone();
    }
}
