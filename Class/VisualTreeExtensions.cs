using System;
using System.Windows.Media;
using System.Windows;

namespace FrpcUI.Class
{
    public static class VisualTreeHelperExtensions
    {
        // 查找父级元素
        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent) return parent;
                child = VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        // 查找子级元素（带条件筛选）
        public static T FindVisualChild<T>(this DependencyObject parent, Func<T, bool> predicate = null)
            where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result && (predicate == null || predicate(result)))
                    return result;

                var descendant = FindVisualChild(child, predicate);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }
    }
}
