using BridgeCli;

namespace Jal123
{
    internal static class CleanUpCmdWrapper
    {
        public static void CleanUp(string dir)
        {
            CleanUpCmd cmd = new CleanUpCmd();
            cmd.Run(dir);
        }
    }
}