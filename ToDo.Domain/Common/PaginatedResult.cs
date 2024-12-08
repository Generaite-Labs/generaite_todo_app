namespace ToDo.Domain.Common
{
  public class PaginatedResult<T>
  {
    public IEnumerable<T> Items { get; }
    public string? NextCursor { get; }
    public bool HasNextPage => !string.IsNullOrEmpty(NextCursor);

    public PaginatedResult(IEnumerable<T> items, string? nextCursor)
    {
      Items = items;
      NextCursor = nextCursor;
    }
  }
}
