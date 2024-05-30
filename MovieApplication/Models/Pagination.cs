namespace MovieApplication.ViewModel
{
    public class Pagination
    {
        public int TotalItems { get; set; } // property 
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool PreviousPage { get; set; }
        public bool NextPage { get; set; }

        public Pagination(int totalItems, int currentPage, int pageSize)
        {
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);

            PreviousPage = CurrentPage > 1;  //if "CurrentPage" is more than 1, then "PreviousPage" is true 
            NextPage = CurrentPage < TotalPages;  //if "CurrentPage" is less than "TotalPages," then "NextPage" is true
        }
    }
}
