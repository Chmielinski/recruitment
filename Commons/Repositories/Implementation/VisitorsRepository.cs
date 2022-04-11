using Commons.Models.Database;
using Commons.Repositories.Interface;
using Commons.Utils.Database;
using Microsoft.EntityFrameworkCore;

namespace Commons.Repositories.Implementation
{
    public class VisitorsRepository : IVisitorsRepository
    {
        private RidelyDbContext _context { get; }

        public VisitorsRepository(RidelyDbContext context)
        {
            _context = context;
        }

        public async Task<Visit> AddVisit(Visit visit)
        {
            _context.Add(visit);
            await _context.SaveChangesAsync();
            return visit;
        }

        public async Task<IEnumerable<Visit>> GetAllVisits()
        {
            return await _context.Visits.ToListAsync();
        }

        public async Task<int> GetVisitsCount()
        {
            return await _context.Visits.CountAsync();
        }
    }
}
