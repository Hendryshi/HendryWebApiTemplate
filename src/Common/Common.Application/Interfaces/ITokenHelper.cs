namespace Common.Application.Interfaces
{
    public interface ITokenHelper
    {
        string GetToken();

        string GetClientIdFromToken();
    }
}
