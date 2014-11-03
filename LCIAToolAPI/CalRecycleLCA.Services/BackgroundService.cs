using LcaDataModel;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using CalRecycleLCA.Repositories;

namespace CalRecycleLCA.Services
{
    public interface IBackgroundService : IService<Background>
    {
        FragmentNodeResource ResolveBackground(int? flowId, int directionId, int scenarioId);
    }

    public class BackgroundService : Service<Background>, IBackgroundService
    {
        private readonly IRepositoryAsync<Background> _repository;

        public BackgroundService(IRepositoryAsync<Background> repository)
            : base(repository)
        {
            _repository = repository;
        }

        public FragmentNodeResource ResolveBackground(int? flowId, int directionId, int scenarioId)
        {
            return _repository.ResolveBackground(flowId, directionId, scenarioId);
        }
    }
}
