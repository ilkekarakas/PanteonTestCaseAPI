using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using PanteonTestCase.Context;
using PanteonTestCase.Dtos;
using PanteonTestCase.Enums;
using PanteonTestCase.Models;
using PanteonTestCase.Security;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;


namespace PanteonTestCase.Service
{
    public class PanteonTestCaseService
    {
        private readonly IConfiguration _configuration;
        PanteonTestCaseContext PanteonTestCaseContext = new PanteonTestCaseContext();
        public PanteonTestCaseService() : base()
        {

#if DEBUG
            IConfiguration Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            _configuration = Configuration;
#else
            IConfiguration Configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.Development.json")
               .Build();
            _configuration = Configuration;
#endif

        }
        #region Register Login
        public async Task<ServiceResult> Register(UserRequestDto model)
        {
            if (string.IsNullOrEmpty(model.Username))
            {
                return new ServiceResult("Username field cannot be empty.", ResultType.Warning);
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                return new ServiceResult("Mail field cannot be empty.", ResultType.Warning);
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return new ServiceResult("Password field cannot be empty.", ResultType.Warning);
            }

            if (!IsPasswordValid(model.Password))
            {
                return new ServiceResult("Password must be at least 8 characters and contain uppercase, lowercase and numbers.", ResultType.Warning);
            }

            var userRepo = PanteonTestCaseContext.Users.ToList();
            var userCheck = userRepo.Where(x => x.Email == model.Email || x.Username == model.Username).FirstOrDefault();
            string SHANewPassword = "";
            if (userCheck != null)
            {
                return new ServiceResult("This email address or username has been used before.", ResultType.Warning);
            }
            else
            {
                var user = new User();
                user.Username = model.Username;
                user.Password = SHA1Password(model.Password);
                user.Email = model.Email;
                PanteonTestCaseContext.Add(user);
                PanteonTestCaseContext.SaveChanges();
                return new ServiceResult("The user has been successfully created.", ResultType.Success);

            }
        }
        public async Task<ServiceResult<UserResponseDto>> Login(LoginRequestDto model)
        {
            var userRepo = PanteonTestCaseContext.Users.ToList();
            string SHAPassword = SHA1Password(model.Password);

            var userCheck = userRepo.Where(x => x.Email == model.Email && x.Password == SHAPassword).FirstOrDefault();
            if (userCheck != null)
            {
                var user = new UserResponseDto()
                {
                    Email = userCheck.Email,
                    Username = userCheck.Username,
                    Id = userCheck.Id,
                };
                user.Token = TokenHandler.CreateToken(user);
                return new ServiceResult<UserResponseDto>(user, "Login successful.", ResultType.Success);
            }
            else
            {
                return new ServiceResult<UserResponseDto>(null, "Your username or password is incorrect.", ResultType.Warning);

            }
        }
        #endregion Register Login

        #region Building Configuration
        public async Task<ServiceResult> AddOrUpdateBuildingConfiguration(BuildingConfigurationDto model)
        {
            if (model.BuildingCost <= 0)
            {
                return new ServiceResult<BuildingConfiguration>(null, "Building Cost cannot be 0 or less than 0.", ResultType.Warning);
            }
            if (model.ConstructionTime <= 29 || model.ConstructionTime >= 1801)
            {
                return new ServiceResult<BuildingConfiguration>(null, "Construction Time cannot be less than 30 or bigger than 1800.", ResultType.Warning);
            }

            var connectionString = _configuration.GetSection("MongoDbConnectionString").Value;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("panteon-db");
            var _collection = database.GetCollection<BuildingConfiguration>("BuildingConfiguration");

            if (!string.IsNullOrEmpty(model.Id))
            {
                //update
                var update = Builders<BuildingConfiguration>.Update
                         .Set(config => config.BuildingCost, model.BuildingCost)
                         .Set(config => config.BuildingType, model.BuildingType)
                         .Set(config => config.ConstructionTime, model.ConstructionTime);

                await _collection.UpdateOneAsync(config => config.Id == new ObjectId(model.Id), update);
            }
            else
            {
                //add
                var filter = Builders<BuildingConfiguration>.Filter.Eq(x => x.BuildingType, model.BuildingType);
                var result = await _collection.Find(filter).ToListAsync();
                if (result.Count() > 0)
                {
                    return new ServiceResult<BuildingConfiguration>(null, "This building type already exists, you cannot add it again.", ResultType.Warning);
                }

                var newConfig = new BuildingConfiguration
                {
                    BuildingType = model.BuildingType,
                    BuildingCost = model.BuildingCost,
                    ConstructionTime = model.ConstructionTime
                };

                await _collection.InsertOneAsync(newConfig);
            }

            return new ServiceResult("Transaction Successful.", ResultType.Success);

        }

        public async Task<ServiceResult<List<BuildingConfigurationDto>>> GetBuildingConfigurationList()
        {
            var connectionString = _configuration.GetSection("MongoDbConnectionString").Value;
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("panteon-db");
            var collection = database.GetCollection<BuildingConfiguration>("BuildingConfiguration");

            // Tüm verileri çekme
            var allBuildingConfiguration = await collection
                .Find(_ => true)
                .ToListAsync();

            return new ServiceResult<List<BuildingConfigurationDto>>(allBuildingConfiguration.Select(x => new BuildingConfigurationDto
            {
                BuildingCost = x.BuildingCost,
                Id = x.Id.ToString(),
                BuildingType = x.BuildingType,
                ConstructionTime = x.ConstructionTime
            }).ToList(), "Transaction Successful.", ResultType.Success);
        }
        public async Task<ServiceResult<List<BuildingTypesDto>>> GetBuildingTypes()
        {
            var data = new BuildingTypesDto();
            var connectionString = _configuration.GetSection("MongoDbConnectionString").Value;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("panteon-db");
            var collection = database.GetCollection<BuildingConfiguration>("BuildingConfiguration");

            var enumValues = Enum.GetValues(typeof(BuildingTypes)).Cast<BuildingTypes>().ToList();
            var comboBoxDataList = new List<BuildingTypesDto>();

            var buildingConfigs = await collection.Find(_ => true).ToListAsync();

            foreach (var enumValue in enumValues)
            {
                var matchingConfig = buildingConfigs.FirstOrDefault(config => config.BuildingType == (int)enumValue);

                if (matchingConfig == null)
                {
                    var comboBoxData = new BuildingTypesDto
                    {
                        Id = (int)enumValue,
                        Name = GetEnumDisplayValue(enumValue)
                    };
                    comboBoxDataList.Add(comboBoxData);
                }
            }


            return new ServiceResult<List<BuildingTypesDto>>(comboBoxDataList);

        }
        #endregion Building Configuration
        public static bool IsPasswordValid(string password)
        {
            string pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$";
            bool isValid = Regex.IsMatch(password, pattern);

            return isValid;
        }

        public static string GetEnumDisplayValue(Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attributes = fieldInfo.GetCustomAttributes(false);

                if (attributes.Length > 0 && attributes[0] is DisplayAttribute displayAttribute)
                {
                    // DisplayAttribute'in "Name" özelliği döndürülür (enum değeri için görsel adı).
                    return displayAttribute.Name;
                }
            }

            return enumValue.ToString();
        }


        public static string SHA1Password(string text)
        {
            SHA1 sha1 = SHA1.Create();

            byte[] hashData = sha1.ComputeHash(Encoding.Default.GetBytes(text));

            StringBuilder SHA1Password = new StringBuilder();

            for (int i = 0; i < hashData.Length; i++)
            {
                SHA1Password.Append(hashData[i].ToString("x2"));
            }
            return SHA1Password.ToString();
        }
    }
}
