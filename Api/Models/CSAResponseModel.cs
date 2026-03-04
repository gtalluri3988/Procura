namespace Api.Models
{
    public class CSAResponseModel<T>
    {
        public CSAResponseModel(T data)
        {
            Error = false;
            Errors = null;
            Data = data;
        }
        public CSAResponseModel(bool error,string[] errors)
        {
            Error = error;
            Errors = errors;
            Data = default;
        }
        public CSAResponseModel(bool error, string errorsMessage)
        {
            Error = error;
            Errors = new string[1] { errorsMessage};
            Data = default;
        }
        public bool Error { get; }
        public string[] Errors { get; }
        public T Data { get; }
    }
    public class PaginationModel
    {
        public PaginationModel(int pageNumber, int rowPerPage, int resultCount, int totalCount)
        {
            PageNumber = pageNumber;
            RowPerPage = rowPerPage;
            ResultCount = resultCount;
            TotalCount = totalCount;
        }
        public int PageNumber { get; }
        public int RowPerPage { get; }
        public int ResultCount { get; }
        public int TotalCount { get; }
    }
    public class CSAResponsePaginationModel<T>
    {
        public  CSAResponsePaginationModel(CSAResponseModel<T> csaResponseModel,PaginationModel pagination)
        {
            CSAResponse=csaResponseModel;
            Pagination=pagination;
        }
        public CSAResponseModel<T> CSAResponse { get; }
        public PaginationModel Pagination { get; }
    }
}
