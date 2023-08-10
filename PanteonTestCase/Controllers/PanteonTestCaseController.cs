using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using PanteonTestCase.Context;
using PanteonTestCase.Dtos;
using PanteonTestCase.Service;
using PanteonTestCase.Security;
using Microsoft.AspNetCore.Authorization;
using PanteonTestCase.Models;

namespace PanteonTestCase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PanteonTestCaseController : ControllerBase
    {
        PanteonTestCaseService _PanteonTestCaseService = new PanteonTestCaseService();

        private readonly IConfiguration _configuration;

        public PanteonTestCaseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        #region Register Login
        [HttpPost("Register")]
        public async Task<ServiceResult> Register([FromBody] UserRequestDto model)
        {
            return await _PanteonTestCaseService.Register(model);
        }

        [HttpPost("Login")]
        public async Task<ServiceResult<UserResponseDto>> Login([FromBody] LoginRequestDto model)
        {
            return await _PanteonTestCaseService.Login(model).ConfigureAwait(false);
        }
        #endregion Register Login

        #region Building Configuration
        [HttpPost("AddOrUpdateBuildingConfiguration")]
        public async Task<ServiceResult> AddOrUpdateBuildingConfiguration([FromBody] BuildingConfigurationDto model)
        {
            return await _PanteonTestCaseService.AddOrUpdateBuildingConfiguration(model);
        }

        [HttpGet("GetBuildingConfigurationList")]
        public async Task<ServiceResult<List<BuildingConfigurationDto>>> GetBuildingConfigurationList()
        {
            return await _PanteonTestCaseService.GetBuildingConfigurationList();
        }      
        
        [HttpGet("GetBuildingTypes")]
        public async Task<ServiceResult<List<BuildingTypesDto>>> GetBuildingTypes()
        {
            return await _PanteonTestCaseService.GetBuildingTypes();
        }
        #endregion Building Configuration

    }
}