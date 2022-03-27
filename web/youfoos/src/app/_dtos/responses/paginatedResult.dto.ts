export class PaginatedResultDto<T> {
  public pageNumber: number;
  public pageSize: number;
  public totalResults: number;
  public results: T[]
}
