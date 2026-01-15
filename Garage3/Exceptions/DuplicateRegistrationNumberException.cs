namespace Garage3.Exceptions
{
    public class DuplicateRegistrationNumberException : Exception
    {
        public DuplicateRegistrationNumberException(string regNr)
            : base($"Vehicle with registration number '{regNr}' already exists.")
        {
        }
    }
}