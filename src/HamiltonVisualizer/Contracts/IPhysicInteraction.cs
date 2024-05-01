using HamiltonVisualizer.Core.Base;

namespace HamiltonVisualizer.Contracts;
internal interface IPhysicInteraction
{
    /// <summary>
    /// Check if any collide detected.
    /// </summary>
    bool HasCollisions();

    /// <summary>
    /// Apply specified <paramref name="force"/> to an <paramref name="other"/> object. A <paramref name="callback"/>
    /// will be called after interaction finished.
    /// </summary>
    void ApplyForce(MovableObject other, double force, Action<MovableObject> callback);

    /// <summary>
    /// Apply specified <paramref name="force"/> to an <paramref name="other"/> object.
    /// </summary>
    void ApplyForce(MovableObject other, double force);

    /// <summary>
    /// Apply specified <paramref name="force"/> from <paramref name="source"/> to this object.
    /// </summary>
    void ApplyForceFrom(MovableObject source, double force);
}
