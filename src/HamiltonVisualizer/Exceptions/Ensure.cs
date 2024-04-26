namespace HamiltonVisualizer.Exceptions
{
    internal static class Ensure
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
}
