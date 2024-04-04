namespace HamiltonVisualizer.Core.Contracts
{
    public interface IRefGetter<T1>
    {
        T1 GetRef();
    }

    public interface IRefGetter<T1, T2>
    {
        (T1, T2) GetRefs();
    }

    public interface IRefGetter<T1, T2, T3>
    {
        (T1, T2, T3) GetRefs();
    }


    public interface IReadOnlyRefGetter<T1>
    {
        T1 GetReadOnlyRef();
    }

    public interface IReadOnlyRefGetter<T1, T2>
    {
        (T1, T2) GetReadOnlyRefs();
    }

    public interface IReadOnlyRefGetter<T1, T2, T3>
    {
        (T1, T2, T3) GetReadOnlyRefs();
    }
}
