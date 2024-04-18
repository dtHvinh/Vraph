using HamiltonVisualizer.Events.EventArgs.ForAlgorithm;

namespace HamiltonVisualizer.Events.EventHandlers.ForAlgorithm;

public delegate void PresentingTraversalAlgorithmEventHandler(object? sender, PresentingTraversalAlgorithmEventArgs args);
public delegate void PresentingLayeredBFSEventHandler(object? sender, PresentingLayeredBFSEventArgs args);
