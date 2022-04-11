using Commons.Models.Exception;
using Commons.Models.ViewModel;
using Commons.Repositories.Interface;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Worker.Services.Interfaces;
using Db = Commons.Models.Database;

namespace Worker.Services.Implementation
{
    public class VisitorsService : IVisitorsService
    {
        private IVisitorsRepository _visitorsRepository;
        private IConfigurationRoot _configuration;

        public VisitorsService(IVisitorsRepository visitorsRepository, IConfigurationRoot configuration)
        {
            _visitorsRepository = visitorsRepository;
            _configuration = configuration;
        }

        public async Task ProcessFiles()
        {
            var directory = _configuration.GetValue(typeof(string), "TempFilesDirectory").ToString();

            if (string.IsNullOrEmpty(directory))
            {
                throw new BusinessLogicException("Missing temp directory config.");
            }

            if (!Directory.Exists(directory))
            {
                throw new BusinessLogicException("Temp directory does not exist.");
            }

            var files = Directory.GetFiles(directory, "*.json", SearchOption.TopDirectoryOnly);

            Console.WriteLine(string.Join(", ", files));

            foreach (var file in files)
            {
                var fullPath = Path.Combine(directory, file);

                var visit = JsonConvert.DeserializeObject<Visit>(File.ReadAllText(fullPath));
                await _visitorsRepository.AddVisit(new Db.Visit
                {
                    Country = visit.Country,
                    Date = visit.Date.ToUniversalTime(),
                    Visitors = visit.Visitors
                });

                File.Delete(fullPath);
            }
        }
    }
}
