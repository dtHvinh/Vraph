using HamiltonVisualizer.Events.EventArgs.ForAlgorithm;

namespace HamiltonVisualizer.Events.EventHandlers.ForAlgorithm;

internal delegate void PresentingTraversalAlgorithmEventHandler(object? sender, PresentingTraversalAlgorithmEventArgs args);
internal delegate void PresentingLayeredBFSEventHandler(object? sender, PresentingLayeredBFSEventArgs args);
