using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Classes;

internal class ServiceImpl<TRepository> : IService<TRepository>
    where TRepository : IRepository
{
    private readonly TRepository _repository;

    public ServiceImpl(TRepository repository)
    {
        _repository = repository;
    }

    public void Service(object o)
    {
        _repository.store(o);
    }
}
