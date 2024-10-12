using DesassemblerBCG16;

internal class Program
{
    private static void Main(string[] args)
    {
        byte[] src = File.ReadAllBytes(args[0]);
        DisAssembler disAssembler = new DisAssembler();
        disAssembler.Build(src);
        File.WriteAllLines("./BIOS.acl", disAssembler.m_Output);
    }
}