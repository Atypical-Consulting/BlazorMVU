// This file is kept for backward compatibility.
// New components should use BlazorMVU.SimpleMvuComponent or BlazorMVU.MvuComponent directly.

namespace BlazorMVU.Demo.Shared;

/// <summary>
/// Legacy MVU component base class for backward compatibility.
/// For new components, use BlazorMVU.SimpleMvuComponent or BlazorMVU.MvuComponent.
/// </summary>
public abstract class MvuComponent<TModel, TMsg> : BlazorMVU.SimpleMvuComponent<TModel, TMsg>
    where TModel : notnull;
