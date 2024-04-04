using HamiltonVisualizer.Utilities;

namespace HamiltonVisualizer.Core.Contracts
{
    public interface IRefSetter
    {
        void SetRefs(RefBag refBag);
    }

    public interface IRefSetter<T>
    {
        void SetRef(T ref1);
    }

    public interface IRefSetter<T1, T2>
    {
        void SetRefs(T1 ref1, T2 ref2);
    }

    public interface IRefSetter<T1, T2, T3>
    {
        void SetRefs(T1 ref1, T2 ref2, T3 ref3);
    }
}
