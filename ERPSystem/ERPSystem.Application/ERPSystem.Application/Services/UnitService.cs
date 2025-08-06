using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ERPSystem.Application.Interfaces;
using ERPSystem.Core.DTOs.Common;
using ERPSystem.Core.DTOs.Unit;
using ERPSystem.Core.Interfaces;

namespace ERPSystem.Application.Services
{
    public class UnitService : IUnitService
    {
        private readonly IMapper _mapper;
        private readonly IUnitRepository _repository;

        public UnitService(IMapper mapper, IUnitRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<UnitDto> GetByIdAsync(int id)
        {
            var unit = await _repository.GetByIdAsync(id);
            var unitDto = _mapper.Map<UnitDto>(unit);
            var productCount = await _repository.GetProductCountByUnitAsync(id);
            return unitDto;
        }

        public async Task<IEnumerable<UnitSelectDto>> GetActiveUnitsForSelectAsync()
        {
            var units = await _repository.GetActiveUnitsAsync();
            return _mapper.Map<IEnumerable<UnitSelectDto>>(units).OrderBy(x => x.Name);
        }

        public Task<PagedResultDto<UnitDto>> GetPagedUnitsAsync(UnitFilterDto filter)
        {
            throw new NotImplementedException();
        }

        public Task<UnitDto> CreateAsync(CreateUnitDto createDto)
        {
            throw new NotImplementedException();
        }

        public Task<UnitDto> UpdateAsync(UpdateUnitDto updateDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UnitUsageInfoDto> GetUnitUsageInfoAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsBySymbolAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<BulkOperationResultDto<UnitDto>> BulkCreateAsync(IEnumerable<CreateUnitDto> createDtos)
        {
            throw new NotImplementedException();
        }

        public Task<UnitDto> ToggleActiveStatusAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
