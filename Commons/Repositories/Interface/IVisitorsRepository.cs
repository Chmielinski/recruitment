using Commons.Models.Database;

namespace Commons.Repositories.Interface
{
    public interface IVisitorsRepository
    {
        Task<Visit> AddVisit(Visit visit);
        Task<IEnumerable<Visit>> GetAllVisits();
        Task<int> GetVisitsCount();
    }
}
