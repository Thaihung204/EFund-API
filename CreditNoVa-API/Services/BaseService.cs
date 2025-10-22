using EFund_API.WebApp.Models;
using EFund_API.WebApp.Services.Interfaces;
using EFund_API.WebApp.Models;

namespace EFund_API.WebApp.Services.Services
{
    public class BaseService
    {
        public readonly IRepository Repo;

        public BaseService(IRepository repo)
        {
            this.Repo = repo;
        }
    }
}