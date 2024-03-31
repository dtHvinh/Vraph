namespace HamiltonVisualizer.Exceptions
{
    public static class Ensure
    {
        public static void ThrowIf(bool condition, Type? exception, string? errorMessage, params object[]? args)
        {
            if (!condition)
            {
                return;
            }

            string? msg = null;

            if (errorMessage is not null)
            {
                if (args is not null)
                    msg = string.Format(errorMessage, args);
            }

            if (exception is null)
                throw new ArgumentException(msg);

            var exceptionInstance = Activator.CreateInstance(exception, args: errorMessage);

            throw (Exception)exceptionInstance!;
        }
    }

    static class EM
    {
        public const string No_Map_At_Index = "No instance of {0} is mapped at index {1}";
        public const string Not_Support_Negative_Number = "Negetive number is illegal in this context";
    }
}
