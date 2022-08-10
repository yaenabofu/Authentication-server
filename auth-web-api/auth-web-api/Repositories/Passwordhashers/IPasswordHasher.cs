namespace auth_web_api.Repositories.Passwordhashers
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}
