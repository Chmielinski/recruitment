using Commons.Models.ViewModel;

namespace API.Services.Interface
{
    public interface IVisitorsService
    {
        public Task<IEnumerable<VisitorsByCountry>> GetVisitorsCounts();
        public Task<int> GetProcessedFilesCount();
        Task UploadVisits(IEnumerable<IFormFile> files);
    }
}
