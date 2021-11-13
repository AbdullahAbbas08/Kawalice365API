using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.Models;
using System.Threading.Tasks;
using TravelAPI.Core;
using TravelAPI.Interfaces;
using TravelAPI.Models;
using TravelAPI.Repositories;

namespace TravelAPI.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BalarinaDatabaseContext _context;
        public IBaseRepository<Category> category { get; private set; }

        public IBaseRepository<Episode> Episode { get; private set; }
        public IBaseRepository<EpisodesKeyword> EpisodesKeyword { get; private set; }
        public IBaseRepository<Interviewer> Interviewer { get; private set; }
        public IBaseRepository<Keyword> Keyword { get; private set; }
        public IBaseRepository<Program> Program { get; private set; }
        public IBaseRepository<ProgramType> ProgramType { get; private set; }
        public IBaseRepository<Seasons> Season  { get; private set; }
        public IBaseRepository<Sliders> Slider  { get; private set; }

        public IBaseRepository<ADTARGETS> ADTARGETS { get; private set; }
        public IBaseRepository<ADSTYLES> ADSTYLES { get; private set; }
        public IBaseRepository<ADPLACEHOLDER> ADPLACEHOLDER { get; private set; }
        public IBaseRepository<ADS> ADS { get; private set; }
        public IBaseRepository<ApplicationUser> ApplicationUser  { get; private set; }


        public UnitOfWork(BalarinaDatabaseContext _context)
        {
            this._context = _context;
            this.category = new BaseRepository<Category>(_context);
            this.Episode = new BaseRepository<Episode>(_context);
            this.EpisodesKeyword = new BaseRepository<EpisodesKeyword>(_context);
            this.Interviewer = new BaseRepository<Interviewer>(_context);
            this.Keyword = new BaseRepository<Keyword>(_context);
            this.Program = new BaseRepository<Program>(_context);
            this.ProgramType = new BaseRepository<ProgramType>(_context);
            this.Season = new BaseRepository<Seasons>(_context);
            this.Slider = new BaseRepository<Sliders>(_context);
            this.ADTARGETS = new BaseRepository<ADTARGETS>(_context);
            this.ADS = new BaseRepository<ADS>(_context);
            this.ADPLACEHOLDER = new BaseRepository<ADPLACEHOLDER>(_context);
            this.ADSTYLES = new BaseRepository<ADSTYLES>(_context);
            ApplicationUser = new BaseRepository<ApplicationUser>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
