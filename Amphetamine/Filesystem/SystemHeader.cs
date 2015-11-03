using System.Runtime.InteropServices;

namespace Amphetamine.Filesystem
{
    /// <summary>
    /// A header placed at the start of a file system
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct SystemHeader
    {
        public const ulong MAGIC = 0x4673486561646572;

        [FieldOffset(0)] public readonly ulong Magic;

        private const int NAME_LENGTH = 128;
        [FieldOffset(sizeof(long))] public unsafe fixed char Name[NAME_LENGTH];

        public SystemHeader(string driveName)
        {
            Magic = MAGIC;

            unsafe
            {
                fixed (char* name = Name)
                {
                    //Copy characters
                    var i = 0;
                    foreach (var character in driveName)
                        name[i++] = character;

                    //Set the rest to null
                    for (; i < NAME_LENGTH; i++)
                        name[i] = '\0';
                }
            }
        }
    }
}
