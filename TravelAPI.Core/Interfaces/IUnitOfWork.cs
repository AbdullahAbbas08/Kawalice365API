using TravelAPI.Interfaces;
using System;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.Models;
using System.Threading.Tasks;

namespace TravelAPI.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Category>  category          { get; }
        IBaseRepository<Episode> Episode { get; }
        IBaseRepository<EpisodesKeyword> EpisodesKeyword { get; }
        IBaseRepository<Interviewer> Interviewer { get; }
        IBaseRepository<Keyword> Keyword { get; }
        IBaseRepository<Program> Program { get; }
        IBaseRepository<Seasons> Season { get; }
        IBaseRepository<Sliders> Slider  { get; }
        IBaseRepository<ProgramType> ProgramType { get; }
        IBaseRepository<ADTARGETS> ADTARGETS { get; }
        IBaseRepository<ADSTYLES> ADSTYLES { get; }
        IBaseRepository<ADPLACEHOLDER> ADPLACEHOLDER { get; }
        IBaseRepository<ADS> ADS { get; }
         

        Task<int> Complete(); 

    }
}
 