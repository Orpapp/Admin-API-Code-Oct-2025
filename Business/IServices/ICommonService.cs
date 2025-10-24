namespace Business.IServices
{
    public interface ICommonService
    {
        DateTime GetTimeSubtractOffSet(DateTime dt);
        DateTime GetTimeAfterAddOffSet(DateTime dt);
        DateTime Convertdate(string date, string time);

    }
}
