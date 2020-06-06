using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FutbolPlay.WebApi.Model
{
    public class PitchModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Hour { get; set; }
        public PitchType PitchType { get; set; }
    }

    public enum PitchType { Single, Multiple }
}
