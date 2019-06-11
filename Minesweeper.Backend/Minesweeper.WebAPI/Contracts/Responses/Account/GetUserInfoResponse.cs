namespace Minesweeper.WebAPI.Contracts.Responses.Account
{
    // TODO: Consolidate other responses to use the same conventions (e.g. no setters for complex objects or allow setter)
    // TODO: Move other responses to appropriate folders and namespaces based on which controller they are returned from
    public class GetUserInfoResponse
    {
        public UserInfo UserInfo { get; } = new UserInfo();
    }
}