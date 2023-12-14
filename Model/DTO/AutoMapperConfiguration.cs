using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LiveWaitlistServer.Model.DTO;

namespace LiveWaitlistServer.Model
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<WaitlistHost, WaitlistHostResult>();
        }
    }
}