namespace DevilMind
{
    public class BundleVersion
    {
        public uint Public;
        public uint Release;
        public uint Develop;
        public uint BuildNumber;

        public override string ToString()
        {
            return Public + "." + Release + "." + Develop + "\n" + "BUILD NUMBER: " + BuildNumber;
        }
    }
        
}