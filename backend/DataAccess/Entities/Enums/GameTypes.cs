#pragma warning disable 1591 // Disable XML comments being required

namespace YouFoos.DataAccess.Entities.Enums
{
    // TODO replace this class with a proper enum - requires changing the table code as well so no small task - probably also remediations

    /// <summary>
    /// This would normally be an Enum but variables in C# can't start with a number and I don't
    /// want to change the table code.
    /// </summary>
    public static class GameTypes
    {
        public static string Singles = "1v1";
        public static string Doubles = "2v2";
    }
}
