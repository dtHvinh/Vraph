using System.Windows;

namespace HamiltonVisualizer.Core.Contracts
{
    /// <summary>
    /// Represent the element can determined by a <see cref="Point"/>.
    /// </summary>
    internal interface INavigableElement
    {
        Point Origin { get; }
    }
}
