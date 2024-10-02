using System.Collections.Generic;

namespace ToDo.Application.DTOs
{
  public class PaginationRequestDto
  {
    public int Limit { get; set; }
    public string? Cursor { get; set; }
  }

  public class PaginatedResultDto<T>
  {
    public List<T> Items { get; set; } = new List<T>();
    public string? NextCursor { get; set; }
    public bool HasNextPage => !string.IsNullOrEmpty(NextCursor);
  }
}