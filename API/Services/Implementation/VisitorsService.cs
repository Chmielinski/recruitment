using API.BusCommunication;
using API.Services.Interface;
using Commons.Models.Exception;
using Commons.Models.ViewModel;
using Commons.Repositories.Interface;

namespace API.Services.Implementation
{
    public class VisitorsService : IVisitorsService
    {
        private IVisitorsRepository _visitorsRepository;
        private IConfigurationRoot _configuration;
        private readonly IFilesUploadMessageProducer _filesUploadMessageProducer;

        public VisitorsService(IVisitorsRepository visitorsRepository, IConfigurationRoot configuration,
            IFilesUploadMessageProducer filesUploadMessageProducer)
        {
            _visitorsRepository = visitorsRepository;
            _configuration = configuration;
            _filesUploadMessageProducer = filesUploadMessageProducer;
        }

        public async Task<int> GetProcessedFilesCount()
        {
            return await _visitorsRepository.GetVisitsCount();
        }

        public async Task<IEnumerable<VisitorsByCountry>> GetVisitorsCounts()
        {
            var allVisits = (await _visitorsRepository.GetAllVisits()).GroupBy(x => x.Country);

            return allVisits.Select(x => new VisitorsByCountry { Country = x.Key, Visitors = x.Sum(x => x.Visitors) }).ToList();
        }

        public async Task UploadVisits(IEnumerable<IFormFile> files)
        {
            if (files.Count() < 1)
            {
                throw new BusinessLogicException("No files provided.");
            }

            var directory = _configuration.GetValue(typeof(string), "TempFilesDirectory").ToString();

            if (string.IsNullOrEmpty(directory))
            {
                throw new BusinessLogicException("Missing temp directory config.");
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            foreach (var file in files.Select((x, y) => new { Value = x, Index = y }).ToList())
            {
                var fileName = $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_{file.Index}.json";
                var path = Path.Combine(directory, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await file.Value.CopyToAsync(stream);
            }

            _filesUploadMessageProducer.SendMessage(new FilesUploadedEvent());
        }
    }
}
