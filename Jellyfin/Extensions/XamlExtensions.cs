using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Jellyfin.Extensions
{
    public class XamlExtensions
    {
        /// <summary>
        /// Finds visual child for a parent.
        /// </summary>
        /// <typeparam name="T">The child type.</typeparam>
        /// <param name="parent">The parent reference.</param>
        /// <returns></returns>
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                    T candidate = child as T;
                    if (candidate != null)
                    {
                        return candidate;
                    }

                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return default(T);
        }
    }
}
