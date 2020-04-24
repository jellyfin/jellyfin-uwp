namespace Jellyfin.Models.Adapters
{
    /// <summary>
    /// Adds an interface to implement a conversion function for TOut type.
    /// </summary>
    /// <typeparam name="TIn">The source object type.</typeparam>
    /// <typeparam name="TOut">The destination object type.</typeparam>
    public interface IAdapter<in TIn, out TOut>
    {
        /// <summary>
        /// Declares a Convert method, to convert the object from Source format to Destination format.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        TOut Convert(TIn source);
    }
}
