using System.Windows;

namespace HamiltonVisualizer.Core.Contracts
{
    /// <summary>
    /// Represent the element can determined by a <see cref="Point"/>.
    /// </summary>
    public interface INavigableElement
    {
        Point Origin { get; }
    }
}
