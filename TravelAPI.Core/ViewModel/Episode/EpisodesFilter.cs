using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalarinaAPI.Core.ViewModel
{
    public class EpisodesFilter
    {
        public int EpisodeId {  get; set; }
        public string EpisodeTitle {  get; set; }
        public int EpisodeViews {  get; set; }
        public DateTime EpisodePublishDate {  get; set; }
        public int ProgramId {  get; set; }
        public string ProgramName {  get; set; }
        public string ProgramImg {  get; set; }
        public int CategoryId {  get; set; }
        public string CategoryTitle {  get; set; }
        public int ProgramTypeId {  get; set; }
        public string ProgramTypeTitle {  get; set; }

    }
}
