
using System;
using System.Diagnostics;
using System.Linq;

namespace Utility
{
	/// <summary>
	/// Helper class for raising events.
	/// </summary>
	public static class EventsHelper
	{
	    /// <summary>
	    /// Helper method for raising events safely.
	    /// </summary>
	    /// <param name="del">Delegate to invoke.</param>
	    /// <param name="sender">The sender of the event.</param>
	    /// <param name="e">The <see cref="EventArgs"/>.</param>
	    /// <remarks>
	    /// Use this method to invoke user code via delegates.
	    /// This method will log any exceptions thrown in user code and immediately rethrow it.
	    /// The typical usage is shown below.
	    /// </remarks>
	    /// <example>
	    /// <code>
	    /// [C#]
	    /// public class PresentationImage
	    /// {
	    ///    private event EventHandler _imageDrawingEvent;
	    ///    
	    ///    public void Draw()
	    ///    {
	    ///       EventsHelper.Fire(_imageDrawingEvent, this, new DrawImageEventArgs());
	    ///    }
	    /// }
	    /// </code>
	    /// </example>
	    public static void Fire(EventHandler del, object sender, EventArgs e)
	    {
            if (del == null)
                return;

	        var delegates = del.GetInvocationList().Cast<EventHandler>();
            foreach (var sink in delegates)
            {
                try
                {
                    sink.Invoke(sender, e);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    throw;
                }
            }
        }

        	    /// <summary>
	    /// Helper method for raising events safely.
	    /// </summary>
	    /// <param name="del">Delegate to invoke.</param>
	    /// <param name="sender">The sender of the event.</param>
	    /// <param name="e">The <see cref="EventArgs"/>.</param>
	    /// <remarks>
	    /// Use this method to invoke user code via delegates.
	    /// This method will log any exceptions thrown in user code and immediately rethrow it.
	    /// The typical usage is shown below.
	    /// </remarks>
	    /// <example>
	    /// <code>
	    /// [C#]
	    /// public class PresentationImage
	    /// {
	    ///    private event EventHandler _imageDrawingEvent;
	    ///    
	    ///    public void Draw()
	    ///    {
	    ///       EventsHelper.Fire(_imageDrawingEvent, this, new DrawImageEventArgs());
	    ///    }
	    /// }
	    /// </code>
	    /// </example>
	    public static void Fire<T>(EventHandler<T> del, object sender, T e) where T : EventArgs
        {
            if (del == null)
                return;

            var delegates = del.GetInvocationList().Cast<EventHandler<T>>();
            foreach (var sink in delegates)
            {
                try
                {
                    sink.Invoke(sender, e);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    throw;
                }
            }
        }

        /// <summary>
        /// Helper method for raising events safely.
        /// </summary>
        /// <param name="del">Delegate to invoke.</param>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        /// <remarks>
        /// Use this method to invoke user code via delegates.
        /// This method will log any exceptions thrown in user code and immediately rethrow it.
        /// The typical usage is shown below.
        /// </remarks>
        /// <example>
        /// <code>
        /// [C#]
        /// public class PresentationImage
        /// {
        ///    private event EventHandler _imageDrawingEvent;
        ///    
        ///    public void Draw()
        ///    {
        ///       EventsHelper.Fire(_imageDrawingEvent, this, new DrawImageEventArgs());
        ///    }
        /// }
        /// </code>
        /// </example>
	    public static void Fire(Delegate del, object sender, EventArgs e)
		{
			// TODO CR (Apr 13): del should really be EventHandler (and an overload added for EventHandler<T>) since this method won't work with any other kind of delegate
			//                   and a strongly typed API would help programmers from putting in the wrong arguments
			if (del == null)
				return;

			var delegates = del.GetInvocationList();

			foreach (var sink in delegates)
			{
				try
				{
					sink.DynamicInvoke(sender, e);
				}
				catch (Exception ex)
				{
                    Debug.WriteLine(ex.ToString());
                    throw;
				}
			}
		}
	}
}