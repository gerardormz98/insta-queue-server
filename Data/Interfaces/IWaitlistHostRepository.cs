using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveWaitlistServer.Model;
using LiveWaitlistServer.Model.DTO;

namespace LiveWaitlistServer.Data
{
    public interface IWaitlistHostRepository
    {
        int GetCount();
        WaitlistHost? Get(string code);
        WaitlistHost Create(WaitlistHostCreateRequest hostRequest);
        WaitlistHost? Update(string waitlistCode, WaitlistHostUpdateRequest hostRequest);
        WaitlistHost? Delete(string waitlistCode);
    }
}