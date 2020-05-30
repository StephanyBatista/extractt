using System.Collections.Generic;

namespace Extractt.Infra
{
    public class Response
    {
        public List<Regions> Regions {get; set;}
    }

    public class Regions
    {
        public List<Lines> Lines {get; set;}
    }

    public class Lines
    {
        public List<Words> Words {get; set;}
    }

    public class Words
    {
        public string Text {get; set;}
    }
}