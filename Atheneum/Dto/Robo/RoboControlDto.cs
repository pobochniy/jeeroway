using System;

namespace Atheneum.Dto.Robo
{
    public class RoboControlDto
    {
        public Guid RoboId { get; set; }

        //public int? timeJs { get; set; }

        public bool W { get; set; }

        public bool S { get; set; }

        public bool A { get; set; }

        public bool D { get; set; }
    }
}
