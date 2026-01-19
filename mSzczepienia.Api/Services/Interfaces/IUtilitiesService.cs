namespace mSzczepienia.Api.Services.Interfaces
{
    public interface IUtilitiesService
    {
        int CalculateLevensteinDistance(string s, string t);
        string ChooseBestMatch(string? input, List<string> options);
        DateTime ParseDateTime(string? rawDate);
    }
}