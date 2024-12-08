namespace ToDo.Domain.Common
{
  public class PaginationRequest
  {
    public int Limit { get; }
    public string? Cursor { get; }

    public PaginationRequest(int limit, string? cursor)
    {
      Limit = limit;
      Cursor = cursor;
    }
  }
}
