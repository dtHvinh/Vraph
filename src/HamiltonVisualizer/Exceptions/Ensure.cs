namespace HamiltonVisualizer.Exceptions
{
    public static class Ensure
    {
        public static void ThrowIf(bool condition, Type? name, string? errorMessage, params object[]? args)
        {
            if (condition)
            {
                return;
            }

            string? msg = null;

            if (errorMessage is not null)
            {
                if (args is not null)
                    msg = string.Format(errorMessage, args);
            }

            if (name is null)
                throw new ArgumentException(msg);

            var exception = Activator.CreateInstance(name, args: errorMessage);

            throw (Exception)exception!;
        }
    }

    static class EM
    {
        public const string No_Map_At_Index = "No instance of {0} is mapped at index {1}";
    }
}
